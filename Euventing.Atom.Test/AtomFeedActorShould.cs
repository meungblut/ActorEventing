using System;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Document;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
   public  class AtomFeedActorShould
    {
        private ShardedActorSystemFactory shardedActorSystemFactory;
        private ActorSystem system;
        private IActorRef atomActorRef;
        private DocumentId documentId;

        [OneTimeSetUp]
        public void Setup()
        {
            shardedActorSystemFactory = new ShardedActorSystemFactory();
            system = shardedActorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            CreateAtomActor("123");
        }

        private void CreateAtomActor(string actorId)
        {
            var dummyDocumentCreator = new DummyAtomDocumentActorCreator();
            var props = Props.Create(() => new AtomFeedActor(new DummyAtomDocumentActorCreator()));

            atomActorRef = system.ActorOf(props, name: actorId);
        }

       [Test]
       public void CreateAHeadDocumentWithTheExpectedParametersWhenAFeedIsCreated()
       {
           
       }

       [Test]
       public void CreateANewDocumentWhenToldTheCurrentHeadDocumentIsFull()
       {
           
       }

       [Test]
       public void UpdateTheHeadDocumentWhenTheNewDocumentIsREadyToAcceptEvents()
       {
           
       }

       [Test]
       public void ReturnANullFeedIfTheFeedHasNotBeenInitialised()
       {
           //Passivate immediately?
       }
    }

    internal class DummyAtomDocumentActorCreator : IAtomDocumentActorBuilder
    {
        public IActorRef GetActorRef()
        {
            throw new NotImplementedException();
        }
    }
}
