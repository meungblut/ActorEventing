using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Euventing.Core.Startup
{
    public class EventSystemFactory
    {
        private readonly SubscriptionManager subscriptionManager;
        private readonly EventPublisher eventPublisher;

        public EventSystemFactory(ActorSystem actorSystem, IEnumerable<ISubsytemConfiguration> subsystemConfigurations)
        {
            subscriptionManager = new SubscriptionManager(actorSystem);
            eventPublisher = new EventPublisher(actorSystem);

            foreach (var subsytemConfiguration in subsystemConfigurations)
            {
                subsytemConfiguration.Configure(actorSystem);
            }
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return subscriptionManager;
        }

        public EventPublisher GetEventPublisher()
        {
            return eventPublisher;
        }
        
    }
}
