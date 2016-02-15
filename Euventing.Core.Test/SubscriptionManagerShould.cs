using System;
using System.Data.SqlClient;
using System.Threading;
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
            var actorSystemFactory = new ActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(8964, "eventActorSystemForTesting", "127.0.0.1:8964");
            var subscriptionManager = new SubscriptionManager(actorSystem);

            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            subscriptionManager.CreateSubscription(subscriptionMessage);

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void NotifyTheShardRegionThatASubscriptionHasBeenReceived()
        {
            
        }
    }
}
