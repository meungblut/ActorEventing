using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Euventing.Atom.Document;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomDocumentActorShould
    {
       
        [Test]
        public void ConfigureWithTheExpectedDocumentInformation()
        {
            var actorSystemFactory = new ShardedActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            var actorRef = actorSystem.ActorOf<AtomDocumentActor>(name: "123");

            var atomDocumentCreationInformation = new AtomDocumentCreationInformation(
                "Matt's Events Feed",
                "Matt Eungblut",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                5);

            actorRef.Tell(atomDocumentCreationInformation, new StandardOutLogger());

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }

    public class AtomDocumentCreationInformation
    {
        public AtomDocumentCreationInformation(
            string title, 
            string author, 
            string feedId, 
            string documentId, 
            string earlierEventsDocumentId, 
            int numberOfEventsPerDocument)
        {
            Title = title;
            Author = author;
            FeedId = feedId;
            DocumentId = documentId;
            EarlierEventsDocumentId = earlierEventsDocumentId;
            NumberOfEventsPerDocument = numberOfEventsPerDocument;
        }

        public string Title { get; }

        public string Author { get; }

        public string FeedId { get; }

        public string DocumentId { get; }

        public string EarlierEventsDocumentId { get; }

        public int NumberOfEventsPerDocument { get; }
    }
}
