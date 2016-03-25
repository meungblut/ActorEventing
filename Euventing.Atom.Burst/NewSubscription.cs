using Akka.Actor;

namespace Euventing.Atom.Burst
{
    public class NewSubscription
    {
        public NewSubscription(IActorRef subscriber)
        {
            SubscriptionQueue = subscriber;
        }

        public IActorRef SubscriptionQueue { get; }
    }
}