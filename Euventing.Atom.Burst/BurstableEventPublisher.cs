using Akka.Actor;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class BurstableEventPublisher : IEventPublisher
    {
        private readonly ActorSystem actorSystem;

        public BurstableEventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            var actor = actorSystem.ActorSelection(ActorLocations.LocalSubscriptionManagerLocation);
            actor.Tell(thingToPublish);
        }
    }
}
