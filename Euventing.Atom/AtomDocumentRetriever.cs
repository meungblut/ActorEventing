using System.Threading.Tasks;
using Akka.Actor;
using Eventing.Atom.Document;
using Eventing.Atom.Document.Actors.ShardSupport.Document;
using Eventing.Atom.Serialization;
using Eventing.Core.Messages;

namespace Eventing.Atom
{
    public class AtomDocumentRetriever : IAtomDocumentRetriever
    {
        private readonly ShardedAtomFeedFactory factory;
        private readonly ShardedAtomDocumentFactory shardedAtomDocumentFactory;
        private readonly AtomDocumentSerialiser atomDocumentSerialiser;

        public AtomDocumentRetriever(ShardedAtomFeedFactory factory, ShardedAtomDocumentFactory builder)
        {
            shardedAtomDocumentFactory = builder;
            this.factory = factory;
            atomDocumentSerialiser = new AtomDocumentSerialiser();
        }

        public async Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            var documentId = await factory.GetActorRef().Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            return documentId;
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            var documentId = await factory.GetActorRef().Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            var atomDocument = await shardedAtomDocumentFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(documentId));
            return atomDocument;
        }

        public async Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            var atomDocument = await shardedAtomDocumentFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(documentId));
            return atomDocument;
        }

        public async Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            var atomDocument = await shardedAtomDocumentFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(documentId));
            return atomDocumentSerialiser.Serialise(atomDocument, "http://matt.com");
        }

        public async Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            var atomDocument = await GetHeadDocument(documentId);
            return atomDocumentSerialiser.Serialise(atomDocument, "http://matt.com");
        }
    }
}
