using Eventing.Core.EventMatching;
using Newtonsoft.Json;

namespace Eventing.Core.Messages
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
