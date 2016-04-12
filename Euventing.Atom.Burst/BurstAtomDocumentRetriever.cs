using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using Euventing.Atom.Serialization;

namespace Euventing.Atom.Burst
{
    public class BurstAtomDocumentRetriever : IAtomDocumentRetriever
    {
        private readonly BurstSubscriptionManager burstManager;
        private readonly AtomDocumentSerialiser atomDocumentSerialiser;
        private ILoggingAdapter _adapter;

        public BurstAtomDocumentRetriever(BurstSubscriptionManager burstManager, ILoggingAdapter adapter)
        {
            _adapter = adapter;
            atomDocumentSerialiser = new AtomDocumentSerialiser();
            this.burstManager = burstManager;
        }

        public async Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            return null;
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            _adapter.Info($"requesting head document for id {subscriptionId.Id}");
            var document = await burstManager.BurstSubscriptionActorRef.Ask<AtomDocument>(new GetHeadDocumentForFeedRequest(subscriptionId));
            return document;
        }

        public async Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            var splitId = documentId.Id.Split(new[] {'_'});
            var subscriptionId = splitId[0];
            _adapter.Info($"requesting document {documentId.Id} from feed {subscriptionId}");

            var document = await burstManager.BurstSubscriptionActorRef.Ask<AtomDocument>(
                new GetDocumentFromFeedRequest(new SubscriptionId(subscriptionId), documentId));

            _adapter.Info($"retrieved document {atomDocumentSerialiser.Serialise(document, "http://localhost:3600/events/atom/document/")}");
            
            return document;
        }

        public Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            var atomDocument = await GetHeadDocument(documentId);
            return atomDocumentSerialiser.Serialise(atomDocument, "http://localhost:3600/events/atom/document/");
        }
    }

    public class GetDocumentFromFeedRequest
    {
        public SubscriptionId SubscriptionId { get; }
        public DocumentId DocumentId { get; }

        public GetDocumentFromFeedRequest(SubscriptionId subscriptionId, DocumentId documentId)
        {
            SubscriptionId = subscriptionId;
            DocumentId = documentId;
        }
    }
}
