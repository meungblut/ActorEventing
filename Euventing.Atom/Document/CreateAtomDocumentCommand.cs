namespace Euventing.Atom.Document
{
    public class CreateAtomDocumentCommand
    {
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

        public CreateAtomDocumentCommand(
            string title,
            string author,
            FeedId feedId,
            DocumentId documentId,
            DocumentId previousHeadDocumentId,
            DocumentId nextHeadDocumentId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            PreviousHeadDocumentId = previousHeadDocumentId;
            NextHeadDocumentId = nextHeadDocumentId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }

        public DocumentId DocumentId { get; }

        public DocumentId PreviousHeadDocumentId { get; }

        public DocumentId NextHeadDocumentId { get; }
    }
}