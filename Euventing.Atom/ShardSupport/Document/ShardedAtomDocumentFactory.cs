using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Atom.ShardSupport.Feed;

namespace Euventing.Atom.ShardSupport.Document
{
    public class ShardedAtomDocumentFactory : IAtomDocumentActorBuilder
    {
        private readonly ActorSystem actorSystem;

        private static bool _initialised;
        private static readonly object InitialisationLock = new object();

        public ShardedAtomDocumentFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            lock (InitialisationLock)
            {
                if (_initialised)
                    return;

                var props = Props.Create(() => new AtomDocumentActor(new HardCodedAtomDocumentSettings()));

                var settings = ClusterShardingSettings.Create(actorSystem);
                ClusterSharding.Get(actorSystem).Start(
                    typeName: "AtomDocumentActor",
                    entityProps: props,
                    settings: settings,
                    messageExtractor: new AtomDocumentShardDataMessageExtractor());

                _initialised = true;
            }

        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomDocumentActor");
        }
    }
}