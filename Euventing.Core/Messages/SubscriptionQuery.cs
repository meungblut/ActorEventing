using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Core.Messages
{
    public class SubscriptionQuery
    {
        public SubscriptionId SubscriptionId { get; private set; }


        public SubscriptionQuery(SubscriptionId subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
