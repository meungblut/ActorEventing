using System;
using Eventing.Core.Messages;

namespace Eventing.Test.Shared
{
    public class DummyEvent : DomainEvent
    {
        public DummyEvent(string id) : base(id, DateTime.Now)
        {
        }
    }
}