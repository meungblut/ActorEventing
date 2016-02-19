using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Document;
using Euventing.Core.Test;
using NUnit.Framework;
using Akka.TestKit;
using Akka.TestKit.NUnit;

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
            shardedActorSystemFactory = new ShardedActorSystemFactory();
            system = shardedActorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            CreateAtomActor("123");
        }

        private void CreateAtomActor(string actorId)
        {
            var props = Props.Create(() => new AtomDocumentActor(new DummyAtomDocumentSettings(2)));

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
            atomActorRef.Tell(new DummyDomainEvent(eventId));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var atomDocument = await atomActorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));

            Assert.IsTrue(atomDocument.Entries.Any(x => x.Content.Contains(eventId)));
        }

        [Test]
        public async Task NotifySenderThatDocumentIsFullWhenNumberOfEventsMatchesMaximumNumberOfEventsPerDocument()
        {
            atomActorRef.Tell(new DummyDomainEvent(Guid.NewGuid().ToString()));

            //Using the ask pattern here because the teskit has an old nunit reference and I got fed up
            //with assembly binding redirects - revisit
            var documentFullEvent = await atomActorRef.Ask<AtomDocumentFullEvent>(new DummyDomainEvent(Guid.NewGuid().ToString())).WithTimeout(TimeSpan.FromSeconds(2));

            Assert.AreEqual(documentId, documentFullEvent.DocumentId);

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        [Test]
        public async Task SetLaterEventDocumentIdWhenSentANewDocumentAddedEvent()
        {
            var newdocumentId = new DocumentId(Guid.NewGuid().ToString());

            atomActorRef.Tell(new NewDocumentAddedEvent(newdocumentId));

            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            var atomDocument = await atomActorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));

            Assert.AreEqual(newdocumentId.Id, atomDocument.LaterEventsDocumentId.Id);
        }
    }
}
