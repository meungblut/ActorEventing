﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocument
    {
        public AtomDocument(string title, string author, string feedId, DocumentId documentId, string earlierEventsDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
        }

        public string Title { get; }

        public DateTime Updated { get; }

        public string Author { get; }

        public string FeedId { get; }

        public DocumentId DocumentId { get; }

        public string LaterEventsDocumentId { get; }

        public string EarlierEventsDocumentId { get; }

        public List<AtomEntry> Entries { get; }
    }
}
