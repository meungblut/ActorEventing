using System;
using System.Collections.Generic;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class EventEnvelope<T>
    {
        public EventEnvelope(IEnumerable<T> events, Guid eventBatchId, int eventCount)
        {
            Events = events;
            EventBatchId = eventBatchId;
            EventCount = eventCount;
        }

        public IEnumerable<T> Events { get; private set; }
        public Guid EventBatchId { get; private set; }
        public int EventCount { get; private set; }
    }
}