using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using Akka.Cluster;

namespace Euventing.Atom.Document
{
    public class AtomFeedActor : PersistentActor
    {
        private readonly IAtomDocumentActorBuilder builder;
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;
        private DocumentId lastHeadDocument;
        private string feedTitle;
        private string feedAuthor;
        private int numberOfEventsInCurrentHeadDocument;

        public override string PersistenceId { get; }

        public AtomFeedActor(IAtomDocumentActorBuilder builder)
        {
            Console.WriteLine("Feed actor path is " + Self.Path);
            this.builder = builder;
            PersistenceId = "AtomFeedActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message == null)
            {
                Console.WriteLine("Received null message");
                return true;
            }

            Console.WriteLine("AtomFeedActor ReceiveCommand " + message.GetType());

            try
            {
                ((dynamic) this).MutateInternalState((dynamic) message);
            }
            catch (Exception e)
            {
                throw new CouldNotProcessPersistenceMessage("Could not process " + message.GetType(), e);
            }

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                Console.WriteLine("Received null message");
                return true;
            }

            try
            {
                ((dynamic)this).Process((dynamic)message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing " + message.GetType() + " " + e.ToString());
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

        private void Process(EventWithSubscriptionNotificationMessage message)
        {
            var notificationMessage = new EventWithDocumentIdNotificationMessage(currentFeedHeadDocument, message.EventToNotify);
            var atomDocument = builder.GetActorRef();
            atomDocument.Tell(notificationMessage, Self);

            var currentEvents = numberOfEventsInCurrentHeadDocument + 1;
            var eventAdded = new EventAddedToDocument(currentEvents);
            Persist(eventAdded, MutateInternalState);

            Console.WriteLine("Adding event {0} with id {3} to doc {1} on node {2}", numberOfEventsInCurrentHeadDocument, currentFeedHeadDocument.Id, Cluster.Get(Context.System).SelfAddress, message.EventToNotify.Id);

            if (currentEvents >= 150)
            {
                var newDocumentId = new DocumentId(Guid.NewGuid().ToString());

                Persist(new AtomFeedDocumentHeadChanged(newDocumentId, currentFeedHeadDocument), MutateInternalState);
                
                atomDocument.Tell(new CreateAtomDocumentCommand(
                    feedTitle, feedAuthor, atomFeedId, newDocumentId, currentFeedHeadDocument), Self);

                atomDocument.Tell(new NewDocumentAddedEvent(newDocumentId));
            }
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadIdRequest)
        {
            Sender.Tell(currentFeedHeadDocument, Self);
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
            Console.WriteLine("Received Unknown recovery command: " + unknownRecoveryCommand.GetType().ToString());
        }
    }
}
