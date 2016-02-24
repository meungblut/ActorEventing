using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Atom.ShardSupport.Feed;

namespace Euventing.Atom.ShardSupport.Document
{
    public class ShardedAtomFeedFactory
    {
        private readonly ActorSystem actorSystem;

        public ShardedAtomFeedFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new AtomFeedActor(new ShardedAtomDocumentFactory(actorSystem), new HardCodedAtomDocumentSettings()));

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
