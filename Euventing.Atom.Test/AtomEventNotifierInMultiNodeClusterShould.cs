using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Sqlite.Snapshot;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomEventNotifierInMultiNodeClusterShould
    {
        private static SubscriptionMessage subscriptionMessage;

        private int totalNotifications;
        private int numberOfNodes;

        private List<AtomEventNotifier> atomNotifiers;
        private List<AtomDocumentRetriever> atomDocumentRetrievers;

        [OneTimeSetUp]
        public void SetupActorSystem()
        {
            atomNotifiers = new List<AtomEventNotifier>();
            atomDocumentRetrievers = new List<AtomDocumentRetriever>();

            Create(3626, "atomActorSystem", "127.0.0.1:3626");
            //Create(3627, "atomActorSystem", "127.0.0.1:3626");
            //Create(3628, "atomActorSystem", "127.0.0.1:3626");
            //Create(3629, "atomActorSystem", "127.0.0.1:3626");
            //Create(3630, "atomActorSystem", "127.0.0.1:3626");


            Thread.Sleep(TimeSpan.FromSeconds(10));

            subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new SubscriptionId("1"),
                new AllEventMatcher());

            atomNotifiers[0].Create(subscriptionMessage);

            Thread.Sleep(TimeSpan.FromSeconds(4));
        }

        private void Create(int port, string actorSystemName, string seedNode)
        {
            var factory = new ShardedActorSystemFactory(port, actorSystemName, "inmem", seedNode);
            var actorSystem = factory.GetActorSystem();
            actorSystem.ActorOf(Props.Create<ClusterEventListener>());
            var actorFactory = new ShardedAtomFeedFactory(actorSystem);
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            atomNotifiers.Add(new AtomEventNotifier(actorFactory));
            atomDocumentRetrievers.Add(new AtomDocumentRetriever(actorFactory, atomDocumentFactory));
            numberOfNodes++;
        }

        [Test]
        public async Task CreateANewAtomFeedWithAHeadDocument()
        {
            var document = await atomDocumentRetrievers[0].GetHeadDocumentId(subscriptionMessage.SubscriptionId);
            Assert.IsNotNull(document.Id);
        }

        [Test]
        public async Task AddAnEventToTheHeadDocumentIdWhenAnEventIsSubmitted()
        {
            Notify(16);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var document = await atomDocumentRetrievers[0].GetHeadDocument(subscriptionMessage.SubscriptionId).WithTimeout(TimeSpan.FromSeconds(5));

            Assert.AreEqual(16, document.Entries.Count);
        }

        [Test]
        public async Task CreateANewDocumentIdWhen151EventsAreSubmitted()
        {
            Notify(171);

            Thread.Sleep(TimeSpan.FromSeconds(3));

            var headDocument = await atomDocumentRetrievers[0].GetHeadDocument(subscriptionMessage.SubscriptionId);
            Assert.IsNotNull(headDocument.EarlierEventsDocumentId);
            var earlierDocument = await atomDocumentRetrievers[0].GetDocument(headDocument.EarlierEventsDocumentId);

            Assert.AreEqual(totalNotifications, headDocument.Entries.Count + earlierDocument.Entries.Count);
        }

        private void Notify(int numberOfNotifications)
        {
            for (int i = 0; i < numberOfNotifications; i++)
            {
                totalNotifications++;

                atomNotifiers[i % numberOfNodes].Notify(subscriptionMessage, new DummyDomainEvent(i.ToString()));
                
                Thread.Sleep(TimeSpan.FromMilliseconds(20));
            }
        }
    }
}
