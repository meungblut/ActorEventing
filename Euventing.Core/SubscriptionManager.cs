using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Core
{
    public class SubscriptionManager
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef shardedSubscriptionActorRef;

        public SubscriptionManager(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var settings = ClusterShardingSettings.Create(actorSystem);

            shardedSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "TransactionCounterActor",
                entityProps: Props.Create<SubscriptionActor>(),
                settings: settings,
                messageExtractor: new SubscriptionMessageExtractor());
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            shardedSubscriptionActorRef.Tell(subscriptionMessage);
        }
    }
}
