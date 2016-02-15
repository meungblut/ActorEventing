using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Core.Messages
{
    public class SubscriptionQuery
    {
        public UserId UserId { get; private set; }
        public SubscriptionId SubscriptionId { get; private set; }


        public SubscriptionQuery(UserId userId, SubscriptionId subscriptionId)
        {
            UserId = userId;
            SubscriptionId = subscriptionId;
        }
    }
}
