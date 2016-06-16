namespace Eventing.Atom.Document.Actors
{
    public class AtomFeedState
    {
        public AtomFeedState(
            FeedId atomFeedId, 
            DocumentId currentFeedHeadDocument, 
            DocumentId lastHeadDocument, 
            string feedTitle,
            string feedAuthor, 
            int numberOfEventsInCurrentHeadDocument, 
            int currentHeadDocumentIndex)
        {
            AtomFeedId = atomFeedId;
            CurrentFeedHeadDocument = currentFeedHeadDocument;
            LastHeadDocument = lastHeadDocument;
            FeedTitle = feedTitle;
            FeedAuthor = feedAuthor;
            NumberOfEventsInCurrentHeadDocument = numberOfEventsInCurrentHeadDocument;
            CurrentHeadDocumentIndex = currentHeadDocumentIndex;
        }

        public FeedId AtomFeedId { get; private set; }
        public DocumentId CurrentFeedHeadDocument { get; private set; }
        public DocumentId LastHeadDocument { get; private set; }
        public string FeedTitle { get; private set; }
        public string FeedAuthor { get; private set; }
        public int NumberOfEventsInCurrentHeadDocument { get; private set; }
        public int CurrentHeadDocumentIndex { get; private set; }
    }
}
