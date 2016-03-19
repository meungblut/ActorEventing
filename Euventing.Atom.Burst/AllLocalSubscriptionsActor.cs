using System.Collections.Generic;
using Akka.Actor;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class AllLocalSubscriptionsActor : ReceiveActor
    {
        protected HashSet<IActorRef> Subscribers = new HashSet<IActorRef>();

        public AllLocalSubscriptionsActor()
        {
            Receive<NewSubscription>(s => CreateSubscription(s));
            Receive<DomainEvent>(s => Publish(s));
        }

        private void Publish(DomainEvent domainEvent)
        {
            foreach (var subscriber in Subscribers)
            {
                subscriber.Tell(domainEvent);
            }
        }

        private void CreateSubscription(NewSubscription s)
        {
            Subscribers.Add(s.SubscriptionQueue);
        }
    }

    public class NewSubscription
    {
        public IActorRef SubscriptionQueue { get; }
    }
}
