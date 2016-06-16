using Eventing.Core;

namespace Eventing.Atom.Document.Actors
{
    public abstract class AtomFeedActorBase : PersistentActorBase
    {
        protected FeedId AtomFeedId;
        protected DocumentId CurrentFeedHeadDocument;
        protected DocumentId LastHeadDocument;
        protected string FeedTitle;
        protected string FeedAuthor;
    }
}
