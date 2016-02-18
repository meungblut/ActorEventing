using System;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Atom.ShardSupport.Feed;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom
{
    public class AtomEventNotifier : IEventNotifier
    {
        private readonly AtomFeedShardedActorRefFactory factory;

        public AtomEventNotifier(AtomFeedShardedActorRefFactory factory)
        {
            this.factory = factory;
        }

        public void Notify(SubscriptionMessage message, DomainEvent eventToNotify)
        {
        }

        public void Create(SubscriptionMessage message)
        {
            factory.GetActorRef().Tell(new FeedId(message.SubscriptionId.Id));
        }
    }
}
