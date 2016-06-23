using Akka.Cluster.Sharding;
using Akka.Persistence;

namespace Eventing.Atom.Document.Actors.ShardSupport.Document
{
    public class AtomDocumentShardDataMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;

            if (message is DocumentId)
                return ((DocumentId)message).Id;

            if (message is GetAtomDocumentRequest)
                return ((GetAtomDocumentRequest)message).DocumentId.Id;

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message).AtomDocumentId.Id;

            if (message is CreateAtomDocumentCommand)
                return ((CreateAtomDocumentCommand)message).DocumentId.Id;

            if (message is NewDocumentAddedEvent)
                return ((NewDocumentAddedEvent)message).DocumentId.Id;

            throw new CouldNotRouteMessageToShardException(this, message);
        }

        public object EntityMessage(object message)
        {
            if (message is DocumentId)
                return ((DocumentId)message);

            if (message is GetAtomDocumentRequest)
                return ((GetAtomDocumentRequest)message);

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message);

            if (message is CreateAtomDocumentCommand)
                return ((CreateAtomDocumentCommand)message);

            if (message is NewDocumentAddedEvent)
                return ((NewDocumentAddedEvent)message);

            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message);

            throw new CouldNotRouteMessageToShardException(this, message);
        }

        public string ShardId(object message)
        {
            return "1";

            if (message is DocumentId)
                return ((DocumentId)message).Id.GetHashCode().ToString();

            if (message is GetAtomDocumentRequest)
                return ((GetAtomDocumentRequest)message).DocumentId.Id.GetHashCode().ToString();

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message).AtomDocumentId.Id.GetHashCode().ToString();

            if (message is CreateAtomDocumentCommand)
                return ((CreateAtomDocumentCommand)message).DocumentId.Id.GetHashCode().ToString();

            if (message is NewDocumentAddedEvent)
                return ((NewDocumentAddedEvent)message).DocumentId.Id.GetHashCode().ToString();

            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess) message).Metadata.PersistenceId.GetHashCode().ToString();

            throw new CouldNotRouteMessageToShardException(this, message);
        }
    }
}
