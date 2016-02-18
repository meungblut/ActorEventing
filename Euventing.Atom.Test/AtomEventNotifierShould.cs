using System;
using System.Threading;
using System.Threading.Tasks;
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
            AtomFeedShardedActorRefFactory actorFActory = new AtomFeedShardedActorRefFactory(actorSystem);
            _notifier = new AtomEventNotifier(actorFActory);
            _retriever = new AtomDocumentRetriever(actorFActory);
        }

        [Test]
        public async Task CreateANewAtomFeedWithTheSubscriptionId()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()), 
                new SubscriptionId(Guid.NewGuid().ToString()), 
                new AllEventMatcher());
            
            Thread.Sleep(TimeSpan.FromSeconds(4));
            _notifier.Create(subscriptionMessage);

            var document = await _retriever.GetHeadDocument(subscriptionMessage.SubscriptionId);
            Assert.IsNotNull(document.Id);
        }
    }
}
