using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Burst
{
    internal class ActorLocations
    {
        internal static string LocalSubscriptionActorLocation = "akka://akkaSystemName@{0}/user/subscriptionQueueActor";
        internal static string SubscriptionQueueActorLocation = "";
        internal static string WorkPullingDocumentActorLocation = "";
        internal static string LocalSubscriptionManagerLocation = "/user/allLocalSubscriptionsActor";
    }
}
