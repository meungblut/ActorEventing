using System;
using Eventing.Core.Messages;

namespace Eventing.ConsoleHost
{
    public class DummyDomainEvent : DomainEvent
    {
        public string Contents { get; }

        public DummyDomainEvent(string contents) : base(contents, DateTime.Now)
        {
            Contents = contents;
        }
    }
}