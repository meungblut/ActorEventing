using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocument
    {
        public AtomDocument(string title, string author, FeedId feedId, DocumentId documentId, DocumentId earlierEventsDocumentId, List<AtomEntry> entries)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
            Entries = entries;
        }

        public string Title { get; }

        public DateTime Updated { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId LaterEventsDocumentId { get; }

        public DocumentId EarlierEventsDocumentId { get; }

        public List<AtomEntry> Entries { get; }
    }
}
