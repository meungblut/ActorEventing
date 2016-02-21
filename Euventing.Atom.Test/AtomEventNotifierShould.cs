using System;
using System.Threading;
using System.Threading.Tasks;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomEventNotifierShould
    {
        private static ShardedActorSystemFactory factory = new ShardedActorSystemFactory();
        private static AtomEventNotifier _notifier;
        private static AtomDocumentRetriever _retriever;
        private static SubscriptionMessage subscriptionMessage;

        private int totalNotifications;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystemFactory = new ShardedActorSystemFactory();
            var actorSystem = factory.GetActorSystem(3624, "atomActorSystem", "127.0.0.1:3624");

            AtomFeedShardedActorRefFactory actorFActory = new AtomFeedShardedActorRefFactory(actorSystem);
            ShardedAtomDocumentFactory atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            _notifier = new AtomEventNotifier(actorFActory);
            _retriever = new AtomDocumentRetriever(actorFActory, atomDocumentFactory);

            subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId(Guid.NewGuid().ToString()),
                new AllEventMatcher());

            _notifier.Create(subscriptionMessage);
        }

        [Test]
        public async Task CreateANewAtomFeedWithAHeadDocument()
        {
            var document = await _retriever.GetHeadDocumentId(subscriptionMessage.SubscriptionId);
            Assert.IsNotNull(document.Id);
        }

        [Test]
        public async Task AddAnEventToTheHeadDocumentIdWhenAnEventIsSubmitted()
        {
            Notify(16);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var document = await _retriever.GetHeadDocument(subscriptionMessage.SubscriptionId).WithTimeout(TimeSpan.FromSeconds(5));

            Assert.AreEqual(16, document.Entries.Count);
        }

        [Test]
        public async Task CreateANewDocumentIdWhen151EventsAreSubmitted()
        {
            Notify(171);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var headDocument = await _retriever.GetHeadDocument(subscriptionMessage.SubscriptionId);
            Assert.IsNotNull(headDocument.EarlierEventsDocumentId);
            var earlierDocument = await _retriever.GetDocument(headDocument.EarlierEventsDocumentId);

            Assert.AreEqual(totalNotifications, headDocument.Entries.Count + earlierDocument.Entries.Count);
        }

        private void Notify(int numberOfNotifications)
        {
            for (int i = 0; i < numberOfNotifications; i++)
            {
                totalNotifications++;
                _notifier.Notify(subscriptionMessage, new DummyDomainEvent(Guid.NewGuid().ToString()));
                Thread.Sleep(TimeSpan.FromMilliseconds(20));
            }
        }
    }
}
