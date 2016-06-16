using Eventing.Core.Messages;

namespace Eventing.Core.Notifications
{
    public interface IEventNotifier
    {
        void Notify(SubscriptionMessage message, DomainEvent eventToNotify);

        void Create(SubscriptionMessage message);
    }
}
