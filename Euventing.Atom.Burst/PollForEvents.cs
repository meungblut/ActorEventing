using Akka.Actor;

namespace Euventing.Atom.Burst
{
    public class PollForEvents
    {
        public PollForEvents(Address addressToPoll)
        {
            AddressToPoll = addressToPoll;
        }

        public Address AddressToPoll { get; private set; }
    }
}