using System.Threading.Tasks;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst.Subscription
{
    public interface IAtomDocumentRepository
    {
        Task Add(string id, AtomEntry entry);

        Task<AtomDocument> GetDocument(string documentId);
    }
}