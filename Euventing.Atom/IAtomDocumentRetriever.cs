using System.Threading.Tasks;
using Eventing.Atom.Document;
using Eventing.Core.Messages;

namespace Eventing.Atom
{
    public interface IAtomDocumentRetriever
    {
        Task<AtomDocument> GetDocument(DocumentId documentId);
        Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId);
        Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId);
        Task<string> GetSerialisedDocument(DocumentId documentId);
        Task<string> GetSerialisedHeadDocument(SubscriptionId documentId);
    }
}