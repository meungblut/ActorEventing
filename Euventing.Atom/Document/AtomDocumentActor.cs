using Akka.Persistence;
using System;
using System.Collections.Generic;
using Euventing.Atom.Serialization;
using Euventing.Atom.Test;
using Euventing.Core.Messages;

namespace Euventing.Atom.Document
{
    public class AtomDocumentActor : PersistentActor
    {
        public AtomDocumentActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
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

        private long sequenceNumber;
        private IAtomDocumentSettings atomDocumentSettings;

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

        private void Process(NewDocumentAddedEvent newDocumentEvent)
        {
            MutateInternalState(newDocumentEvent);
            Persist(newDocumentEvent, null);
        }

        private void MutateInternalState(NewDocumentAddedEvent newDocumentEvent)
        {
            this.laterEventsDocumentId = newDocumentEvent.DocumentId;
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
            sequenceNumber = sequenceNumber + 1;
            atomEntry.SequenceNumber = sequenceNumber;

            if (sequenceNumber >= atomDocumentSettings.NumberOfEventsPerDocument)
            {
                Sender.Tell(new AtomDocumentFullEvent(documentId), Self);
            }

            MutateInternalState(atomEntry);
            Persist(atomEntry, null);
        }

        private void Process(GetAtomDocumentRequest request)
        {
            Sender.Tell(new AtomDocument(title, author, feedId, documentId, earlierEventsDocumentId, laterEventsDocumentId, entries), Self);
        }

        private void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.author = documentCreated.Author;
            this.documentId = documentCreated.DocumentId;
            this.earlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.title = documentCreated.Title;
            this.feedId = documentCreated.FeedId;
        }

        private void MutateInternalState(RecoveryCompleted documentCreated)
        {
        }

        private void MutateInternalState(AtomEntry atomEntry)
        {
            entries.Add(atomEntry);
            sequenceNumber = atomEntry.SequenceNumber;
            updated = atomEntry.Updated;
        }
    }
}
