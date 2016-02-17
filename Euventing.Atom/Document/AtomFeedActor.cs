using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;

namespace Euventing.Atom.Document
{
    public class AtomFeedActor : PersistentActor
    {
        private FeedId atomFeedId;

        public AtomFeedActor()
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
        }
        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is FeedId)
                atomFeedId = ((FeedId) message);
            else
                return false;

            return true;
        }

        public override string PersistenceId { get; }
    }
}
