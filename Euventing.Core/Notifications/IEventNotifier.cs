using Euventing.Core.Messages;

namespace Euventing.Core.Notifications
{
    public interface IEventNotifier
    {
        void Notify(SubscriptionMessage message, DomainEvent eventToNotify);
    }
}
