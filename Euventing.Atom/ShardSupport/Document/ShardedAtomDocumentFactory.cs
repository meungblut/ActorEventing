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
    public class ShardedAtomDocumentFactory : IAtomDocumentActorFactory
    {
        private readonly ActorSystem actorSystem;

        public ShardedAtomDocumentFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new AtomDocumentActor(new HardCodedAtomDocumentSettings()));

            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomDocumentActor",
                entityProps: props,
                settings: settings,
                messageExtractor: new AtomDocumentShardDataMessageExtractor());
        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomDocumentActor");
        }
    }
}