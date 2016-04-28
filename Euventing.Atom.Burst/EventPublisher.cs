using Akka.Actor;
using Euventing.Atom.Burst.Subscription;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IActorRef actorRef;

        public EventPublisher(ActorSystem actorSystem)
        {
            actorRef = actorSystem.ActorOf
                (Props.Create<EventQueueActor>(), ActorLocations.LocalQueueLocation);

            ActorLocations.LocalQueueActor = actorRef;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            actorRef.Tell(thingToPublish);
        }
    }
}
