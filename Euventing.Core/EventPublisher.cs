using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Euventing.Core.Messages;

namespace Euventing.Core
{
    public class EventPublisher
    {
        private readonly ActorSystem actorSystem;

        public EventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            var mediator = DistributedPubSub.Get(actorSystem).Mediator;
            mediator.Tell(new Publish("publishedEventsTopic", thingToPublish));
        }
    }
}
