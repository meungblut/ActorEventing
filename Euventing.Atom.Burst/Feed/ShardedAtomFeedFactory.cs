using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;

namespace Euventing.Atom.Burst.Feed
{
    public class ShardedAtomFeedFactory
    {
        private readonly ActorSystem actorSystem;
        private readonly IAtomDocumentSettings atomDocumentSettings;

        public ShardedAtomFeedFactory(ActorSystem actorSystem, IAtomDocumentSettings atomDocumentSettings)
        {
            this.atomDocumentSettings = atomDocumentSettings;
            this.actorSystem = actorSystem;

            var props = Props.Create(() => new FeedActor(this.atomDocumentSettings));

            var settings = ClusterShardingSettings.Create(actorSystem);
            ClusterSharding.Get(actorSystem).Start(
                typeName: "FeedActor",
                entityProps: props,
                settings: settings,
                messageExtractor: new FeedActorMessageExtractor());
        }

        public IActorRef GetActorRef()
        {
            return ClusterSharding.Get(actorSystem).ShardRegion("FeedActor");
        }
    }
}
