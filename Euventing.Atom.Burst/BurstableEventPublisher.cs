using Akka.Actor;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class BurstableEventPublisher : IEventPublisher
    {
        private readonly ActorSystem actorSystem;
        private IActorRef actorRef;

        public BurstableEventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
            actorRef = actorSystem.ActorOf
                (Props.Create<AllLocalSubscriptionsActor>(), ActorLocations.LocalSubscriptionManagerLocation);
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            //var actor = actorSystem.ActorSelection(ActorLocations.LocalSubscriptionManagerLocation);
            actorRef.Tell(thingToPublish);
        }
    }
}
