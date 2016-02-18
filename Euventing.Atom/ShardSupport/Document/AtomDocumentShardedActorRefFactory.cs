using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.ShardSupport.Feed;

namespace Euventing.Atom.ShardSupport.Document
{
    public class AtomFeedShardedActorRefFactory
    {
        private readonly ActorSystem actorSystem;

        public AtomFeedShardedActorRefFactory(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;

            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "AtomFeedActor",
                entityProps: Props.Create<AtomFeedActor>(),
                settings: settings,
                messageExtractor: new AtomFeedShardDataMessageExtractor());
        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("AtomFeedActor");
        }
}
}
