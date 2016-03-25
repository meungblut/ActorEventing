using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom.Burst.Subscritpion
{
    public class BurstSubscriptionManager : ISubscriptionManager
    {
        private readonly IActorRef burstSubscriptionActorRef;

        public BurstSubscriptionManager(ActorSystem actorSystem)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            var props = Props.Create(() => new BurstSubscriptionActor(new ShardedAtomFeedFactory(actorSystem, new ConfigurableAtomDocumentSettings(10))));

            burstSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "BurstSubscriptionActor",
                entityProps: props,
                settings: settings,
                messageExtractor: new BurstSubscriptionMessageExtractor());
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            burstSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage)
        {
            burstSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public async Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            return await burstSubscriptionActorRef.Ask<SubscriptionMessage>(query, TimeSpan.FromSeconds(3));
        }
    }
}
