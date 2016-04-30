using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Feed
{
    public class InMemoryAtomDocumentRepository : IAtomDocumentRepository
    {
        private static readonly Dictionary<string, List<AtomEntry>> entries = new Dictionary<string, List<AtomEntry>>();

        public void Add(string id, AtomEntry entry)
        {
            if (!entries.ContainsKey(id))
                entries.Add(id, new List<AtomEntry>());

            entries[id].Add(entry);
        }

        public Task<AtomDocument> GetDocument(string documentId)
        {
            List<AtomEntry> atomEntries = new List<AtomEntry>();

            if (entries.ContainsKey(documentId))
                atomEntries = entries[documentId];

            var document = new AtomDocument("", "", new FeedId(""), new DocumentId(documentId), new DocumentId(documentId), atomEntries);
            return Task.FromResult(document);
        }
    }
}