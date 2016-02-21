using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport.Feed;

namespace Euventing.Atom.ShardSupport.Document
{
    public class ShardedAtomDocumentFactory : IAtomDocumentActorBuilder
    {
        private readonly ActorSystem actorSystem;

        private static bool initialised;
        private static object initialisationLock = new object();

        public ShardedAtomDocumentFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            lock (initialisationLock)
            {
                if (initialised)
                    return;

                var props = Props.Create(() => new AtomDocumentActor(new HardCodedAtomDocumentSettings()));

                var settings = ClusterShardingSettings.Create(actorSystem);
                ClusterSharding.Get(actorSystem).Start(
                    typeName: "AtomDocumentActor",
                    entityProps: props,
                    settings: settings,
                    messageExtractor: new AtomDocumentShardDataMessageExtractor());

                initialised = true;
            }

        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomDocumentActor");
        }
    }
}