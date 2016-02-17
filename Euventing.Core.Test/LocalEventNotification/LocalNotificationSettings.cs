using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core.Notifications;

namespace Euventing.Core.Test.LocalEventNotification
{
    public class LocalNotificationSettings : NotificationSettings
    {
        public LocalNotificationSettings()
        {
            this.AddNotifierType(typeof(LocalEventNotificationChannel), new LocalEventNotifier());
        }
    }
}
