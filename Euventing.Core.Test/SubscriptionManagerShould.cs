using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test.LocalEventNotification;
using NUnit.Framework;
using Akka.Cluster;
using Euventing.Core.Subscriptions;

namespace Euventing.Core.Test
{
    public class SubscriptionManagerShould
    {
        private static SingleShardedSubscriptionManager _singleShardedSubscriptionManager;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystemFactory = new ShardedActorSystemFactory(8964, "eventActorSystemForTesting", "inmem", "127.0.0.1:8964");
            var actorSystem = actorSystemFactory.GetActorSystem();
            _singleShardedSubscriptionManager = new SingleShardedSubscriptionManager(actorSystem);
        }

        [Test]
        public async Task ReturnNullSubscriptionMessageIfSubscriptionHasNotBeenMade()
        {

            var queryMessage = new SubscriptionQuery(new SubscriptionId("someotheruuid"));
            var result = await _singleShardedSubscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.IsInstanceOf<NullSubscription>(result);
        }

        [Test]
        public async Task CreateANewSubscriptionIfTheSubscriptionDoesntExist()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _singleShardedSubscriptionManager.CreateSubscription(subscriptionMessage);

            var queryMessage = new SubscriptionQuery(subscriptionMessage.SubscriptionId);
            var result = await _singleShardedSubscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.AreEqual(subscriptionMessage, result);
        }
    }
}
