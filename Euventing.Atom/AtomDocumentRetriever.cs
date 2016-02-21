using System;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom
{
    public class AtomDocumentRetriever
    {
        private readonly AtomFeedShardedActorRefFactory factory;
        private ShardedAtomDocumentFactory shardedAtomDocumentFactory;

        public AtomDocumentRetriever(AtomFeedShardedActorRefFactory factory, ShardedAtomDocumentFactory builder)
        {
            shardedAtomDocumentFactory = builder;
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
            var documentId = await factory.GetActorRef().Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            var atomDocument = await shardedAtomDocumentFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(documentId));
            return atomDocument;
        }
        
        public void TestIfItIsTheMessageThatsScrewingMeUp(SubscriptionId subscriptionId)
        {
            factory.GetActorRef().Tell(new GetHeadDocumentForFeedRequest(subscriptionId));
        }
    }
}
