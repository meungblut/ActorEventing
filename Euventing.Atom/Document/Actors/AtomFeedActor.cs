using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;

namespace Euventing.Atom.Document
{
    public class AtomFeedActorNew : PersistentActor
    {
        private readonly IAtomDocumentActorBuilder builder;
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;
        private DocumentId nextHeadDocument;
        private string feedTitle;
        private string feedAuthor;

        public override string PersistenceId { get; }

        public AtomFeedActorNew(IAtomDocumentActorBuilder builder)
        {
            Console.WriteLine("Feed actor path is " + Self.Path);
            this.builder = builder;
            PersistenceId = "AtomFeedActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                Console.WriteLine("Received null message");
                return true;
            }

            Console.WriteLine("AtomFeedActor ReceiveCommand " + message.GetType());


            try
            {
                ((dynamic)this).Process((dynamic)message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing " + message.GetType() + e.ToString());
            }

            return true;
        }

        private void Process(string message)
        {
            Console.WriteLine(message);
        }

        private void Process(AtomDocumentFullEvent fullEvent)
        {
            Console.WriteLine("Feed received document full message");
            var documentId = new DocumentId(Guid.NewGuid().ToString());
            nextHeadDocument = documentId;
            var atomDocument = builder.GetActorRef();
            Console.WriteLine("Creating document" + documentId.Id);
            atomDocument.Tell(new CreateAtomDocumentCommand(
                feedTitle, feedAuthor, atomFeedId, documentId, currentFeedHeadDocument), Self);
        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            var documentId = new DocumentId(Guid.NewGuid().ToString());
            currentFeedHeadDocument = documentId;
            nextHeadDocument = documentId;
            var atomDocument = builder.GetActorRef();
            feedTitle = creationCommand.Title;
            feedAuthor = creationCommand.Author;
            atomDocument.Tell(new CreateAtomDocumentCommand(
                creationCommand.Title, creationCommand.Author, creationCommand.FeedId, currentFeedHeadDocument, creationCommand.EarlierEventsDocumentId), Self);
        }
        
        private void Process(FeedId feedId)
        {
            atomFeedId = feedId;
            currentFeedHeadDocument = new DocumentId(Guid.NewGuid().ToString());
        }

        private void Process(EventWithSubscriptionNotificationMessage message)
        {
            var notificationMessage = new EventWithDocumentIdNotificationMessage(currentFeedHeadDocument, message.EventToNotify);
            var atomDocument = builder.GetActorRef();
            atomDocument.Tell(notificationMessage, Self);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadIdRequest)
        {
            Sender.Tell(currentFeedHeadDocument, Self);
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadRequest)
        {
            Console.WriteLine("Getting document with id " + currentFeedHeadDocument.Id);

            var atomDocument =
                builder.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(currentFeedHeadDocument)).Result;
            Sender.Tell(atomDocument, Self);
        }

        private void MutateInternalState(AtomFeedDocumentHeadChanged headChanged)
        {
            currentFeedHeadDocument = headChanged.CurrentHeadDocumentId;
            
        }
    }
}
