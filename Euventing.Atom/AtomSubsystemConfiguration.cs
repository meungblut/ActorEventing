using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.Document.Actors.ShardSupport.Document;
using Euventing.Core.Startup;

namespace Euventing.Atom
{
    public class AtomSubsystemConfiguration : ISubsytemConfiguration
    {
        public void Configure(ActorSystem actorSystem)
        {
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            var atomFeedFactory = new ShardedAtomFeedFactory(actorSystem, atomDocumentFactory);
            var settings = new AtomNotificationSettings(atomFeedFactory);
        }
    }
}
