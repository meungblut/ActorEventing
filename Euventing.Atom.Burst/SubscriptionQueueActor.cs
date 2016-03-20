using System.Collections.Generic;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class SubscriptionQueueActor : PersistentActor
    {
        readonly Queue<AtomEntry> queuedItems = new Queue<AtomEntry>();
        private bool shouldBeInThisStream = true;
        private int queueLength;

        public SubscriptionQueueActor()
        {
            PersistenceId = Context.Parent.Path.Name + "|" + Self.Path.Name;
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
                DequeueAndSend();

            if (shouldBeInThisStream)
                Persist(message, x => Enqueue((DomainEvent)x));

            return true;
        }

        private void Enqueue(DomainEvent message)
        {
            queueLength++;
            queuedItems.Enqueue(new AtomEntry());
        }

        private void DequeueAndSend()
        {
            queueLength--;
            var sendItem = new QueuedEvent(queuedItems.Dequeue(), queueLength);
            Context.Sender.Tell(sendItem, Self);
        }

        public override string PersistenceId { get; }
    }

    public class QueuedEvent
    {
        public AtomEntry Message { get; }
        public int QueueLength { get; }

        public QueuedEvent(AtomEntry message, int queueLength)
        {
            Message = message;
            QueueLength = queueLength;
        }
    }
}