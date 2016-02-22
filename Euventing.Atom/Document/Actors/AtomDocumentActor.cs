using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using Euventing.Atom.Serialization;

namespace Euventing.Atom.Document.Actors
{
    public class AtomDocumentActorNew : PersistentActor, IWithUnboundedStash
    {
        public AtomDocumentActorNew(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
            PersistenceId = "AtomDocumentActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
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
            //ToDo: establish where the nulls are coming from
            if (message == null)
                return true;

            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
                return true;

            Console.WriteLine("AtomDocumentActor ReceiveCommand " + message.GetType());

            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        public override string PersistenceId { get; }

        private void Process(CreateAtomDocumentCommand creationRequest)
        {
            Console.WriteLine("Creating document " + creationRequest.DocumentId);
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
            Sender.Tell(new DocumentReadyToReceiveEvents(this.documentId), Self);
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

        private void Process(EventWithDocumentIdNotificationMessage eventToAdd)
        {
            var atomEntry = new AtomEntry();
            var serializer = new JsonEventSerialisation();
            var content = serializer.GetContentWithContentType(eventToAdd.DomainEvent);
            atomEntry.Content = content.Content;
            atomEntry.Id = eventToAdd.DomainEvent.Id;
            atomEntry.Updated = eventToAdd.DomainEvent.OccurredTime;
            atomEntry.Title = content.ContentType;
            sequenceNumber = sequenceNumber + 1;
            atomEntry.SequenceNumber = sequenceNumber;

            Console.WriteLine("saving event: " + sequenceNumber + " to doc " + documentId.Id);
            Console.WriteLine("Greater than? " + atomDocumentSettings.NumberOfEventsPerDocument);

            Persist(atomEntry, MutateInternalState);

            if (sequenceNumber >= atomDocumentSettings.NumberOfEventsPerDocument)
            {
                Console.WriteLine("Document Full!");
                Console.WriteLine("Telling actor" + Sender.Path);

                Sender.Tell(new AtomDocumentFullEvent(documentId), Self);
            }
            Sender.Tell("Ack", Self);
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
