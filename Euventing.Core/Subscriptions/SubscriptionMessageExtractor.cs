using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster.Sharding;
using Akka.Persistence;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;
            
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id;
            
            return ((SubscriptionMessage)message).SubscriptionId.Id;
        }

        public object EntityMessage(object message)
        {
            if (message is SubscriptionMessage)
                return (SubscriptionMessage) message;

            if (message is SubscriptionQuery)
                return (SubscriptionQuery) message;

            return null;
        }

        public string ShardId(object message)
        {
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id.GetHashCode().ToString();

            return ((SubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();
        }
    }
}
