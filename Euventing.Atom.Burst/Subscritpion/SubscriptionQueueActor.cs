using System.Collections.Generic;
using System.Threading;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscritpion
{
    public class SubscriptionQueueActor : PersistentActorBase
    {
        readonly Queue<AtomEntry> queuedItems = new Queue<AtomEntry>();
        private bool shouldBeInThisStream = true;
        private int queueLength;
        private FeedId feedId;

        protected override void PreStart()
        {
            var actor = Context.ActorSelection("/user/" + ActorLocations.LocalSubscriptionManagerLocation);

            actor.Tell(new NewSubscription(Context.Self));

            base.PreStart();
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is DomainEvent)
                Persist(message, x => Enqueue((DomainEvent)message));

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is RequestEvents)
                DequeueAndSend((RequestEvents)message);

            if (message is DomainEvent && shouldBeInThisStream)
                Persist(message, x => Enqueue((DomainEvent)x));

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
                events.Add(new QueuedEvent(queuedItems.Dequeue(), queueLength));
                DeleteMessages(1, true);
            }

            Context.Sender.Tell(events, Context.Self);
        }
    }
}