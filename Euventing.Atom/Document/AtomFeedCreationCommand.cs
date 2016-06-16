namespace Eventing.Atom.Document
{
    public class AtomFeedCreationCommand
    {
        public AtomFeedCreationCommand(
            string title,
            string author,
            FeedId feedId,
            DocumentId earlierEventsDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId EarlierEventsDocumentId { get; }
    }
}