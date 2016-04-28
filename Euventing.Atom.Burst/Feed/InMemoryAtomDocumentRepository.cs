using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Feed
{
    public class InMemoryAtomDocumentRepository : IAtomDocumentRepository
    {
        private static readonly List<AtomEntry> entries = new List<AtomEntry>();

        public void Add(AtomEntry entry)
        {
            entries.Add(entry);
        }

        public void Add(IEnumerable<AtomEntry> events)
        {
            entries.AddRange(events);
        }

        public Task<AtomDocument> GetDocument(string documentId)
        {
            var atomEntries = entries.Where(x => x.DocumentId.Id == documentId).ToList();
            var document = new AtomDocument("", "", new FeedId(""), new DocumentId(documentId), new DocumentId(documentId), new DocumentId(documentId), atomEntries);
            return Task.FromResult(document);
        }
    }
}