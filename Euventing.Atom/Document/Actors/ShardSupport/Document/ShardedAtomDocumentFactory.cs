using Akka.Actor;
using Akka.Cluster.Sharding;

namespace Eventing.Atom.Document.Actors.ShardSupport.Document
{
    public class ShardedAtomDocumentFactory : IAtomDocumentActorFactory
    {
        private readonly ActorSystem actorSystem;

        public ShardedAtomDocumentFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new AtomDocumentActor());

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