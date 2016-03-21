using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public class ShardedSubscriptionManager : ISubscriptionManager
    {
        private readonly IActorRef shardedSubscriptionActorRef;

        public ShardedSubscriptionManager(ActorSystem actorSystem)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            shardedSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "SubscriptionActor",
                entityProps: Props.Create<SubscriptionActor>(),
                settings: settings,
                messageExtractor: new SubscriptionMessageExtractor());
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            shardedSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage)
        {
            shardedSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public async Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            return await shardedSubscriptionActorRef.Ask<SubscriptionMessage>(query, TimeSpan.FromSeconds(3));
        }
    }
}
