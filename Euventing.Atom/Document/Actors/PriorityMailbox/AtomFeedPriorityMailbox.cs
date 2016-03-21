using Akka.Dispatch;

namespace Euventing.Atom.Document.Actors.PriorityMailbox
{
    public class AtomFeedPriorityMailbox : UnboundedPriorityMailbox
    {
        protected override int PriorityGenerator(object message)
        {
            if (message is GetHeadDocumentForFeedRequest || message is GetHeadDocumentIdForFeedRequest)
            {
                return 0;
            }

            return 2;
        }
    }
}