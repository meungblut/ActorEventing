using System;
using System.Collections.Generic;

namespace Euventing.Atom.Document
{
    public class AtomDocument
    {
        public AtomDocument(string title, string author, FeedId feedId, DocumentId documentId, DocumentId nextArchiveDocumentId, List<AtomEntry> entries)
        {
            NextArchiveDocumentId = nextArchiveDocumentId;
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            Entries = entries;

            if (DocumentId.DocumentIndex > 0)
                PreviousArchiveDocumentId = DocumentId.Subtract(1);

            NextArchiveDocumentId = documentId.Add(1);
        }

        public string Title { get; }

        public DateTime Updated { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId NextArchiveDocumentId { get; }

        public DocumentId PreviousArchiveDocumentId { get; }

        public List<AtomEntry> Entries { get; }

        public string DocumentInformation { get; private set; }

        public void AddDocumentInformation(string information)
        {
            DocumentInformation += information + "\r\n";
        }
    }
}
