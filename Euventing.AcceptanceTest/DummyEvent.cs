using System;
using Euventing.Core.Messages;

namespace Euventing.AcceptanceTest
{
    internal class DummyEvent : DomainEvent
    {
        public DummyEvent(string id) : base(id, DateTime.Now)
        {
        }
    }
}