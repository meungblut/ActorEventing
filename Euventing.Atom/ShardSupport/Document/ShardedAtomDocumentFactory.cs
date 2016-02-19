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

        public ShardedAtomDocumentFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            
            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomDocumentActor",
                entityProps: Props.Create<AtomDocumentActor>(),
                settings: settings,
                messageExtractor: new AtomDocumentShardDataMessageExtractor());
        }
        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomDocumentActor");
        }
    }
}