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
        public async Task ConfigureWithTheExpectedDocumentInformation()
        {
            var actorSystemFactory = new ShardedActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(8965, "eventActorSystemForTesting", "127.0.0.1:8965");
            var actorRef = actorSystem.ActorOf<AtomDocumentActor>(name: "123");

            var documentId = new DocumentId(Guid.NewGuid().ToString());

            var atomDocumentCreationInformation = new CreateAtomDocumentCommand(
                "Matt's Events Feed",
                "Matt",
                Guid.NewGuid().ToString(),
                documentId,
                Guid.NewGuid().ToString());

            actorRef.Tell(atomDocumentCreationInformation, new StandardOutLogger());

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var atomDocument = await actorRef.Ask<AtomDocument>(new GetAtomDocumentRequest(documentId)).WithTimeout(TimeSpan.FromSeconds(2));
            Assert.AreEqual(atomDocument.DocumentId, atomDocumentCreationInformation.DocumentId);
        }
    }

    public static class TaskExtensions
    {
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task;
            }
            throw new TimeoutException();
        }
    }
}
