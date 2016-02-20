using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using Euventing.Atom.Document;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomFeedActorShould
    {
        private ShardedActorSystemFactory shardedActorSystemFactory;
        private ActorSystem system;
        private IActorRef atomActorRef;
        private DummyAtomDocumentActorCreator dummyAtomDocumentActorCreator;
        private FeedId feddId;

        [OneTimeSetUp]
        public void Setup()
        {
            dummyAtomDocumentActorCreator = new DummyAtomDocumentActorCreator();
            shardedActorSystemFactory = new ShardedActorSystemFactory();
            system = shardedActorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            CreateAtomActor("123");
        }

        private void CreateAtomActor(string actorId)
        {
            dummyAtomDocumentActorCreator = new DummyAtomDocumentActorCreator();
            var props = Props.Create(() => new AtomFeedActor(dummyAtomDocumentActorCreator));

            atomActorRef = system.ActorOf(props, name: actorId);
        }

        [Test]
        public void CreateAHeadDocumentWithTheExpectedParametersWhenAFeedIsCreated()
        {
            CreateFeed();

            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(1));
            var documentCreated =
                (CreateAtomDocumentCommand) dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            Assert.AreEqual(feddId.Id, documentCreated.FeedId.Id);
        }

        private void CreateFeed()
        {
            feddId = new FeedId(Guid.NewGuid().ToString());
            var atomFeedCreationCommand = new AtomFeedCreationCommand("title", "author", feddId,
                new DocumentId(Guid.NewGuid().ToString()));
            atomActorRef.Tell(atomFeedCreationCommand);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void CreateANewDocumentWhenToldTheCurrentHeadDocumentIsFull()
        {
            CreateFeed();

            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(1));
            var documentCreated =
                (CreateAtomDocumentCommand)dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            atomActorRef.Tell(new AtomDocumentFullEvent(documentCreated.DocumentId));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.Reset();
            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(1));
            var secondDocumentCreated =
                (CreateAtomDocumentCommand)dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            Assert.AreNotEqual(documentCreated.DocumentId.Id, secondDocumentCreated.DocumentId.Id);
        }

        [Test]
        public async Task UpdateTheHeadDocumentWhenTheNewDocumentIsReadyToAcceptEvents()
        {
            CreateFeed();

            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(1));
            var documentCreated =
                (CreateAtomDocumentCommand)dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            atomActorRef.Tell(new AtomDocumentFullEvent(documentCreated.DocumentId));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.Reset();
            dummyAtomDocumentActorCreator.ActorRefReturned.ActorTellCalled.WaitOne(TimeSpan.FromSeconds(1));
            var secondDocumentCreated =
                (CreateAtomDocumentCommand)dummyAtomDocumentActorCreator.ActorRefReturned.MessageTellCalledWith;

            atomActorRef.Tell(new DocumentReadyToReceiveEvents(secondDocumentCreated.DocumentId));

            var documentId = await atomActorRef.Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(null));
            Assert.AreEqual(secondDocumentCreated.DocumentId, documentId);
        }

        [Test]
        public void ReturnANullFeedIfTheFeedHasNotBeenInitialised()
        {
            //Passivate immediately?
        }
    }

    internal class DummyAtomDocumentActorCreator : IAtomDocumentActorBuilder
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
