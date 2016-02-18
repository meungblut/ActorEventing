namespace Euventing.Atom.Document
{
    public class CreateAtomDocumentCommand
    {
        public CreateAtomDocumentCommand(
            string title, 
            string author, 
            FeedId feedId,
            DocumentId documentId, 
            DocumentId earlierEventsDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId EarlierEventsDocumentId { get; }
    }
}