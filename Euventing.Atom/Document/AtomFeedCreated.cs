namespace Euventing.Atom.Document
{
    internal class AtomFeedCreated
    {
        public DocumentId DocumentId { get;  }
        public string FeedTitle { get;  }
        public string FeedAuthor { get;}
        public FeedId FeedId { get; }

        public AtomFeedCreated(DocumentId documentId, string feedTitle, string feedAuthor, FeedId feedId)
        {
            DocumentId = documentId;
            FeedTitle = feedTitle;
            FeedAuthor = feedAuthor;
            FeedId = feedId;
        }
    }
}