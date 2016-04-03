using System.Collections.Generic;
using Akka.Actor;

namespace Euventing.Atom.Burst.Subscription
{
    internal class RequestedEvents
    {
        public RequestedEvents(IEnumerable<QueuedEvent> events, int messagesRemaining, Address addressOfSender)
        {
            Events = events;
            MessagesRemaining = messagesRemaining;
            AddressOfSender = addressOfSender;
        }

        public int MessagesRemaining { get; private set; }

        public IEnumerable<QueuedEvent> Events { get; private set; } 

        public Address AddressOfSender { get; private set; }
    }
}