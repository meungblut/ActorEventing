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
    public class AtomFeedActor : AtomFeedActorBase
    {
        private readonly IAtomDocumentActorFactory atomDocumentActorFactory;
        private readonly IAtomDocumentSettings settings;
        private int numberOfEventsInCurrentHeadDocument;
        private int currentDocumentId;
        private Stopwatch stopwatch = new Stopwatch();

        public AtomFeedActor(IAtomDocumentActorFactory builder, IAtomDocumentSettings settings)
        {
            this.atomDocumentActorFactory = builder;
            this.settings = settings;
        }

        protected override bool ReceiveRecover(object message)
        {
            try
            {
                ((dynamic)this).MutateInternalState((dynamic)message);
            }
            catch (Exception e)
            {
                LoggingAdapter.Error(e.ToString());
                throw new CouldNotProcessPersistenceMessage("Could not process " + message.GetType(), e);
            }

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                LoggingAdapter.Info("Received null message");
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
            if (this.CurrentFeedHeadDocument != null)
                throw new FeedAlreadyCreatedException(CurrentFeedHeadDocument.Id);

            var documentId = new DocumentId(creationCommand.FeedId.Id + "|0");
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author, creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            var atomDocument = atomDocumentActorFactory.GetActorRef();
            atomDocument.Tell(new CreateAtomDocumentCommand(
                creationCommand.Title, creationCommand.Author, creationCommand.FeedId, documentId, creationCommand.EarlierEventsDocumentId), Self);
        }

        private void Process(SaveSnapshotFailure failure)
        {
            LoggingAdapter.Error(failure.Metadata.PersistenceId + ":" + failure.Metadata.SequenceNr +
                "Snapshot save failure " + failure.Cause.ToString());
        }

        private void Process(SaveSnapshotSuccess success)
        {
            LoggingAdapter.Debug("Snapshot save succeeded " + success.Metadata.PersistenceId);
        }

        private void Process(EventWithSubscriptionNotificationMessage message)
        {
            if (CurrentFeedHeadDocument == null)
                throw new TryingToRaiseEventToFeedWithNoHeadException(PersistenceId);

            var notificationMessage = new EventWithDocumentIdNotificationMessage(CurrentFeedHeadDocument, message.EventToNotify);

            var atomDocument = atomDocumentActorFactory.GetActorRef();

            atomDocument.Tell(notificationMessage, Self);

            numberOfEventsInCurrentHeadDocument += 1;

            if (numberOfEventsInCurrentHeadDocument % 100 == 0)
            {
                var eventAdded = new EventAddedToDocument(numberOfEventsInCurrentHeadDocument);
                Persist(eventAdded, MutateInternalState);
            }

            if (numberOfEventsInCurrentHeadDocument >= settings.NumberOfEventsPerDocument)
            {
                var documentId = currentDocumentId + 1;
                var newDocumentId = new DocumentId(AtomFeedId.Id + "|" + documentId);

                Persist(new AtomFeedDocumentHeadChanged(newDocumentId, CurrentFeedHeadDocument, documentId), MutateInternalState);

                atomDocument.Tell(new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, AtomFeedId, newDocumentId, CurrentFeedHeadDocument), Self);

                atomDocument.Tell(new NewDocumentAddedEvent(CurrentFeedHeadDocument));
            }
        }

        private void CreateSnapshot()
        {
            var state = new AtomFeedState(this.AtomFeedId, this.CurrentFeedHeadDocument, this.LastHeadDocument,
                this.FeedTitle, this.FeedAuthor, this.numberOfEventsInCurrentHeadDocument, this.currentDocumentId);

            SaveSnapshot(state);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadIdRequest)
        {
            Sender.Tell(CurrentFeedHeadDocument, Self);
        }

        private void MutateInternalState(SnapshotOffer snapshotOffer)
        {
            AtomFeedState savedState;
            if (!(snapshotOffer.Snapshot is AtomFeedState))
                return;

            savedState = (AtomFeedState)snapshotOffer.Snapshot;

            this.CurrentFeedHeadDocument = savedState.CurrentFeedHeadDocument;
            this.AtomFeedId = savedState.AtomFeedId;
            this.FeedAuthor = savedState.FeedAuthor;
            this.FeedTitle = savedState.FeedTitle;
            this.LastHeadDocument = savedState.LastHeadDocument;
            this.numberOfEventsInCurrentHeadDocument = savedState.NumberOfEventsInCurrentHeadDocument;
            this.currentDocumentId = savedState.CurrentHeadDocumentIndex;
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadRequest)
        {
            var atomDocument =
                atomDocumentActorFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(CurrentFeedHeadDocument)).Result;
            Sender.Tell(atomDocument, Self);
        }

        private void MutateInternalState(AtomFeedDocumentHeadChanged headChanged)
        {
            CurrentFeedHeadDocument = headChanged.CurrentHeadDocumentId;
            LastHeadDocument = headChanged.EarlierDocumentId;
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
            AtomFeedId = atomFeedCreated.FeedId;
            CurrentFeedHeadDocument = atomFeedCreated.DocumentId;
            FeedTitle = atomFeedCreated.FeedTitle;
            FeedAuthor = atomFeedCreated.FeedAuthor;
        }

        private void MutateInternalState(object unknownRecoveryCommand)
        {
            LoggingAdapter.Info("Received Unknown recovery command: " + unknownRecoveryCommand.GetType().ToString());
        }

        private void Process(object unknownCommand)
        {
            LoggingAdapter.Info("Received Unknown command: " + unknownCommand.GetType().ToString());
        }
    }
}
