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
        private static SubscriptionMessage _subscriptionMessage;
        private static AtomEventNotifier _notifier;
        private static AtomDocumentRetriever _retriever;

        static void Main(string[] args)
        {
            var actorSystemFactory = new ShardedActorSystemFactory();
            var actorSystem = actorSystemFactory.GetActorSystem(int.Parse(args[0]), args[1], args[2]);

            actorSystem.ActorOf(Props.Create<SimpleClusterListener>());
            var actorFactory = new ShardedAtomFeedFactory(actorSystem);
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);

            _notifier = new AtomEventNotifier(actorFactory);
            _retriever = new AtomDocumentRetriever(actorFactory, atomDocumentFactory);

            Console.WriteLine("Setup with {0}, {1}, {2}", args[0], args[1], args[2]);

            Console.Title = args[0];

            Thread.Sleep(TimeSpan.FromSeconds(10));
             
            _subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId("2"),
                new AllEventMatcher());

            _notifier.Create(_subscriptionMessage);

            if (args[3] == "notify")
            {
                Notify(args[0]);
            }

        }

        private static void Notify(string port)
        {
            for (int i = 0; i < 1000; i++)
            {
                _notifier.Notify(_subscriptionMessage, new DummyDomainEvent(port + ":" + i.ToString()));

                Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            }
        }

        private static void Get()
        {
            
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
