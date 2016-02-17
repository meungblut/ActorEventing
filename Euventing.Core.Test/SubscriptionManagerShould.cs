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
            var actorSystemFactory = new ActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(8964, "eventActorSystemForTesting", "127.0.0.1:8964");
            _subscriptionManager = new SubscriptionManager(actorSystem);
        }

        [Test]
        public async Task ReturnNullSubscriptionMessageIfSubscriptionHasNotBeenMade()
        {
            var queryMessage = new SubscriptionQuery(new UserId("someuuid"), new SubscriptionId("someotheruuid"));
            var result = await _subscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.IsInstanceOf<NullSubscription>(result);
        }

        [Test]
        public async Task CreateANewAtomSubscriptionIfTheAtomSubscriptionDoesntExist()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new LocalEventNotificationChannel(), 
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _subscriptionManager.CreateSubscription(subscriptionMessage);

            var queryMessage = new SubscriptionQuery(subscriptionMessage.UserId, subscriptionMessage.SubscriptionId);
            var result = await _subscriptionManager.GetSubscriptionDetails(queryMessage);
            Assert.AreEqual(subscriptionMessage, result);
        }
    }
}
