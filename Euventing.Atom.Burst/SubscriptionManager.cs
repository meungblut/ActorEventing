using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document.Actors.ShardSupport.Feed;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom.Burst
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IActorRef shardedSubscriptionActorRef;

        public SubscriptionManager(ActorSystem actorSystem)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            shardedSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "FeedActor",
                entityProps: Props.Create<FeedActor>(),
                settings: settings,
                messageExtractor: new AtomFeedShardDataMessageExtractor());
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
