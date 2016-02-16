using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
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
            var actorSystemFactory = new ActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            _subscriptionManager = new SubscriptionManager(actorSystem);
            _eventPublisher = new EventPublisher(actorSystem);
            Thread.Sleep(TimeSpan.FromSeconds(4));
        }

        [Test]
        public async Task CreateANewAtomSubscriptionIfTheAtomSubscriptionDoesntExist()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _subscriptionManager.CreateSubscription(subscriptionMessage);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            _eventPublisher.PublishMessage(new DummyDomainEvent());

            Thread.Sleep(TimeSpan.FromSeconds(2));

        }
    }
}
