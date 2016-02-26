using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core.EventMatching;

namespace Euventing.Core.Messages
{
    public class SubscriptionMessage
    {
        public INotificationChannel NotificationChannel { get; private set; }
        public SubscriptionId SubscriptionId { get; private set; }
        public IEventMatcher AllEventMatcher { get; private set; }

        public SubscriptionMessage(INotificationChannel notificationChannel, SubscriptionId subscriptionId, IEventMatcher allEventMatcher)
        {
            NotificationChannel = notificationChannel;
            SubscriptionId = subscriptionId;
            AllEventMatcher = allEventMatcher;
        }
    }
}
