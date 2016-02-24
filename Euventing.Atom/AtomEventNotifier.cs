using System;
using Akka.Actor;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;

namespace Euventing.Atom
{
    public class AtomEventNotifier : IEventNotifier
    {
        private readonly ShardedAtomFeedFactory factory;

        public AtomEventNotifier(ShardedAtomFeedFactory factory)
        {
            this.factory = factory;
        }

        public void Notify(SubscriptionMessage message, DomainEvent eventToNotify)
        {
            factory.GetActorRef().Tell(new EventWithSubscriptionNotificationMessage(message.SubscriptionId, eventToNotify));
        }

        public void Create(SubscriptionMessage message)
        {
            factory.GetActorRef().Tell(new AtomFeedCreationCommand("Title", "author", new FeedId(message.SubscriptionId.Id), null));
        }
    }
}
