using System.Collections.Generic;
using Akka.Actor;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class AllLocalSubscriptionsActor : UntypedActor
    {
        protected HashSet<IActorRef> Subscribers = new HashSet<IActorRef>();

        private void Publish(DomainEvent domainEvent)
        {
            foreach (var subscriber in Subscribers)
            {
                subscriber.Tell(domainEvent);
            }
        }

        private void CreateSubscription(NewLocalSubscriptionCreated s)
        {
            Subscribers.Add(s.SubscriptionQueue);
        }

        protected override void OnReceive(object message)
        {
            if (message is NewLocalSubscriptionCreated)
                CreateSubscription((NewLocalSubscriptionCreated)message);

            string s = Context.Self.Path.Name.ToString();

            if (message is DomainEvent)
                Publish((DomainEvent)message);
        }
    }
}
