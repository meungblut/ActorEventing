using Akka.Cluster.Sharding;
using Akka.Persistence;
using Eventing.Core.Messages;

namespace Eventing.Core.Subscriptions
{
    public class SubscriptionMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;
            
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id;

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id;

            return ((SubscriptionMessage)message).SubscriptionId.Id;
        }

        public object EntityMessage(object message)
        {
            if (message is SubscriptionMessage)
                return (SubscriptionMessage) message;

            if (message is SubscriptionQuery)
                return (SubscriptionQuery) message;

            if (message is DeleteSubscriptionMessage)
                return (DeleteSubscriptionMessage)message;

            return null;
        }

        public string ShardId(object message)
        {
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            return ((SubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();
        }
    }
}
