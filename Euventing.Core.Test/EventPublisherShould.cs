using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test.LocalEventNotification;
using NUnit.Framework;

namespace Euventing.Core.Test
{
    public class EventPublisherShould
    {
        private static SubscriptionManager _subscriptionManager;
        private static EventPublisher _eventPublisher;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var settings = new LocalNotificationSettings();
            var actorSystemFactory = new ShardedActorSystemFactory(8965, "eventActorSystemForTesting", "inmem", "127.0.0.1:8965");
            var actorSystem = actorSystemFactory.GetActorSystem();
            _subscriptionManager = new SubscriptionManager(actorSystem);
            _eventPublisher = new EventPublisher(actorSystem);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task NotifyALocalCallbackWhenAnEventIsPublished()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _subscriptionManager.CreateSubscription(subscriptionMessage);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var dummyEvent = new DummyDomainEvent("some id");
            _eventPublisher.PublishMessage(dummyEvent);

            if (!LocalEventNotifier.EventReceived.WaitOne(TimeSpan.FromSeconds(2)))
                Assert.Fail("Expected but did not receive event notification");

            Assert.AreEqual(dummyEvent, LocalEventNotifier.EventNotifiedWith);
        }
    }
}
