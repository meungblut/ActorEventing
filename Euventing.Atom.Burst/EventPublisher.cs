using Akka.Actor;
using Euventing.Atom.Burst.Subscription;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
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
