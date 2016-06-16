using System.Collections.Generic;
using Akka.Actor;
using Eventing.Core.Publishing;
using Eventing.Core.Subscriptions;

namespace Eventing.Core.Startup
{
    public class EventSystemFactory
    {
        private readonly ISubscriptionManager shardedSubscriptionManager;
        private readonly DistributedPubSubEventPublisher distributedPubSubEventPublisher;
        private readonly ActorSystem actorSystem;

        public EventSystemFactory(ActorSystem actorSystem, IEnumerable<ISubsytemConfiguration> subsystemConfigurations)
        {
            this.actorSystem = actorSystem;
            shardedSubscriptionManager = new ShardedSubscriptionManager(actorSystem);
            distributedPubSubEventPublisher = new DistributedPubSubEventPublisher(actorSystem);

            foreach (var subsytemConfiguration in subsystemConfigurations)
            {
                subsytemConfiguration.Configure(actorSystem);
            }
        }

        public ISubscriptionManager GetSubscriptionManager()
        {
            return shardedSubscriptionManager;
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
