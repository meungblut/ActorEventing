using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test.LocalEventNotification;
using NUnit.Framework;

namespace Euventing.Core.Test
{
    public class SubscriptionManagerShould
    {
        private static SubscriptionManager _subscriptionManager;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystemFactory = new ShardedActorSystemFactory(8964, "eventActorSystemForTesting", "inmem", "127.0.0.1:8964");
            var actorSystem = actorSystemFactory.GetActorSystem();
            _subscriptionManager = new SubscriptionManager(actorSystem);
        }

        [Test]
        public async Task ReturnNullSubscriptionMessageIfSubscriptionHasNotBeenMade()
        {
            var queryMessage = new SubscriptionQuery(new SubscriptionId("someotheruuid"));
            var result = await _subscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.IsInstanceOf<NullSubscription>(result);
        }

        [Test]
        public async Task CreateANewSubscriptionIfTheSubscriptionDoesntExist()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _subscriptionManager.CreateSubscription(subscriptionMessage);

            var queryMessage = new SubscriptionQuery(subscriptionMessage.SubscriptionId);
            var result = await _subscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.AreEqual(subscriptionMessage, result);
        }
    }
}
