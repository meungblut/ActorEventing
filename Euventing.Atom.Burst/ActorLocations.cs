using System;
using Akka.Actor;

namespace Euventing.Atom.Burst
{
    internal static class ActorLocations
    {
        internal static string LocalQueueLocation =  $"user_localEventQueue";

        internal static IActorRef LocalQueueActor { get; set; }

    }
}
