using System.Collections.Generic;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class SubscriptionQueueActor : PersistentActorBase
    {
        readonly Queue<AtomEntry> queuedItems = new Queue<AtomEntry>();
        private bool shouldBeInThisStream = true;
        private int queueLength;

        protected override bool ReceiveRecover(object message)
        {
            if (message is DomainEvent)
                Persist(message, x => Enqueue((DomainEvent)message));

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is RequestEvents)
                DequeueAndSend();

            if (shouldBeInThisStream)
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

        private void DequeueAndSend()
        {
            queueLength--;
            var sendItem = new QueuedEvent(queuedItems.Dequeue(), queueLength);
            Context.Sender.Tell(sendItem, Self);
            DeleteMessages(1, true);
        }
    }
}