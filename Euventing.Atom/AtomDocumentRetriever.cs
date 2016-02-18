using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport;
using Euventing.Core.Messages;

namespace Euventing.Atom
{
    public class AtomDocumentRetriever
    {
        private readonly AtomFeedShardedActorRefFactory factory;

        public AtomDocumentRetriever(AtomFeedShardedActorRefFactory factory)
        {
            this.factory = factory;
        }

        public async Task<DocumentId> GetHeadDocument(SubscriptionId subscriptionId)
        {
            var feedId = await factory.GetActorRef().Ask<DocumentId>(new GetHeadDocumentForFeedRequest(subscriptionId));
            return feedId;
        }
    }
}
