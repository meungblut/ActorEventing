using System;
using Akka.Persistence;

namespace Euventing.Atom.Document
{
    public class AtomFeedActor : PersistentActor
    {
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;

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
            {
                atomFeedId = ((FeedId) message);
                currentFeedHeadDocument = new DocumentId(Guid.NewGuid().ToString());
            }
            else if (message is GetHeadDocumentForFeedRequest)
                Sender.Tell(currentFeedHeadDocument, Self);
            else
                return false;

            return true;
        }

        public override string PersistenceId { get; }
    }
}
