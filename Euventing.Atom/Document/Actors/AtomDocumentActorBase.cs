using System;
using System.Collections.Generic;
using Akka.Cluster;
using Akka.Event;
using Akka.Persistence;
using Euventing.Core;

namespace Euventing.Atom.Document.Actors
{
    public abstract class AtomDocumentActorBase : PersistentActorBase
    {
        protected string Title;
        protected DateTime Updated;
        protected string Author;
        protected FeedId FeedId;
        protected DocumentId DocumentId;
        protected DocumentId LaterEventsDocumentId;
        protected DocumentId EarlierEventsDocumentId;
        protected readonly List<AtomEntry> Entries = new List<AtomEntry>();

        protected long SequenceNumber;

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.FeedId;
        }

        private void Process(CreateAtomDocumentCommand creationRequest)
        {
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
        }

        protected void GetCurrentAtomDocument()
        {
            var atomDocument = new AtomDocument(Title, Author, FeedId, DocumentId, EarlierEventsDocumentId,
    LaterEventsDocumentId, Entries);
            atomDocument.AddDocumentInformation(Cluster.Get(Context.System).SelfAddress.ToString());
            Sender.Tell(atomDocument, Self);
        }
    }
}
