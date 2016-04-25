using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Subscription
{
    public interface IAtomDocumentRepository
    {
        void Add(PersistableAtomEntry entry);
    }
}