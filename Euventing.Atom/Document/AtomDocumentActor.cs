using Akka.Persistence;
using System;
using System.Collections.Generic;
using Euventing.Atom.Serialization;
using Euventing.Core.Messages;

namespace Euventing.Atom.Document
{
    public class AtomDocumentActor : PersistentActor
    {
        public AtomDocumentActor(IAtomDocumentSettings settings)
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
        }

        public string title;
        public DateTime updated;
        public string author;
        public FeedId feedId;
        public DocumentId documentId;
        public DocumentId laterEventsDocumentId;
        public DocumentId earlierEventsDocumentId;
        public List<AtomEntry> entries = new List<AtomEntry>();

        private int eventsPerDocument;
        private long sequenceNumber;

        protected override bool ReceiveRecover(object message)
        {
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

        private void Process(DomainEvent eventToAdd)
        {
            var atomEntry = new AtomEntry();
            var serializer = new JsonEventSerialisation();
            var content = serializer.GetContentWithContentType(eventToAdd);
            atomEntry.Content = content.Content;
            atomEntry.Id = eventToAdd.Id;
            atomEntry.Updated = eventToAdd.OccurredTime;
            atomEntry.Title = content.ContentType;
            atomEntry.SequenceNumber = sequenceNumber++;
            MutateInternalState(atomEntry);
            Persist(atomEntry, null);
        }

        private void Process(GetAtomDocumentRequest request)
        {
            Sender.Tell(new AtomDocument(title, author, feedId, documentId, earlierEventsDocumentId, entries), Self);
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

        private void MutateInternalState(AtomEntry atomEntry)
        {
            entries.Add(atomEntry);
            sequenceNumber = atomEntry.SequenceNumber;
        }
    }
}
