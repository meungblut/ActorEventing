using Akka.Cluster.Sharding;
using Akka.Persistence;
using Euventing.Atom.Document.Actors.ShardSupport;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscritpion
{
    public class BurstSubscriptionMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;
            
            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id;

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id;

            if (message is SubscriptionMessage)
               return ((SubscriptionMessage)message).SubscriptionId.Id;

            throw new CouldNotRouteMessageToShardException(null, message);
        }

        public object EntityMessage(object message)
        {
            if (message is SubscriptionMessage)
                return (SubscriptionMessage) message;

            if (message is SubscriptionQuery)
                return (SubscriptionQuery) message;

            if (message is SaveSnapshotSuccess)
                return (SaveSnapshotSuccess)message;

            if (message is DeleteSubscriptionMessage)
                return (DeleteSubscriptionMessage)message;

            throw new CouldNotRouteMessageToShardException(null, message);

        }

        public string ShardId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId.GetHashCode().ToString();

            if (message is SubscriptionQuery)
                return ((SubscriptionQuery)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is DeleteSubscriptionMessage)
                return ((DeleteSubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            if (message is SubscriptionMessage)
                return ((SubscriptionMessage)message).SubscriptionId.Id.GetHashCode().ToString();

            throw new CouldNotRouteMessageToShardException(null, message);
        }
    }
}
