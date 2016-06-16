using System;
using Eventing.Core.Messages;

namespace Eventing.Core.Test
{
    public class DummyDomainEvent : DomainEvent
    {
        public DummyDomainEvent(string id) : base(id, DateTime.Now)
        {
        }
    }
}
