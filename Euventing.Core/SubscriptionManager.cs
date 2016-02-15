using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Util.Internal;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Core
{
    public class SubscriptionManager
    {
        private readonly IActorRef shardedSubscriptionActorRef;

        public SubscriptionManager(ActorSystem actorSystem)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            shardedSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "SubscriptionActor",
                entityProps: Props.Create<SubscriptionActor>(),
                settings: settings,
                messageExtractor: new SubscriptionMessageExtractor());

            Console.WriteLine(shardedSubscriptionActorRef.Path);
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            shardedSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public async Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            return await shardedSubscriptionActorRef.Ask<SubscriptionMessage>(query, TimeSpan.FromSeconds(3));
        }
    }
}
