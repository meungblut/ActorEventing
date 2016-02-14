using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster.Sharding;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            return (message as SubscriptionMessage)?.SubscriptionId.ToString();
        }

        public object EntityMessage(object message)
        {
            return (message as SubscriptionMessage);
        }

        public string ShardId(object message)
        {
            return (message as SubscriptionMessage)?.SubscriptionId.ToString();
        }
    }
}
