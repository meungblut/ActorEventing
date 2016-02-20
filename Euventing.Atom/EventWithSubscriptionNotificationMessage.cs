using Euventing.Core.Messages;

namespace Euventing.Atom
{
    public class EventWithSubscriptionNotificationMessage
    {
        public SubscriptionId SubscriptionId { get; }
        public DomainEvent EventToNotify { get; }

        public EventWithSubscriptionNotificationMessage(SubscriptionId subscriptionId, DomainEvent eventToNotify)
        {
            SubscriptionId = subscriptionId;
            EventToNotify = eventToNotify;
        }
    }
}