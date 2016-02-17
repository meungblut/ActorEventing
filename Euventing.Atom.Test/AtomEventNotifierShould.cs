using System;
using System.Threading;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Test;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class AtomEventNotifierShould
    {
        ActorSystemFactory factory = new ActorSystemFactory();

        [Test]
        public void CreateANewAtomFeedWithTheSubscriptionId()
        {
            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()), 
                new SubscriptionId(Guid.NewGuid().ToString()), 
                new AllEventMatcher());

            var actorSystem = factory.GetActorSystem(3624, "atomActorSystem");
            AtomEventNotifier notifier = new AtomEventNotifier(actorSystem);

            Thread.Sleep(TimeSpan.FromSeconds(2));
            notifier.Create(subscriptionMessage);

            AtomDocumentRetriever retriever = new AtomDocumentRetriever(actorSystem);
            var document = retriever.GetHeadDocument(subscriptionMessage.SubscriptionId);
        }
    }
}
