namespace Euventing.Atom.Document
{
    public class AtomDocumentCreatedEvent
    {
        public AtomDocumentCreatedEvent(
            string title,
            string author,
            string feedId,
            DocumentId documentId,
            string earlierEventsDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public string FeedId { get; }

        public DocumentId DocumentId { get; }

        public string EarlierEventsDocumentId { get; }
    }
}