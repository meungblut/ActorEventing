using System.Threading.Tasks;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Subscription
{
    public interface IAtomDocumentRepository
    {
        void Add(AtomEntry entry);

        Task<AtomDocument> GetDocument(string documentId);
    }
}