﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core.Startup;

namespace Euventing.Atom
{
    public class AtomSubsystemConfiguration : ISubsytemConfiguration
    {
        public void Configure(ActorSystem actorSystem)
        {
            ShardedAtomFeedFactory atomFeedFactory = new ShardedAtomFeedFactory(actorSystem);
            ShardedAtomDocumentFactory atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);

            var settings = new AtomNotificationSettings(atomFeedFactory);
        }
    }
}
