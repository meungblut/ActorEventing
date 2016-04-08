using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;

namespace Euventing.Atom.Burst
{
    public class WorkPullingDocumentActor : AtomDocumentActorBase, IWithUnboundedStash
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private int entriesInCurrentDocument;
        private DocumentId documentIdToBeUsedAsNextHead;

        private readonly ConcurrentDictionary<Address, IActorRef> subscriptionsCurrentlyPolling = new ConcurrentDictionary<Address, IActorRef>();
        private SubscriptionsAtomFeedShouldPoll subscriptionsAtomFeedShouldPoll;

        public WorkPullingDocumentActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
        }

        private void PollQueues()
        {
            foreach (var member in subscriptionsAtomFeedShouldPoll.SubscriptionQueues)
            {
                PollSubscriptionQueue(member);
            }
        }

        private void PollSubscriptionQueue(IActorRef actorRef)
        {
            LoggingAdapter.Info($"Asking for events from node {actorRef.Path}");

            var eventsToRequest = atomDocumentSettings.NumberOfEventsPerDocument - entriesInCurrentDocument;

            actorRef.Tell(new RequestEvents(eventsToRequest));
        }

        protected void Process(CreateAtomDocumentCommand creationRequest)
        {
            LoggingAdapter.Info($"Created atom document with id {creationRequest.DocumentId.Id}");

            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.PreviousHeadDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
        }

        private void Process(SubscriptionsAtomFeedShouldPoll subscriptions)
        {
            LoggingAdapter.Info($"********Setting the subscriptions to poll");

            subscriptionsAtomFeedShouldPoll = subscriptions;
            PollQueues();
        }

        private void Process(RequestedEvents requestedEvents)
        {
            LoggingAdapter.Info($"Received {requestedEvents.Events.Count()} events");

            subscriptionsCurrentlyPolling.AddOrUpdate(requestedEvents.AddressOfSender, Context.Sender,
                (x, y) => Context.Sender);

            foreach (var requestedEvent in requestedEvents.Events)
            {
                //TODO: something with this.
                entriesInCurrentDocument++;

                Persist(requestedEvent.Message, MutateInternalState);
            }

            if (requestedEvents.MessagesRemaining > 0)
                LoggingAdapter.Info($"{requestedEvents.MessagesRemaining} remain in queue");

            LoggingAdapter.Info($"{entriesInCurrentDocument} in document against {atomDocumentSettings.NumberOfEventsPerDocument}");

            if (entriesInCurrentDocument >= atomDocumentSettings.NumberOfEventsPerDocument)
            {
                LoggingAdapter.Info($"Document is full");

                DocumentIsFull();
            }
            else
            {
                Self.Tell(new PollForEvents(Context.Sender));
                LoggingAdapter.Info($"Requesting events");
            }
        }

        private void DocumentIsFull()
        {
            Context.Parent.Tell(new DocumentFull(FeedId, DocumentId));
            LaterEventsDocumentId = documentIdToBeUsedAsNextHead;
        }

        private Address GetDifferentNodeIfPossible()
        {
            if (Cluster.ReadView.IsSingletonCluster)
                return Cluster.ReadView.SelfAddress;

            return Cluster.ReadView.Members.First(x => x.Address != Cluster.SelfAddress).Address;
        }

        private void MutateInternalState(AtomEntry entry)
        {
            Entries.Add(entry);
            LoggingAdapter.Info($"Added event {entry.Id} to feed {FeedId.Id} document {DocumentId.Id}");
        }

        private void Process(GetAtomDocumentRequest request)
        {
            GetCurrentAtomDocument();
        }

        private void Process(PollForEvents request)
        {
            PollSubscriptionQueue(request.AddressToPoll);
        }

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.FeedId;
            this.documentIdToBeUsedAsNextHead = documentCreated.NextEventsDocumentId;
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

    internal class CheckAllClusterMembersAreBeingPolled
    {
    }
}
