using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomDocumentActorShould
    {
        private ShardedActorSystemFactory shardedActorSystemFactory;
        private ActorSystem system;
        private IActorRef atomActorRef;
        private DocumentId documentId;
        
        [OneTimeSetUp]
        public void Setup()
        {
            shardedActorSystemFactory = new ShardedActorSystemFactory(8965, "eventActorSystemForTesting", "inmem", "127.0.0.1:8965");
            system = shardedActorSystemFactory.GetActorSystem();
            CreateAtomActor("123");
        }

        private void CreateAtomActor(string actorId)
        {
            var props = Props.Create(() => new AtomDocumentActor());

            atomActorRef = system.ActorOf(props, name: actorId);

            documentId = new DocumentId(Guid.NewGuid().ToString());

            var atomDocumentCreationInformation = new CreateAtomDocumentCommand(
                "Matt's Events Feed",
                "Matt",
                new FeedId(Guid.NewGuid().ToString()),
                documentId,
                new DocumentId(Guid.NewGuid().ToString()));

            atomActorRef.Tell(atomDocumentCreationInformation, new StandardOutLogger());
        }

        [Test]
        public async Task ConfigureWithTheExpectedDocumentInformation()
        {
            var atomDocument = await atomActorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));
            Assert.AreEqual(atomDocument.DocumentId, documentId);
        }

        [Test]
        public async Task AddRaisedEventToTheAtomDocument()
        {
            var eventId = Guid.NewGuid().ToString();
            atomActorRef.Tell(new EventWithDocumentIdNotificationMessage(documentId, 
                new DummyDomainEvent(eventId)));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var atomDocument = await atomActorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));

            Assert.IsTrue(atomDocument.Entries.Any(x => x.Content.Contains(eventId)));
        }

        [Test]
        public async Task SetLaterEventDocumentIdWhenSentANewDocumentAddedEvent()
        {
            var newdocumentId = new DocumentId(Guid.NewGuid().ToString());

            atomActorRef.Tell(new NewDocumentAddedEvent(newdocumentId));

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            var atomDocument = await atomActorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));

            Assert.AreEqual(newdocumentId.Id, atomDocument.LaterEventsDocumentId.Id);
        }
    }
}
