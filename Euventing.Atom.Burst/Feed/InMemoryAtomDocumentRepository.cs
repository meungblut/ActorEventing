using System.Collections.Generic;
using System.Linq;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Feed
{
    internal class InMemoryAtomDocumentRepository : IAtomDocumentRepository
    {
        private readonly List<AtomEntry> entries
            = new List<AtomEntry>();

        public void Add(AtomEntry entry)
        {
            entries.Add(entry);
        }

        public void Add(IEnumerable<AtomEntry> events)
        {
            entries.AddRange(events);
        }

        public AtomDocument GetDocument(string documentId)
        {
            var atomEntries = entries.Where(x => x.DocumentId.Id == documentId).ToList();
            return new AtomDocument("", "", new FeedId(""), new DocumentId(documentId), new DocumentId(documentId), new DocumentId(documentId), atomEntries);
        }
    }
}