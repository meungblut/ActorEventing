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
        public AtomNotificationChannel AtomNotificationChannel { get; private set; }
        public UserId UserId { get; private set; }
        public SubscriptionId SubscriptionId { get; private set; }
        public IEventMatcher AllEventMatcher { get; private set; }


        public SubscriptionMessage(AtomNotificationChannel atomNotificationChannel, UserId userId, SubscriptionId subscriptionId, IEventMatcher allEventMatcher)
        {
            AtomNotificationChannel = atomNotificationChannel;
            UserId = userId;
            SubscriptionId = subscriptionId;
            AllEventMatcher = allEventMatcher;
        }
    }
}
