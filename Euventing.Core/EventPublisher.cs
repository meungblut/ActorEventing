using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Euventing.Core
{
    public class EventPublisher
    {
        private readonly ActorSystem actorSystem;

        public EventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        //akka://eventActorSystemForTesting/user/sharding/SubscriptionActor
        public void PublishMessages(object thingToPublish)
        {
            var actorRef =
                actorSystem.ActorSelection("/user/sharding/SubscriptionActor/*");
            actorRef.Tell(thingToPublish);
        }
    }
}
