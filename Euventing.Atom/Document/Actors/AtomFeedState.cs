using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document.Actors
{
    public class AtomFeedState
    {
        public AtomFeedState(FeedId atomFeedId, DocumentId currentFeedHeadDocument, DocumentId lastHeadDocument, string feedTitle, string feedAuthor, int numberOfEventsInCurrentHeadDocument)
        {
            AtomFeedId = atomFeedId;
            CurrentFeedHeadDocument = currentFeedHeadDocument;
            LastHeadDocument = lastHeadDocument;
            FeedTitle = feedTitle;
            FeedAuthor = feedAuthor;
            NumberOfEventsInCurrentHeadDocument = numberOfEventsInCurrentHeadDocument;
        }

        public FeedId AtomFeedId { get; private set; }
        public DocumentId CurrentFeedHeadDocument { get; private set; }
        public DocumentId LastHeadDocument { get; private set; }
        public string FeedTitle { get; private set; }
        public string FeedAuthor { get; private set; }
        public int NumberOfEventsInCurrentHeadDocument { get; private set; }
    }
}
