using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription.EventQueue;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscription
{
    public class EventQueueActor : PersistentActorBase, IWithUnboundedStash
    {
        private readonly SubscriptionQueue<AtomEntry> queuedItems = new SubscriptionQueue<AtomEntry>();
        private readonly LowestItemTracker lowestItemTracker = new LowestItemTracker();
        private Address queueAddress;
        private readonly DomainEventToAtomEntryConverter converter = new DomainEventToAtomEntryConverter();

        protected override void PreStart()
        {
            LogTraceInfo("Starting subscription queue actor");
            queueAddress = Cluster.Get(Context.System).SelfAddress;
            base.PreStart();
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is DomainEvent)
                Enqueue((DomainEvent)message);

            if (message is RecoveryCompleted)
                this.UnstashAll();

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is RequestEvents)
                GetEvents((RequestEvents)message);

            if (message is DomainEvent)
                PersistAsync(message, x => Enqueue((DomainEvent)x));

            if (message is FeedDeleted)
                FeedHasStoppedListening((FeedDeleted)message);

            return true;
        }

        private void FeedHasStoppedListening(FeedDeleted message)
        {
            lowestItemTracker.RemoveListener(message.FeedId);
        }

        private void Enqueue(DomainEvent message)
        {
            LogTraceInfo($"Added event {message.Id} to queue with sequence number {LastSequenceNr}");
            var atomEntry = converter.ConvertDomainEventToAtomEntry(message);
            queuedItems.Add(atomEntry, LastSequenceNr);
        }

        private void GetEvents(RequestEvents eventRequest)
        {
            lowestItemTracker.AddEntry(eventRequest.LastProcessedId, eventRequest.FeedId.Id);
            queuedItems.RemoveItemsWithIndexLowerThan(lowestItemTracker.GetLowestValue() + 1);

            LogTraceInfo($"Received event request for {eventRequest.EventsToSend} events with {eventRequest.LastProcessedId} last events from actor {Context.Sender.Path}");

            List<ItemEnvelope<AtomEntry>> eventEnvelope;

            if (eventRequest.LastProcessedId == 0)
                eventEnvelope = queuedItems.Get(eventRequest.EventsToSend, eventRequest.EarliestEventsToSend);
                else
                eventEnvelope = queuedItems.Get(eventRequest.EventsToSend, eventRequest.LastProcessedId, eventRequest.FeedId);

            if (eventEnvelope.Count > 0)
                LogTraceInfo($"Returning events {string.Join(",", (from p in eventEnvelope select p.ItemToStore.Id).ToArray())} events with {eventRequest.LastProcessedId} last events from actor {Context.Sender.Path}");

            RequestedEvents<AtomEntry> events = new RequestedEvents<AtomEntry>(eventEnvelope, queueAddress);
            Context.Sender.Tell(events, Context.Self);

        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }
    }
}