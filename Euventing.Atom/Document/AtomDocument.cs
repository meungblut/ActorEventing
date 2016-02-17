using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocument
    {
        public string Title { get; }

        public DateTime Updated { get; }

        public string Author { get; }

        public string FeedId { get; }

        public string DocumentId { get; }

        public string LaterEventsDocumentId { get; }

        public string EarlierEventsDocumentId { get; }

        public List<AtomEntry> Entries { get; }
    }
}
