using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomEventNotifierInMultiNodeClusterShould
    {
        private static ShardedActorSystemFactory factory = new ShardedActorSystemFactory();
        private static AtomEventNotifier _notifier;
        private static AtomEventNotifier _notifier1;
        private static AtomDocumentRetriever _retriever;
        private static AtomDocumentRetriever _retriever1;
        private static SubscriptionMessage subscriptionMessage;

        private int totalNotifications;

        [OneTimeSetUp]
        public static void SetupActorSystem()
        {
            var actorSystem = factory.GetActorSystemWithSqlitePersistence(3626, "atomActorSystem", "127.0.0.1:3626");
            var actorSystem1 = factory.GetActorSystemWithSqlitePersistence(3627, "atomActorSystem", "127.0.0.1:3626");
            actorSystem1.ActorOf(Props.Create<SimpleClusterListener>());
            actorSystem.ActorOf(Props.Create<SimpleClusterListener>());

            var actorFactory = new ShardedAtomFeedFactory(actorSystem);
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);

            var actorFactory1 = new ShardedAtomFeedFactory(actorSystem1);
            var atomDocumentFactory1 = new ShardedAtomDocumentFactory(actorSystem1);

            _notifier = new AtomEventNotifier(actorFactory);
            _retriever = new AtomDocumentRetriever(actorFactory, atomDocumentFactory);

            _notifier1 = new AtomEventNotifier(actorFactory1);
            _retriever1 = new AtomDocumentRetriever(actorFactory1, atomDocumentFactory1);

            Thread.Sleep(TimeSpan.FromSeconds(5));

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
                if (i % 2 == 0)
                    _notifier1.Notify(subscriptionMessage, new DummyDomainEvent(i.ToString()));
                else
                    _notifier.Notify(subscriptionMessage, new DummyDomainEvent(i.ToString()));

                Thread.Sleep(TimeSpan.FromMilliseconds(20));
            }
        }
    }
}
