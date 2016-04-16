using System.Collections.Generic;
using Akka.Actor;
using Euventing.Atom.Burst.Subscription.EventQueue;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Subscription
{
    internal class RequestedEvents
    {
        public RequestedEvents(EventEnvelope<AtomEntry> events, int messagesRemaining, Address addressOfSender)
        {
            Events = events;
            AddressOfSender = addressOfSender;
        }

        public EventEnvelope<AtomEntry> Events { get; private set; } 

        public Address AddressOfSender { get; private set; }
    }
}