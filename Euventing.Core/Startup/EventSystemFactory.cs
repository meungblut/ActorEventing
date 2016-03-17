using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core.Publishing;
using Euventing.Core.Subscriptions;

namespace Euventing.Core.Startup
{
    public class EventSystemFactory
    {
        private readonly SingleShardedSubscriptionManager singleShardedSubscriptionManager;
        private readonly DistributedPubSubEventPublisher distributedPubSubEventPublisher;
        private readonly ActorSystem actorSystem;

        public EventSystemFactory(ActorSystem actorSystem, IEnumerable<ISubsytemConfiguration> subsystemConfigurations)
        {
            this.actorSystem = actorSystem;
            singleShardedSubscriptionManager = new SingleShardedSubscriptionManager(actorSystem);
            distributedPubSubEventPublisher = new DistributedPubSubEventPublisher(actorSystem);

            foreach (var subsytemConfiguration in subsystemConfigurations)
            {
                subsytemConfiguration.Configure(actorSystem);
            }
        }

        public SingleShardedSubscriptionManager GetSubscriptionManager()
        {
            return singleShardedSubscriptionManager;
        }

        public DistributedPubSubEventPublisher GetEventPublisher()
        {
            return distributedPubSubEventPublisher;
        }

        public void Stop()
        {
            actorSystem.Terminate();
        }
    }
}
