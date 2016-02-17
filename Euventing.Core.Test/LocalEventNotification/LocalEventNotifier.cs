using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;

namespace Euventing.Core.Test.LocalEventNotification
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
    }
}
