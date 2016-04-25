using System.Collections.Generic;
using Akka.Actor;
using Euventing.Atom.Burst.Subscription.EventQueue;

namespace Euventing.Atom.Burst.Subscription
{
    internal class RequestedEvents<T>
    {
        public RequestedEvents(List<ItemEnvelope<T>> events, Address addressOfSender)
        {
            Events = events;
            AddressOfSender = addressOfSender;
        }

        public List<ItemEnvelope<T>> Events { get; private set; } 

        public Address AddressOfSender { get; private set; }
    }
}