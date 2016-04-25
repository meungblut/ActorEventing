namespace Euventing.Atom.Document
{
    public class PersistableAtomEntry
    {
        public string DocumentId { get; }
        public string FeedId { get; }
        public AtomEntry AtomEntry { get; }

        public PersistableAtomEntry(AtomEntry entry, string feedId, string documentId)
        {
            this.FeedId = feedId;
            this.DocumentId = documentId;
            this.AtomEntry = entry;
        }
    }
}
