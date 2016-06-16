using Akka.Actor;
using Akka.Cluster.Sharding;
using Eventing.Atom.Document.Actors.ShardSupport.Feed;

namespace Eventing.Atom.Document.Actors.ShardSupport.Document
{
    public class ShardedAtomFeedFactory
    {
        private readonly ActorSystem actorSystem;
        private IAtomDocumentSettings atomDocumentSettings;

        public ShardedAtomFeedFactory(ActorSystem actorSystem, IAtomDocumentActorFactory factory, IAtomDocumentSettings atomDocumentSettings)
        {
            this.atomDocumentSettings = atomDocumentSettings;
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new AtomFeedActor(factory, atomDocumentSettings));

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
