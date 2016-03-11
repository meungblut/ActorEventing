using System;
using Euventing.Core.Messages;

namespace Euventing.Test.Shared
{
    public class DummyEvent : DomainEvent
    {
        public DummyEvent(string id) : base(id, DateTime.Now)
        {
        }
    }
}