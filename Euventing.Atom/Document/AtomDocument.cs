﻿using System;
using System.Collections.Generic;

namespace Euventing.Atom.Document
{
    public class AtomDocument
    {
        public AtomDocument(string title, string author, FeedId feedId, DocumentId documentId, DocumentId earlierEventsDocumentId, DocumentId laterEventsDocumentId, List<AtomEntry> entries)
        {
            LaterEventsDocumentId = laterEventsDocumentId;
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

        public string DocumentInformation { get; private set; }

        public void AddDocumentInformation(string information)
        {
            DocumentInformation += information + "\r\n";
        }
    }
}
