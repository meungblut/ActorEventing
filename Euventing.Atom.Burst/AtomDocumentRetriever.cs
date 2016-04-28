using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Serialization;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class AtomDocumentRetriever : IAtomDocumentRetriever
    {
        private readonly AtomDocumentSerialiser atomDocumentSerialiser;
        private readonly ILoggingAdapter _adapter;
        private readonly SubscriptionManager burstManager;
        private readonly IAtomDocumentRepository _documentRepository;

        public AtomDocumentRetriever(SubscriptionManager burstManager, ILoggingAdapter adapter, IAtomDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
            this.burstManager = burstManager;
            _adapter = adapter;
            atomDocumentSerialiser = new AtomDocumentSerialiser();
        }

        public async Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            _adapter.Info($"requesting head document for id {subscriptionId.Id}");
            var document = await burstManager.SubscriptionActorRef.Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            return document;
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            _adapter.Info($"requesting head document for id {subscriptionId.Id}");
            var documentId = await burstManager.SubscriptionActorRef.Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            var document = await _documentRepository.GetDocument(documentId);
            return document;
        }

        public async Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            _adapter.Info($"requesting document {documentId.Id} from feed {documentId.FeedId}");

            return await _documentRepository.GetDocument(documentId);
        }

        public async Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            return atomDocumentSerialiser.Serialise(await GetDocument(documentId), "http://localhost:3600/events/atom/document/");
        }

        public async Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            var atomDocument = await GetHeadDocument(documentId);
            return atomDocumentSerialiser.Serialise(atomDocument, "http://localhost:3600/events/atom/document/");
        }
    }
}