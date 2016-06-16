using System;
using System.Threading.Tasks;
using Eventing.Core.EventMatching;
using Eventing.Core.Messages;
using Eventing.Core.Subscriptions;
using Eventing.Core.Test.LocalEventNotification;
using NUnit.Framework;

namespace Eventing.Core.Test
{
    public class SubscriptionManagerShould
    {
        private static ShardedSubscriptionManager _shardedSubscriptionManager;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystemFactory = new ShardedActorSystemFactory(8964, "eventActorSystemForTesting", "inmem", "127.0.0.1:8964");
            var actorSystem = actorSystemFactory.GetActorSystem();
            _shardedSubscriptionManager = new ShardedSubscriptionManager(actorSystem);
        }

        [Test]
        public async Task ReturnNullSubscriptionMessageIfSubscriptionHasNotBeenMade()
        {

            var queryMessage = new SubscriptionQuery(new SubscriptionId("someotheruuid"));
            var result = await _shardedSubscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.IsInstanceOf<NullSubscription>(result);
        }

        [Test]
        public async Task CreateANewSubscriptionIfTheSubscriptionDoesntExist()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _shardedSubscriptionManager.CreateSubscription(subscriptionMessage);

            var queryMessage = new SubscriptionQuery(subscriptionMessage.SubscriptionId);
            var result = await _shardedSubscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.AreEqual(subscriptionMessage, result);
        }
    }
}
