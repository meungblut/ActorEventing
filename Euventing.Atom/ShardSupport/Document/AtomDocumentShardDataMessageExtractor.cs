using Akka.Cluster.Sharding;
using Akka.Persistence;
using Euventing.Atom.Document;

namespace Euventing.Atom.ShardSupport.Document
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
                return ((GetAtomDocumentRequest) message).DocumentId.Id;

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message).AtomDocumentId.Id;

            return null;
        }

        public object EntityMessage(object message)
        {
            if (message is DocumentId)
                return ((DocumentId)message);

            if (message is GetAtomDocumentRequest)
                return ((GetAtomDocumentRequest)message);

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message);

            return null;
        }

        public string ShardId(object message)
        {
            if (message is DocumentId)
                return ((DocumentId)message).Id.GetHashCode().ToString();

            if (message is GetAtomDocumentRequest)
                return ((GetAtomDocumentRequest)message).DocumentId.Id.GetHashCode().ToString();

            if (message is EventWithDocumentIdNotificationMessage)
                return ((EventWithDocumentIdNotificationMessage)message).AtomDocumentId.Id.GetHashCode().ToString();

            return null;
        }
    }
}
