using Akka.Cluster.Sharding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors.ShardSupport;
using Euventing.Core;

namespace Euventing.Atom.Burst.Feed
{
    public class FeedActorMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message).SubscriptionId.Id;

            if (message is GetHeadDocumentForFeedRequest)
                return ((GetHeadDocumentForFeedRequest)message).SubscriptionId.Id;

            if (message is AtomFeedCreationCommand)
                return ((AtomFeedCreationCommand)message).FeedId.Id;

            throw new CouldNotRouteMessageToShardException(this, message);
        }

        public object EntityMessage(object message)
        {
            if (message is FeedId)
                return ((FeedId)message);

            if (message is GetHeadDocumentIdForFeedRequest)
                return ((GetHeadDocumentIdForFeedRequest)message);

            if (message is GetHeadDocumentForFeedRequest)
                return ((GetHeadDocumentForFeedRequest)message);

            if (message is AtomFeedCreationCommand)
                return ((AtomFeedCreationCommand)message);

            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message);

            throw new CouldNotRouteMessageToShardException(this, message);
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

            if (message is AtomFeedCreationCommand)
                return ((AtomFeedCreationCommand)message).FeedId.Id.GetHashCode().ToString();

            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId.GetHashCode().ToString();

            throw new CouldNotRouteMessageToShardException(this, message);
        }
    }
}
