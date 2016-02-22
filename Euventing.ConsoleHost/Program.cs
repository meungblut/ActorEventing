using System;
using System.Threading;
using Akka.Actor;
using Euventing.Atom;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;

namespace Euventing.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystemFactory = new ShardedActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(int.Parse(args[0]), args[1], args[2]);

            actorSystem.ActorOf(Props.Create<SimpleClusterListener>());
            var actorFactory = new ShardedAtomFeedFactory(actorSystem);
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);

            var notifier = new AtomEventNotifier(actorFactory);
            var retriever = new AtomDocumentRetriever(actorFactory, atomDocumentFactory);

            Console.WriteLine("Setup with {0}, {1}, {2}", args[0], args[1], args[2]);

            Console.Title = args[0];

            Thread.Sleep(TimeSpan.FromSeconds(10));
             
            var subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId("2"),
                new AllEventMatcher());

            notifier.Create(subscriptionMessage);

            for (int i = 0; i < 1000; i++)
            {
                notifier.Notify(subscriptionMessage, new DummyDomainEvent(args[0] + ":" + i.ToString()));

                Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            }
        }
    }

    public class DummyDomainEvent : DomainEvent
    {
        public string ToString { get; }

        public DummyDomainEvent(string toString) : base(toString, DateTime.Now)
        {
            ToString = toString;
        }
    }
}
