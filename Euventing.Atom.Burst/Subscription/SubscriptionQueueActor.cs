using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscription
{
    public class SubscriptionQueueActor : PersistentActorBase, IWithUnboundedStash
    {
        readonly Queue<AtomEntry> queuedItems = new Queue<AtomEntry>();
        private bool shouldBeInThisStream = true;
        private int queueLength;
        private Address queueAddress;

        protected override void PreStart()
        {
            Context.GetLogger().Info("Starting subscription queue actor with id " + Context.Self.Path);

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
                DequeueAndSend((RequestEvents)message);

            if (message is DomainEvent && shouldBeInThisStream)
                PersistAsync(message, x => Enqueue((DomainEvent)x));

            return true;
        }

        private void Enqueue(DomainEvent message)
        {
            queueLength++;
            var converter = new DomainEventToAtomEntryConverter();
            var atomEntry = converter.ConvertDomainEventToAtomEntry(message);
            queuedItems.Enqueue(atomEntry);
        }

        private void DequeueAndSend(RequestEvents eventRequest)
        {
            int i = 0;
            var events = new List<QueuedEvent>();

            while (i < eventRequest.MaxEventsToSend && queueLength > 0)
            {
                queueLength--;
                events.Add(new QueuedEvent(queuedItems.Dequeue()));
                DeleteMessages(1, true);
                i++;
            }

            Context.Sender.Tell(new RequestedEvents(events, queueLength, queueAddress), Context.Self);
        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }
    }
}