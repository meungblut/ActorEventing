namespace Euventing.Atom.Document
{
    internal class AtomFeedCreated
    {
        public DocumentId DocumentId { get;  }
        public string FeedTitle { get;  }
        public string FeedAuthor { get;}

        public AtomFeedCreated(DocumentId documentId, string feedTitle, string feedAuthor)
        {
            DocumentId = documentId;
            FeedTitle = feedTitle;
            FeedAuthor = feedAuthor;
        }
    }
}