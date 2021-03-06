﻿using Akka.Cluster.Sharding;
using Akka.Persistence;
using Eventing.Core;

namespace Eventing.Atom.Document.Actors.ShardSupport.Feed
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

            if (message is AtomFeedCreationCommand)
                return ((AtomFeedCreationCommand) message).FeedId.Id;

            throw new CouldNotRouteMessageToShardException(this, message);
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
