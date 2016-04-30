using System.Threading.Tasks;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Subscription
{
    public interface IAtomDocumentRepository
    {
        void Add(string id, AtomEntry entry);

        Task<AtomDocument> GetDocument(string documentId);
    }
}