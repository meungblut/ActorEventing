using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Util;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core;
using Euventing.Core.Messages;
using Euventing.InMemoryPersistence;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomFeedActorShould
    {
        private ShardedActorSystemFactory shardedActorSystemFactory;
        private ActorSystem system;
        private IActorRef atomActorRef;
        private DummyAtomDocumentActorCreator dummyAtomDocumentActorCreator;
        private FeedId feedId;


        [OneTimeSetUp]
        public void Setup()
        {
            dummyAtomDocumentActorCreator = new DummyAtomDocumentActorCreator();
            shardedActorSystemFactory = new ShardedActorSystemFactory(8965, "eventActorSystemForTesting", "inmem", "127.0.0.1:8965");
            system = shardedActorSystemFactory.GetActorSystem();
            CreateAtomFeedActor(Guid.NewGuid().ToString());
            CreateFeed();
        }

        private void CreateAtomFeedActor(string actorId)
        {
            dummyAtomDocumentActorCreator = new DummyAtomDocumentActorCreator();
            var props = Props.Create(() => new AtomFeedActor(dummyAtomDocumentActorCreator, new DummyAtomDocumentSettings(2)));
            atomActorRef = system.ActorOf(props, name: actorId);
        }

        [Test]
        public void CreateAHeadDocumentWithTheExpectedParametersWhenAFeedIsCreated()
        {
            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(4));
            var documentCreated =
                (CreateAtomDocumentCommand)dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            Assert.AreEqual(feedId.Id, documentCreated.FeedId.Id);
        }

        [Test]
        public void SaveASnapshotWhenTheDocumentIsRotated()
        {
            atomActorRef.Tell(new EventWithSubscriptionNotificationMessage(new SubscriptionId(this.feedId.Id),
                new DummyDomainEvent(Guid.NewGuid().ToString())));

            atomActorRef.Tell(new EventWithSubscriptionNotificationMessage(new SubscriptionId(this.feedId.Id),
                new DummyDomainEvent(Guid.NewGuid().ToString())));

            atomActorRef.Tell(new EventWithSubscriptionNotificationMessage(new SubscriptionId(this.feedId.Id),
    new DummyDomainEvent(Guid.NewGuid().ToString())));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var snapshot = InMemorySnapshotStore.RepositorySavedWIth.GetData<SnapshotEntry>();

            Assert.IsNotNull(snapshot.First().Snapshot);
        }

        private void CreateFeed()
        {
            feedId = new FeedId(Guid.NewGuid().ToString());
            var atomFeedCreationCommand = new AtomFeedCreationCommand("title", "author", feedId,
                new DocumentId(Guid.NewGuid().ToString()));
            atomActorRef.Tell(atomFeedCreationCommand);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void ReturnANullFeedIfTheFeedHasNotBeenInitialised()
        {
            //Passivate immediately?
        }
    }

    internal class DummyAtomDocumentActorCreator : IAtomDocumentActorFactory
    {
        internal DummyActorRef ActorRefReturned { get; private set; }
        public IActorRef GetActorRef()
        {
            ActorRefReturned = new DummyActorRef();
            return ActorRefReturned;
        }
    }

    internal class DummyActorRef : IActorRef
    {
        internal object MessageTellCalledWith { get; private set; }

        internal ManualResetEvent ActorTellCalled = new ManualResetEvent(false);

        public void Tell(object message, IActorRef sender)
        {
            MessageTellCalledWith = message;
            ActorTellCalled.Set();
        }

        public bool Equals(IActorRef other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IActorRef other)
        {
            throw new NotImplementedException();
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public ActorPath Path { get; }
    }
}
