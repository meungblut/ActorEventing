using Eventing.Core.Notifications;

namespace Eventing.Core.Test.LocalEventNotification
{
    public class LocalNotificationSettings : NotificationSettings
    {
        public LocalNotificationSettings()
        {
            this.AddNotifierType(typeof(LocalEventNotificationChannel), new LocalEventNotifier());
        }
    }
}
