using System.Threading;
using Eventing.Core.Messages;
using Eventing.Core.Notifications;

namespace Eventing.Core.Test.LocalEventNotification
{
    public class LocalEventNotifier : IEventNotifier
    {
        public static DomainEvent EventNotifiedWith { get; private set; }
        public static SubscriptionMessage SubscriptionNotifiedWith { get; private set; }

        public static ManualResetEvent EventReceived { get; private set; }

        static LocalEventNotifier()
        {
            EventReceived = new ManualResetEvent(false);
        }

        public void Notify(SubscriptionMessage message, DomainEvent eventToNotify)
        {
            EventNotifiedWith = eventToNotify;
            EventReceived.Set();
        }

        public void Create(SubscriptionMessage message)
        {
        }
    }
}
