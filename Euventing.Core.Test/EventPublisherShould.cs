using System;
using System.Threading;
using System.Threading.Tasks;
using Eventing.Core.EventMatching;
using Eventing.Core.Messages;
using Eventing.Core.Publishing;
using Eventing.Core.Subscriptions;
using Eventing.Core.Test.LocalEventNotification;
using NUnit.Framework;

namespace Eventing.Core.Test
{
    public class EventPublisherShould
    {
        private static ShardedSubscriptionManager _shardedSubscriptionManager;
        private static DistributedPubSubEventPublisher _distributedPubSubEventPublisher;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var settings = new LocalNotificationSettings();
            var actorSystemFactory = new ShardedActorSystemFactory(8965, "eventActorSystemForTesting", "inmem", "127.0.0.1:8965");
            var actorSystem = actorSystemFactory.GetActorSystem();
            _shardedSubscriptionManager = new ShardedSubscriptionManager(actorSystem);
            _distributedPubSubEventPublisher = new DistributedPubSubEventPublisher(actorSystem);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task NotifyALocalCallbackWhenAnEventIsPublished()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _shardedSubscriptionManager.CreateSubscription(subscriptionMessage);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var dummyEvent = new DummyDomainEvent("some id");
            _distributedPubSubEventPublisher.PublishMessage(dummyEvent);

            if (!LocalEventNotifier.EventReceived.WaitOne(TimeSpan.FromSeconds(2)))
                Assert.Fail("Expected but did not receive event notification");

            Assert.AreEqual(dummyEvent, LocalEventNotifier.EventNotifiedWith);
        }
    }
}
