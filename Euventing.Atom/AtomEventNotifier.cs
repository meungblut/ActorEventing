using System;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom
{
    public class AtomEventNotifier : IEventNotifier
    {
        private ActorSystem actorSystem;
        private readonly IActorRef atomFeedActor;

        public AtomEventNotifier(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var settings = ClusterShardingSettings.Create(actorSystem);

            atomFeedActor = ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomFeedActor",
                entityProps: Props.Create<AtomFeedActor>(),
                settings: settings,
                messageExtractor: new AtomFeedShardDataMessageExtractor());
        }

        public void Notify(SubscriptionMessage message, DomainEvent eventToNotify)
        {
        }

        public void Create(SubscriptionMessage message)
        {
            atomFeedActor.Tell(new FeedId(message.SubscriptionId.Id));
        }
    }
}
