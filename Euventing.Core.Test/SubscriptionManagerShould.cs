using System;
using System.Data.SqlClient;
using Akka.Actor;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using NUnit.Framework;

namespace Euventing.Core.Test
{
    public class SubscriptionManagerShould
    {
        [Test]
        public void CreateANewAtomSubscriptionIfTheAtomSubscriptionDoesntExist()
        {
            var actorSystem = ActorSystem.Create("eventActorSystemForTesting");
            var subscriptionManager = new SubscriptionManager(actorSystem);

            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            subscriptionManager.CreateSubscription(subscriptionMessage);
        }

        [Test]
        public void NotifyTheShardRegionThatASubscriptionHasBeenReceived()
        {
            
        }
    }
}
