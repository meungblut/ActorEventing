using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Euventing.Core.Messages;

namespace Euventing.Core.Publishing
{
    public class DistributedPubSubEventPublisher : IEventPublisher
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef mediator;

        public DistributedPubSubEventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            mediator = DistributedPubSub.Get(actorSystem).Mediator;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            mediator.Tell(new Publish("publishedEventsTopic", thingToPublish));
        }
    }
}
