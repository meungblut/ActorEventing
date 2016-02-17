using System;
using System.Threading;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomEventNotifierShould
    {
        private static ActorSystemFactory factory = new ActorSystemFactory();
        private static AtomEventNotifier _notifier;
        private static AtomDocumentRetriever _retriever;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystemFactory = new ActorSystemFactory();
            var actorSystem = factory.GetActorSystem(3624, "atomActorSystem", "127.0.0.1:3624");
            _notifier = new AtomEventNotifier(actorSystem);
            _retriever = new AtomDocumentRetriever(actorSystem);
        }

        [Test]
        public void CreateANewAtomFeedWithTheSubscriptionId()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()), 
                new SubscriptionId(Guid.NewGuid().ToString()), 
                new AllEventMatcher());



            Thread.Sleep(TimeSpan.FromSeconds(2));
            _notifier.Create(subscriptionMessage);

            var document = _retriever.GetHeadDocument(subscriptionMessage.SubscriptionId);
        }
    }
}
