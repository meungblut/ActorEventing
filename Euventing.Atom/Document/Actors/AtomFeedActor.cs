using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using Akka.Cluster;
using Akka.Event;
using Euventing.Atom.Document.Actors;

namespace Euventing.Atom.Document
{
    public class AtomFeedActor : PersistentActor
    {
        private readonly ILoggingAdapter loggingAdapter;
        private readonly IAtomDocumentActorBuilder builder;
        private readonly IAtomDocumentSettings settings;
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;
        private DocumentId lastHeadDocument;
        private string feedTitle;
        private string feedAuthor;
        private int numberOfEventsInCurrentHeadDocument;

        public override string PersistenceId { get; }

        public AtomFeedActor(IAtomDocumentActorBuilder builder, IAtomDocumentSettings settings)
        {
            loggingAdapter = Context.GetLogger();
            loggingAdapter.Info("Feed actor path is " + Self.Path);
            this.builder = builder;
            this.settings = settings;
            PersistenceId = "AtomFeedActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            loggingAdapter.Info("AtomFeedActor ReceiveRecover " + message.GetType());

            try
            {
                ((dynamic) this).MutateInternalState((dynamic) message);
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

            try
            {
                ((dynamic)this).Process((dynamic)message);
            }
            catch (Exception e)
            {
                loggingAdapter.Error(e.ToString());
                throw new CouldNotProcessPersistenceMessage("Could not process " + message.GetType(), e);
            }

            return true;
        }

        private void Process(string message)
        {
            Console.WriteLine(message);
        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            var documentId = new DocumentId(Guid.NewGuid().ToString());
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author, creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            var atomDocument = builder.GetActorRef();
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
            var notificationMessage = new EventWithDocumentIdNotificationMessage(currentFeedHeadDocument, message.EventToNotify);
            var atomDocument = builder.GetActorRef();
            atomDocument.Tell(notificationMessage, Self);

            var currentEvents = numberOfEventsInCurrentHeadDocument + 1;
            var eventAdded = new EventAddedToDocument(currentEvents);
            Persist(eventAdded, MutateInternalState);

            loggingAdapter.Info("Adding event {0} with id {3} to doc {1} on node {2}", numberOfEventsInCurrentHeadDocument, currentFeedHeadDocument.Id, Cluster.Get(Context.System).SelfAddress, message.EventToNotify.Id);

            if (currentEvents >= settings.NumberOfEventsPerDocument)
            {
                var newDocumentId = new DocumentId(Guid.NewGuid().ToString());

                Persist(new AtomFeedDocumentHeadChanged(newDocumentId, currentFeedHeadDocument), MutateInternalState);
                
                atomDocument.Tell(new CreateAtomDocumentCommand(
                    feedTitle, feedAuthor, atomFeedId, newDocumentId, currentFeedHeadDocument), Self);

                atomDocument.Tell(new NewDocumentAddedEvent(newDocumentId));

                CreateSnapshot();
            }
        }

        private void CreateSnapshot()
        {
            var state = new AtomFeedState(this.atomFeedId, this.currentFeedHeadDocument, this.lastHeadDocument,
                this.feedTitle, this.feedAuthor, this.numberOfEventsInCurrentHeadDocument);
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

            savedState = (AtomFeedState) snapshotOffer.Snapshot;

            this.currentFeedHeadDocument = savedState.CurrentFeedHeadDocument;
            this.atomFeedId = savedState.AtomFeedId;
            this.feedAuthor = savedState.FeedAuthor;
            this.feedTitle = savedState.FeedTitle;
            this.lastHeadDocument = savedState.LastHeadDocument;
            this.numberOfEventsInCurrentHeadDocument = savedState.NumberOfEventsInCurrentHeadDocument;
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadRequest)
        {
            var atomDocument =
                builder.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(currentFeedHeadDocument)).Result;
            Sender.Tell(atomDocument, Self);
        }

        private void MutateInternalState(AtomFeedDocumentHeadChanged headChanged)
        {
            currentFeedHeadDocument = headChanged.CurrentHeadDocumentId;
            lastHeadDocument = headChanged.EarlierDocumentId;
            numberOfEventsInCurrentHeadDocument = 0;
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
    }
}
