using System;

namespace Eventing.Core.Messages
{
    public abstract class DomainEvent
    {
        public string Id { get; private set; }
        public DateTime OccurredTime { get; private set; }

        protected DomainEvent()
        {
            
        }
        protected DomainEvent(string id, DateTime occurred)
        {
            Id = id;
            OccurredTime = occurred;
        }
    }
}
