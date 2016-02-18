using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocumentActor : PersistentActor
    {
        public AtomDocumentActor()
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
        }

        public string title;

        public DateTime updated;

        public string author;

        public string feedId;

        public DocumentId documentId;

        public string laterEventsDocumentId;

        public string earlierEventsDocumentId;

        public List<AtomEntry> entries;

        private int eventsPerDocument;

        protected override bool ReceiveRecover(object message)
        {
            Console.WriteLine("***********" + message.GetType().ToString());
            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        public override string PersistenceId { get; }

        private void Process(CreateAtomDocumentCommand creationRequest)
        {
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);
            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, null);
        }

        private void Process(GetAtomDocumentRequest request)
        {
            Sender.Tell(new AtomDocument(title, author, feedId, documentId, earlierEventsDocumentId), Self);
        }

        private void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.author = documentCreated.Author;
            this.documentId = documentCreated.DocumentId;
            this.earlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.title = documentCreated.Title;
            this.feedId = documentCreated.FeedId;

            //TODO: config this or something
            this.eventsPerDocument = 10;
        }

        private void MutateInternalState(RecoveryCompleted documentCreated)
        {
        }
    }
}
