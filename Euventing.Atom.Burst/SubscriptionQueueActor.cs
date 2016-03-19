using System.Collections.Generic;
using Akka.Persistence;

namespace Euventing.Atom.Burst
{
    public class SubscriptionQueueActor : PersistentActor
    {
        readonly Queue<object> queuedItems = new Queue<object>();
        private bool shouldBeInThisStream = true;
        private int queueLength;

        public SubscriptionQueueActor()
        {
            PersistenceId = Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            Persist(message, x => queuedItems.Enqueue(message));
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is RequestEvents)
                DequeueAndSend();

            if (shouldBeInThisStream)
                Persist(message, x => Enqueue(x));

            return true;
        }

        private void Enqueue(object message)
        {
            queueLength++;
            queuedItems.Enqueue(message);
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
        public object Message { get; }
        public int QueueLength { get; }

        public QueuedEvent(object message, int queueLength)
        {
            Message = message;
            QueueLength = queueLength;
        }
    }
}