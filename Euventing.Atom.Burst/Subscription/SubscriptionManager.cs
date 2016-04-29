using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom.Burst.Subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {
        public IActorRef SubscriptionActorRef { get; private set; }

        public SubscriptionManager(ActorSystem actorSystem, IAtomDocumentSettings atomDocumentSettings)
        {
            var settings = ClusterShardingSettings.Create(actorSystem);

            var props = Props.Create(() => new SubscriptionActor(atomDocumentSettings));

            var messageExtractor = new SubscriptionMessageExtractor();

            SubscriptionActorRef = ClusterSharding.Get(actorSystem).Start(
                typeName: "SubscriptionActor",
                entityProps: props,
                settings: settings,
                messageExtractor: messageExtractor);
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
            SubscriptionActorRef.Tell(subscriptionMessage);
        }

        public void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage)
        {
            SubscriptionActorRef.Tell(subscriptionMessage);
        }

        public async Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            return await SubscriptionActorRef.Ask<SubscriptionMessage>(query, TimeSpan.FromSeconds(3));
        }
    }
}
