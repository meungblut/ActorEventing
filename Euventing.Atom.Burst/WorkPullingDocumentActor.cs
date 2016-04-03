using System;
using System.Collections.Concurrent;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Feed;
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
        private readonly ShardedAtomFeedFactory shardedAtomFeedFactory;
        private DocumentId documentIdToBeUsedAsNextHead;

        private readonly ConcurrentDictionary<Address, IActorRef> subscriptionsCurrentlyPolling = new ConcurrentDictionary<Address, IActorRef>();

        public WorkPullingDocumentActor(IAtomDocumentSettings settings, ShardedAtomFeedFactory feedFactory)
        {
            shardedAtomFeedFactory = feedFactory;
            atomDocumentSettings = settings;
        }

        private void PollQueues()
        {
            Cluster = Cluster.Get(Context.System);

            foreach (var member in Cluster.ReadView.Members)
            {
                PollSubscriptionQueue(member.Address);
            }

            Context.System.Scheduler.ScheduleTellRepeatedly
                (TimeSpan.FromMilliseconds(50),
                TimeSpan.FromMilliseconds(50),
                Context.Self, new CheckAllClusterMembersAreBeingPolled(), Context.Self);
        }

        private void PollSubscriptionQueue(Address member)
        {
            LoggingAdapter.Info($"Asking for events from node {member.ToString()}");

            string addressFormat = "{0}/user/subscription_{1}_" + FeedId.Id;
            var address = string.Format(addressFormat, member, member.GetHashCode());

            //string addressFormat = "*/user/subscription_{0}_" + FeedId.Id;
            //var address = string.Format(addressFormat, member.GetHashCode());

            var actorRef = Context.System.ActorSelection(address);

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

            PollQueues();
        }

        private void Process(CheckAllClusterMembersAreBeingPolled checkMembers)
        {
            Cluster = Cluster.Get(Context.System);

            LoggingAdapter.Info($"Checking members to see if being polled");


            foreach (var member in Cluster.ReadView.Members)
            {
                if (!subscriptionsCurrentlyPolling.ContainsKey(member.Address))
                {
                    LoggingAdapter.Info($"Didn't find {member.Address} in poll list. Polling now");
                    PollSubscriptionQueue(member.Address);
                }
            }
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
                Self.Tell(new PollForEvents(requestedEvents.AddressOfSender));
            }
        }

        private void DocumentIsFull()
        {
            shardedAtomFeedFactory.GetActorRef().Tell(new DocumentFull(FeedId, DocumentId));
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
