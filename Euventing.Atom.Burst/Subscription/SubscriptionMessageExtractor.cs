using Akka.Cluster.Sharding;
using Akka.Persistence;
using Eventing.Core.Messages;

namespace Eventing.Atom.Burst.Subscription
{
    public class SubscriptionMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
            {
                return ((SaveSnapshotSuccess) message).Metadata.PersistenceId;
            }

            if (message is GetDocumentFromFeedRequest)
                return ((GetDocumentFromFeedRequest)message).SubscriptionId.Id;
            
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id;

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id;

            if (message is SubscriptionMessage)
                return ((SubscriptionMessage)message).SubscriptionId.Id;

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message).SubscriptionId.Id;

            return string.Empty;
            //throw new CouldNotRouteMessageToShardException(null, message);
        }

        public object EntityMessage(object message)
        {
            if (message is GetDocumentFromFeedRequest)
                return ((GetDocumentFromFeedRequest)message);

            if (message is SubscriptionMessage)
                return (SubscriptionMessage)message;

            if (message is SubscriptionQuery)
                return (SubscriptionQuery)message;

            if (message is SaveSnapshotSuccess)
                return (SaveSnapshotSuccess)message;

            if (message is DeleteSubscriptionMessage)
                return (DeleteSubscriptionMessage)message;

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message);

            return null;
            //throw new CouldNotRouteMessageToShardException(null, message);

        }

        public string ShardId(object message)
        {
            return "1";
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId.GetHashCode().ToString();

            if (message is GetDocumentFromFeedRequest)
                return ((GetDocumentFromFeedRequest)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is SubscriptionMessage)
                return ((SubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message).SubscriptionId.Id.GetHashCode().ToString();

            return null;
            //throw new CouldNotRouteMessageToShardException(null, message);
        }
    }
}
