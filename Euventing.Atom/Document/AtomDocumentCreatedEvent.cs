namespace Euventing.Atom.Document
{
    public class AtomDocumentCreatedEvent
    {
        public AtomDocumentCreatedEvent(
            string title,
            string author,
            DocumentId documentId)
        {
            Title = title;
            Author = author;
            DocumentId = documentId;
        }

        public AtomDocumentCreatedEvent(
            string title,
            string author,
            FeedId feedId,
            DocumentId documentId,
            DocumentId earlierEventsDocumentId,
            DocumentId nextEventsDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
            NextEventsDocumentId = nextEventsDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId EarlierEventsDocumentId { get; }
        public DocumentId NextEventsDocumentId { get; }
    }
}