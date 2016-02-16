using System;
using System.Collections.Generic;

namespace Euventing.Core.Notifications
{
    public class NotificationSettings
    {
        private static Dictionary<Type, IEventNotifier> notifiers = new Dictionary<Type, IEventNotifier>();

        public Dictionary<Type, IEventNotifier> GetNotifiers()
        {
            return notifiers;
        } 
    }
}
