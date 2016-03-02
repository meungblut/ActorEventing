using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document.Actors.ShardSupport.Feed;

namespace Euventing.Atom.Document.Actors.ShardSupport.Document
{
    public class ShardedAtomFeedFactory
    {
        private readonly ActorSystem actorSystem;

        public ShardedAtomFeedFactory(ActorSystem actorSystem, ShardedAtomDocumentFactory factory)
        {
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new AtomFeedActor(factory, new HardCodedAtomDocumentSettings()));

            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomFeedActor",
                entityProps: props,
                settings: settings,
                messageExtractor: new AtomFeedShardDataMessageExtractor());
        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomFeedActor");
        }
    }
}
