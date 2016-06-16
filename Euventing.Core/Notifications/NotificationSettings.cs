using System;
using System.Collections.Generic;

namespace Eventing.Core.Notifications
{
    public class NotificationSettings
    {
        private static readonly Dictionary<Type, IEventNotifier> Notifiers = new Dictionary<Type, IEventNotifier>();

        public Dictionary<Type, IEventNotifier> GetNotifiers()
        {
            return Notifiers;
        }

        protected void AddNotifierType(Type notificationChannelType, IEventNotifier notifier)
        {
            Notifiers.Add(notificationChannelType, notifier);
        }
    }
}
