namespace Euventing.Atom.Document
{
    public class CreateAtomDocumentCommand
    {
        public CreateAtomDocumentCommand(
            string title,
            string author,
            FeedId feedId)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
        }

        public string Title { get; }

        public string Author { get; }

        public FeedId FeedId { get; }
        public DocumentId DocumentId { get; set; }
    }
}