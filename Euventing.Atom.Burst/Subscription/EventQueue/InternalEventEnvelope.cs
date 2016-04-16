using System;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    internal class InternalEventEnvelope<T>
    {
        internal EventEnvelope<T> Events { get; }
        internal long HighestPersistenceIdInBatch { get; }
        internal DateTime requestTime { get; }

        public InternalEventEnvelope(EventEnvelope<T> events, long highestPersistenceIdInBatch, DateTime requestTime)
        {
            Events = events;
            HighestPersistenceIdInBatch = highestPersistenceIdInBatch;
            this.requestTime = requestTime;
        }
    }
}