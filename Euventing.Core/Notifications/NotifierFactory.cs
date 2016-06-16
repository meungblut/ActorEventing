using System;
using System.Collections.Generic;

namespace Eventing.Core.Notifications
{
    public class NotifierFactory
    {
        private readonly Dictionary<Type, IEventNotifier> notifiers; 
        public NotifierFactory()
        {
            var settings = new NotificationSettings();
            notifiers = settings.GetNotifiers();
        }

        public IEventNotifier GetNotifierFor(Type notificationChannelType)
        {
            return notifiers[notificationChannelType];
        }
    }
}
