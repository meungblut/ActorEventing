using System.Threading.Tasks;
using Euventing.Atom.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom
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