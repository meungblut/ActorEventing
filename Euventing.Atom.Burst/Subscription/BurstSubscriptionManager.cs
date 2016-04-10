using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom.Burst.Subscription
{
    public class BurstSubscriptionManager : ISubscriptionManager
    {
        public IActorRef BurstSubscriptionActorRef { get; private set; }

        public BurstSubscriptionManager(ActorSystem actorSystem, IAtomDocumentSettings atomDocumentSettings)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            var props = Props.Create(() => new BurstSubscriptionActor(atomDocumentSettings));

            var messageExtractor = new LoggingMessageExtractorDecorator(new BurstSubscriptionMessageExtractor(), actorSystem.Log);

            BurstSubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "BurstSubscriptionActor",
                entityProps: props,
                settings: settings,
                messageExtractor: messageExtractor);
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            BurstSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage)
        {
            BurstSubscriptionActorRef.Tell(subscriptionMessage);
        }

        public async Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            return await BurstSubscriptionActorRef.Ask<SubscriptionMessage>(query, TimeSpan.FromSeconds(3));
        }
    }
}
