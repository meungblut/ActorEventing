namespace Eventing.Atom.Document
{
    public class AtomDocumentSettings : IAtomDocumentSettings
    {
        public int NumberOfEventsPerDocument { get { return 10; } }
    }
}
