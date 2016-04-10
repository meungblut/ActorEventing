namespace Euventing.Atom.Document
{
    public class CreateAtomDocumentCommand
    {
        public CreateAtomDocumentCommand(
            string title,
            string author,
            FeedId feedId,
            DocumentId currentHeadDocumentId,
            DocumentId previousHeadDocumentId,
            DocumentId nextHeadDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = currentHeadDocumentId;
            PreviousHeadDocumentId = previousHeadDocumentId;
            NextHeadDocumentId = nextHeadDocumentId;
        }

        public CreateAtomDocumentCommand(
            string title,
            string author,
            FeedId feedId,
            DocumentId documentId,
            DocumentId previousHeadDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            PreviousHeadDocumentId = previousHeadDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId PreviousHeadDocumentId { get; }

        public DocumentId NextHeadDocumentId { get; }
    }
}