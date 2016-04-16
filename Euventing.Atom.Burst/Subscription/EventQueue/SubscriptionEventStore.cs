using System;
using System.Collections.Generic;
using System.Linq;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class SubscriptionEventStore<T>
    {
        private readonly Queue<Tuple<long, T>> queuedEntries = new Queue<Tuple<long, T>>();
        public long LowestCurrentlyHeldPersistenceId { get; private set; }
        public int EventsInQueue { get; private set; }

        private readonly Dictionary<Guid, InternalEventEnvelope<T>> unconfirmedBatches = new Dictionary<Guid, InternalEventEnvelope<T>>();

        private long lastPersistenceId;

        public void Add(T entry, long persistenceReference)
        {
            if (persistenceReference <= lastPersistenceId)
                throw new PersistenceIdAlreadyAddedException(persistenceReference);

            queuedEntries.Enqueue(new Tuple<long, T>(persistenceReference, entry));
            lastPersistenceId = persistenceReference;
            EventsInQueue++;
        }

        public EventEnvelope<T> Get(int entriesToGet)
        {
            long highestPersistenceIdInBatch = 0;
            List<T> events = new List<T>(entriesToGet);

            int entriesReturned;
            for (entriesReturned = 0; entriesReturned < entriesToGet; entriesReturned++)
            {
                if (queuedEntries.Count == 0)
                    break;

                var entry = queuedEntries.Dequeue();
                events.Add(entry.Item2);
                highestPersistenceIdInBatch = entry.Item1;
                EventsInQueue--;
            }

            var eventEnvelope = new EventEnvelope<T>(events, Guid.NewGuid(), entriesReturned);
            var internalEnvelope = new InternalEventEnvelope<T>(eventEnvelope, highestPersistenceIdInBatch, DateTime.Now);
            unconfirmedBatches.Add(eventEnvelope.EventBatchId, internalEnvelope);

            return eventEnvelope;
        }

        public void ConfirmHandled(Guid eventBatchId)
        {
            var internalEnvelope = unconfirmedBatches[eventBatchId];
            unconfirmedBatches.Remove(eventBatchId);

            if (ThereIsNoLowerPersistenceIdInAPendingBatch(internalEnvelope.HighestPersistenceIdInBatch))
                LowestCurrentlyHeldPersistenceId = internalEnvelope.HighestPersistenceIdInBatch;
        }

        private bool ThereIsNoLowerPersistenceIdInAPendingBatch(long highestPersistenceIdInBatch)
        {
            return !unconfirmedBatches.Any(x => x.Value.HighestPersistenceIdInBatch < highestPersistenceIdInBatch);
        }
    }
}
