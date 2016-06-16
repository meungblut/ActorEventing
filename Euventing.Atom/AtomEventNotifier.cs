using Akka.Actor;
using Eventing.Atom.Document;
using Eventing.Atom.Document.Actors.ShardSupport.Document;
using Eventing.Core;
using Eventing.Core.Messages;
using Eventing.Core.Notifications;

namespace Eventing.Atom
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
            factory.GetActorRef().Tell(new AtomFeedCreationCommand("Title", "Author", new FeedId(message.SubscriptionId.Id), null));
        }
    }
}
