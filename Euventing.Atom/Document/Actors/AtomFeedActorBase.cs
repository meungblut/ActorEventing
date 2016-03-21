using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core;

namespace Euventing.Atom.Document.Actors
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
