using System;
using Euventing.Core.Messages;

namespace Euventing.ConsoleHost
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