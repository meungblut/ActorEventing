using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster.Sharding;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom.ShardSupport
{
    public class AtomFeedShardDataMessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            if (message is SaveSnapshotSuccess)
                return ((SaveSnapshotSuccess)message).Metadata.PersistenceId;

            if (message is FeedId)
                return ((FeedId) message).Id;

            return null;
        }

        public object EntityMessage(object message)
        {
            if (message is FeedId)
                return ((FeedId)message);

            return null;
        }

        public string ShardId(object message)
        {
            if (message is FeedId)
                return ((FeedId)message).Id.GetHashCode().ToString();

            return null;
        }
    }
}
