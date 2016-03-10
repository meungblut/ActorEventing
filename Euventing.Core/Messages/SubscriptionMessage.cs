using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core.EventMatching;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Euventing.Core.Messages
{
    public class SubscriptionMessage
    {
        public INotificationChannel NotificationChannel { get; private set; }
        public SubscriptionId SubscriptionId { get; private set; }

        [JsonIgnore]
        public IEventMatcher EventMatcher { get; private set; }

        public SubscriptionMessage(INotificationChannel notificationChannel, SubscriptionId subscriptionId, IEventMatcher eventMatcher)
        {
            NotificationChannel = notificationChannel;
            SubscriptionId = subscriptionId;
            EventMatcher = eventMatcher;
        }
    }
}
