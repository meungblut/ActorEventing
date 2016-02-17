using Akka.Actor;
using Euventing.Atom.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom
{
    public class AtomDocumentRetriever
    {
        private ActorSystem actorSystem;

        public AtomDocumentRetriever(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public DocumentId GetHeadDocument(SubscriptionId subscriptionId)
        {
            return new DocumentId("");
        }
    }
}
