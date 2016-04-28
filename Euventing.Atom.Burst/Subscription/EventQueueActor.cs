using System.Collections.Generic;
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

            return true;
        }

        private void Enqueue(DomainEvent message)
        {
            var atomEntry = converter.ConvertDomainEventToAtomEntry(message);
            queuedItems.Add(atomEntry, LastSequenceNr);
        }

        private void GetEvents(RequestEvents eventRequest)
        {
            LogTraceInfo($"Received event request for {eventRequest.EventsToSend} events with {eventRequest.LastProcessedId} last events");

            List<ItemEnvelope<AtomEntry>> eventEnvelope = queuedItems.Get(eventRequest.EventsToSend, eventRequest.LastProcessedId, eventRequest.FeedId);
            RequestedEvents<AtomEntry> events = new RequestedEvents<AtomEntry>(eventEnvelope, queueAddress);
            Context.Sender.Tell(events, Context.Self);
            lowestItemTracker.AddEntry(eventRequest.LastProcessedId, eventRequest.FeedId.Id);
            queuedItems.RemoveItemsWithIndexLowerThan(lowestItemTracker.GetLowestValue());
        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }
    }
}