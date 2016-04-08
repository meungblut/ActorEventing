using Akka.Actor;

namespace Euventing.Atom.Burst
{
    public class PollForEvents
    {
        public PollForEvents(IActorRef addressToPoll)
        {
            AddressToPoll = addressToPoll;
        }

        public IActorRef AddressToPoll { get; private set; }
    }
}