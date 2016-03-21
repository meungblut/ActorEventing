using Akka.Dispatch;

namespace Euventing.Atom.Document.Actors.PriorityMailbox
{
    public class AtomDocumentPriorityMailbox : UnboundedPriorityMailbox
    {
        protected override int PriorityGenerator(object message)
        {
            var docRequest = message as GetAtomDocumentRequest;

            if (docRequest != null)
            {
                return 0;
            }

            return 2;
        }

    }
}
