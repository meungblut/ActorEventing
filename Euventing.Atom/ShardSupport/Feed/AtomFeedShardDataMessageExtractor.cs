using Akka.Cluster.Sharding;
using Akka.Persistence;
using Euventing.Atom.Document;

namespace Euventing.Atom.ShardSupport.Feed
{
    public class AtomFeedShardDataMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;

            if (message is FeedId)
                return ((FeedId)message).Id;

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message).SubscriptionId.Id;

            if (message is GetHeadDocumentForFeedRequest)
                return ((GetHeadDocumentForFeedRequest)message).SubscriptionId.Id;

            if (message is EventWithSubscriptionNotificationMessage)
                return ((EventWithSubscriptionNotificationMessage) message).SubscriptionId.Id;

            return null;
        }

        public object EntityMessage(object message)
        {
            if (message is FeedId)
                return ((FeedId)message);

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest) message);

            if (message is GetHeadDocumentForFeedRequest)
                return ((GetHeadDocumentForFeedRequest)message);

            if (message is EventWithSubscriptionNotificationMessage)
                return ((EventWithSubscriptionNotificationMessage)message);

            return null;
        }

        public string ShardId(object message)
        {
            if (message is FeedId)
                return ((FeedId)message).Id.GetHashCode().ToString();

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is GetHeadDocumentForFeedRequest)
                return ((GetHeadDocumentForFeedRequest)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is EventWithSubscriptionNotificationMessage)
                return ((EventWithSubscriptionNotificationMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            return null;
        }
    }
}
