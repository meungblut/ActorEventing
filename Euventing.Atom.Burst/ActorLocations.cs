using Akka.Actor;

namespace Eventing.Atom.Burst
{
    internal static class ActorLocations
    {
        internal static string LocalQueueLocation =  $"user_localEventQueue";

        internal static IActorRef LocalQueueActor { get; set; }

    }
}
