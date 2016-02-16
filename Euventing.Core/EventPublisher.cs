using System;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Euventing.Core.Messages;

namespace Euventing.Core
{
    public class EventPublisher
    {
        private readonly ActorSystem actorSystem;
        private IActorRef mediator;

        public EventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            mediator = DistributedPubSub.Get(actorSystem).Mediator;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            Console.WriteLine("*********Publishing domain event");
            mediator.Tell(new Publish("publishedEventsTopic", thingToPublish));
        }
    }
}
