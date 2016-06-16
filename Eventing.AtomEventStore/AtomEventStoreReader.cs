using System.Threading.Tasks;
using Eventing.Atom;
using Eventing.Atom.Document;
using Eventing.Core.Messages;

namespace Eventing.AtomEventStore
{
    public class AtomEventStoreReader : IAtomDocumentRetriever

    {
        public Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            throw new System.NotImplementedException();
        }

        public Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            throw new System.NotImplementedException();
        }
    }
}
