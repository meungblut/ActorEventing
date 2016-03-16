using System;
using System.Diagnostics;
using Akka.Actor;
using Akka.Cluster;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Persistence;
using Euventing.Core;

namespace Euventing.Atom.Document.Actors
{
    public class AtomFeedActor : PersistentActor
    {
        private readonly ILoggingAdapter loggingAdapter;
        private readonly IAtomDocumentActorFactory atomDocumentActorFactory;
        private readonly IAtomDocumentSettings settings;
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;
        private DocumentId lastHeadDocument;
        private string feedTitle;
        private string feedAuthor;
        private int numberOfEventsInCurrentHeadDocument;
        private int currentDocumentId;
        private Stopwatch stopwatch = new Stopwatch();

        private int recoveryMessages = 0;

        public override string PersistenceId { get; }

        public AtomFeedActor(IAtomDocumentActorFactory builder, IAtomDocumentSettings settings)
        {
            loggingAdapter = Context.GetLogger();
            this.atomDocumentActorFactory = builder;
            this.settings = settings;
            PersistenceId = "AtomFeedActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            try
            {
                ((dynamic)this).MutateInternalState((dynamic)message);
            }
            catch (Exception e)
            {
                loggingAdapter.Error(e.ToString());
                throw new CouldNotProcessPersistenceMessage("Could not process " + message.GetType(), e);
            }

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {

            if (message == null)
            {
                loggingAdapter.Info("Received null message");
                return false;
            }

            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        private void Process(string message)
        {

        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            if (this.currentFeedHeadDocument != null)
                throw new FeedAlreadyCreatedException(currentFeedHeadDocument.Id);

            var documentId = new DocumentId(creationCommand.FeedId.Id + "|0");
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author, creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            var atomDocument = atomDocumentActorFactory.GetActorRef();
            atomDocument.Tell(new CreateAtomDocumentCommand(
                creationCommand.Title, creationCommand.Author, creationCommand.FeedId, documentId, creationCommand.EarlierEventsDocumentId), Self);
        }

        private void Process(SaveSnapshotFailure failure)
        {
            loggingAdapter.Error(failure.Metadata.PersistenceId + ":" + failure.Metadata.SequenceNr +
                "Snapshot save failure " + failure.Cause.ToString());
        }

        private void Process(SaveSnapshotSuccess success)
        {
            loggingAdapter.Debug("Snapshot save succeeded " + success.Metadata.PersistenceId);
        }

        private void Process(EventWithSubscriptionNotificationMessage message)
        {
            if (currentFeedHeadDocument == null)
                throw new TryingToRaiseEventToFeedWithNoHeadException(PersistenceId);

            var notificationMessage = new EventWithDocumentIdNotificationMessage(currentFeedHeadDocument, message.EventToNotify);

            var atomDocument = atomDocumentActorFactory.GetActorRef();

            atomDocument.Tell(notificationMessage, Self);

            var currentEvents = numberOfEventsInCurrentHeadDocument + 1;
            var eventAdded = new EventAddedToDocument(currentEvents);
            Persist(eventAdded, MutateInternalState);

            if (currentEvents >= settings.NumberOfEventsPerDocument)
            {
                var documentId = currentDocumentId + 1;
                var newDocumentId = new DocumentId(atomFeedId.Id + "|" + documentId);

                Persist(new AtomFeedDocumentHeadChanged(newDocumentId, currentFeedHeadDocument, documentId), MutateInternalState);

                atomDocument.Tell(new CreateAtomDocumentCommand(
                    feedTitle, feedAuthor, atomFeedId, newDocumentId, currentFeedHeadDocument), Self);

                atomDocument.Tell(new NewDocumentAddedEvent(currentFeedHeadDocument));
            }
        }

        private void CreateSnapshot()
        {
            var state = new AtomFeedState(this.atomFeedId, this.currentFeedHeadDocument, this.lastHeadDocument,
                this.feedTitle, this.feedAuthor, this.numberOfEventsInCurrentHeadDocument, this.currentDocumentId);

            SaveSnapshot(state);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadIdRequest)
        {
            Sender.Tell(currentFeedHeadDocument, Self);
        }

        private void MutateInternalState(SnapshotOffer snapshotOffer)
        {
            AtomFeedState savedState;
            if (!(snapshotOffer.Snapshot is AtomFeedState))
                return;

            savedState = (AtomFeedState)snapshotOffer.Snapshot;

            this.currentFeedHeadDocument = savedState.CurrentFeedHeadDocument;
            this.atomFeedId = savedState.AtomFeedId;
            this.feedAuthor = savedState.FeedAuthor;
            this.feedTitle = savedState.FeedTitle;
            this.lastHeadDocument = savedState.LastHeadDocument;
            this.numberOfEventsInCurrentHeadDocument = savedState.NumberOfEventsInCurrentHeadDocument;
            this.currentDocumentId = savedState.CurrentHeadDocumentIndex;
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadRequest)
        {
            var atomDocument =
                atomDocumentActorFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(currentFeedHeadDocument)).Result;
            Sender.Tell(atomDocument, Self);
        }

        private void MutateInternalState(AtomFeedDocumentHeadChanged headChanged)
        {
            currentFeedHeadDocument = headChanged.CurrentHeadDocumentId;
            lastHeadDocument = headChanged.EarlierDocumentId;
            currentDocumentId = headChanged.CurrentDocumentIndex;
            numberOfEventsInCurrentHeadDocument = 0;

            CreateSnapshot();
        }

        private void MutateInternalState(EventAddedToDocument eventAdded)
        {
            numberOfEventsInCurrentHeadDocument = eventAdded.CurrentEvents;
        }

        private void MutateInternalState(AtomFeedCreated atomFeedCreated)
        {
            atomFeedId = atomFeedCreated.FeedId;
            currentFeedHeadDocument = atomFeedCreated.DocumentId;
            feedTitle = atomFeedCreated.FeedTitle;
            feedAuthor = atomFeedCreated.FeedAuthor;
        }

        private void MutateInternalState(object unknownRecoveryCommand)
        {
            loggingAdapter.Info("Received Unknown recovery command: " + unknownRecoveryCommand.GetType().ToString());
        }

        private void Process(object unknownCommand)
        {
            loggingAdapter.Info("Received Unknown command: " + unknownCommand.GetType().ToString());
        }
    }
}
