using Akka.Actor;
using Akka.Cluster.Sharding;
using Eventing.Atom.Document.Actors.ShardSupport.Document;

namespace Eventing.Atom.Document.Actors.ShardSupport.Feed
{
    public class AtomDocumentShardedActorRefFactory
    {
        private readonly ActorSystem actorSystem;

        public AtomDocumentShardedActorRefFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomFeedActor",
                entityProps: Props.Create<AtomDocumentActor>(),
                settings: settings,
                messageExtractor: new AtomDocumentShardDataMessageExtractor());
        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomFeedActor");
        }
}
}
