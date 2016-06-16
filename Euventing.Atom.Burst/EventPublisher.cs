using Akka.Actor;
using Eventing.Atom.Burst.Subscription;
using Eventing.Core;
using Eventing.Core.Messages;

namespace Eventing.Atom.Burst
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef actorRef;

        public EventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            actorRef = actorSystem.ActorOf
                (Props.Create<EventQueueActor>(), ActorLocations.LocalQueueLocation);

            ActorLocations.LocalQueueActor = actorRef;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            actorRef.Tell(thingToPublish);
            this.actorSystem.EventStream.Publish(thingToPublish);
        }
    }
}
