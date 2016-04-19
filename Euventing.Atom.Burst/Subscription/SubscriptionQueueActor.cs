using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription.EventQueue;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscription
{
    public class SubscriptionQueueActor : PersistentActorBase, IWithUnboundedStash
    {
        readonly SubscriptionEventStore<AtomEntry> queuedItems = new SubscriptionEventStore<AtomEntry>();
        private bool shouldBeInThisStream = true;
        private Address queueAddress;
        readonly DomainEventToAtomEntryConverter converter = new DomainEventToAtomEntryConverter();

        protected override void PreStart()
        {
            LogTraceInfo("Starting subscription queue actor");

            var actor = Context.ActorSelection("/user/" + ActorLocations.LocalSubscriptionManagerLocation);
            actor.Tell(new NewLocalSubscriptionCreated(Context.Self));

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

            if (message is DomainEvent && shouldBeInThisStream)
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
            var eventEnvelope = queuedItems.Get(eventRequest.MaxEventsToSend);
            Context.Sender.Tell(new RequestedEvents(eventEnvelope, eventEnvelope.EventCount, queueAddress), Context.Self);
        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }
    }
}