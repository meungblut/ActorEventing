using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Atom.ShardSupport.Feed;
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

        public async Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            var documentId = await factory.GetActorRef().Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            return documentId;
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            Console.WriteLine("Getting document with subscription " + subscriptionId.Id);
            var document = await factory.GetActorRef().Ask<AtomDocument>(new GetHeadDocumentForFeedRequest(subscriptionId));
            return document;
        }
    }
}
