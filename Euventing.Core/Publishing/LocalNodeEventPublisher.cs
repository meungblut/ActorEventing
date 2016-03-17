using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core.Messages;

namespace Euventing.Core.Publishing
{
    public class LocalNodeEventPublisher : IEventPublisher
    {
        private readonly ActorSystem actorSystem;

        public LocalNodeEventPublisher(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
        }
    }
}
