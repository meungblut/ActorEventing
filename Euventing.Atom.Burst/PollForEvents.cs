using System;
using Akka.Actor;

namespace Euventing.Atom.Burst
{
    public class PollForEvents
    {
        public PollForEvents(IActorRef addressToPoll)
        {
            AddressToPoll = addressToPoll;
        }

        public PollForEvents(IActorRef addressToPoll, Guid lastBatchProcessed)
        {
            AddressToPoll = addressToPoll;
        }

        public IActorRef AddressToPoll { get; private set; }

        public Guid LastBatchProcessed { get; private set; }
    }
}