using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class BurstableEventPublisher : IEventPublisher
    {
        private ActorSystem actorSystem;

        public BurstableEventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            throw new NotImplementedException();
        }
    }
}
