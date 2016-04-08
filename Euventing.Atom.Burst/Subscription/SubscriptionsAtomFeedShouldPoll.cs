using System.Collections.Generic;
using Akka.Actor;

namespace Euventing.Atom.Burst.Subscription
{
    internal class SubscriptionsAtomFeedShouldPoll
    {
        public List<IActorRef> SubscriptionQueues { get; }

        public SubscriptionsAtomFeedShouldPoll(List<IActorRef> subscriptionQueues)
        {
            SubscriptionQueues = subscriptionQueues;
        }
    }
}