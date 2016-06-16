using Akka.Actor;
using Eventing.Atom.Document;
using Eventing.Atom.Document.Actors.ShardSupport.Document;
using Eventing.Core.Startup;

namespace Eventing.Atom
{
    public class AtomSubsystemConfiguration : ISubsytemConfiguration
    {
        private readonly IAtomDocumentSettings documentSettings;

        public AtomSubsystemConfiguration(IAtomDocumentSettings atomDocumentSettings)
        {
            documentSettings = atomDocumentSettings;
        }

        public void Configure(ActorSystem actorSystem)
        {
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            var atomFeedFactory = new ShardedAtomFeedFactory(actorSystem, atomDocumentFactory, documentSettings);
            var settings = new AtomNotificationSettings(atomFeedFactory);
        }
    }
}
