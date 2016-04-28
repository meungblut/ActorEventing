using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class AtomDocumentActor : AtomDocumentActorBase, IWithUnboundedStash
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private int entriesInCurrentDocument;

        private bool feedCancelled = false;

        private long lastEventIdProcessed;
        private readonly IAtomDocumentRepository _repository;

        private readonly IActorRef eventQueueOnThisNode;

        private DocumentId CurrentDocumentId;
        
        public AtomDocumentActor(IAtomDocumentSettings settings, IAtomDocumentRepository repository)
        {
            _repository = repository;
            atomDocumentSettings = settings;

            eventQueueOnThisNode = ActorLocations.LocalQueueActor;
        }

        private void PollSubscriptionQueue(IActorRef actorRef)
        {
            LogTraceInfo($"Asking for events from node {actorRef.Path}");

            var eventsToRequest = atomDocumentSettings.NumberOfEventsPerDocument - entriesInCurrentDocument;

            actorRef.Tell(new RequestEvents(eventsToRequest, lastEventIdProcessed));
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LogTraceInfo($"Cancelling document");

            feedCancelled = true;
        }

        private void Process(CreateAtomDocumentCommand createDocument)
        {
            var createdEvent = new AtomDocumentCreatedEvent(
                createDocument.Title,
                createDocument.Author,
                createDocument.DocumentId);

            Persist(createdEvent, MutateInternalState);
        }

        private void Process(RequestedEvents<AtomEntry> requestedEvents)
        {
            AddEventsToDocument(requestedEvents);

            if (feedCancelled)
                return;

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Self.Tell(new PollForEvents(Context.Sender));
        }

        private void AddEventsToDocument(RequestedEvents<AtomEntry> requestedEvents)
        {
            foreach (var itemEnvelope in requestedEvents.Events)
            {
                LogTraceInfo($"Saving {itemEnvelope.ItemToStore.Id} event to repo");

                itemEnvelope.ItemToStore.DocumentId = this.CurrentDocumentId;

                _repository.Add((itemEnvelope.ItemToStore));

                lastEventIdProcessed = itemEnvelope.ItemSequenceNumber;
            }

            entriesInCurrentDocument += requestedEvents.Events.Count;

            if (entriesInCurrentDocument > atomDocumentSettings.NumberOfEventsPerDocument)
            {
                DocumentId = DocumentId.Add(1);
                Context.Parent.Tell(new DocumentMovedToNewId(DocumentId));
            }
        }

        private void Process(PollForEvents request)
        {
            PollSubscriptionQueue(request.AddressToPoll);
        }

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.CurrentDocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.FeedId;

            PollSubscriptionQueue(eventQueueOnThisNode);
        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }
    }

    internal class DocumentMovedToNewId
    {
        public DocumentId DocumentId { get; }

        public DocumentMovedToNewId(DocumentId documentId)
        {
            DocumentId = documentId;
        }
    }
}
