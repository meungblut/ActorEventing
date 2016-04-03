using Akka.Actor;

namespace Euventing.Atom.Burst
{
    public class NewLocalSubscriptionCreated
    {
        public NewLocalSubscriptionCreated(IActorRef subscriber)
        {
            SubscriptionQueue = subscriber;
        }

        public IActorRef SubscriptionQueue { get; }
    }
}