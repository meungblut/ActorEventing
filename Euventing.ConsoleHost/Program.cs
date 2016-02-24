using System;
using System.Threading;
using Akka.Actor;
using Euventing.Atom;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using System.Threading.Tasks;

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
            var actorSystem = actorSystemFactory.GetActorSystemWithSqlitePersistence(int.Parse(args[0]), args[1], args[2]);

            actorSystem.ActorOf(Props.Create<SimpleClusterListener>());
            var actorFeedFactory = new ShardedAtomFeedFactory(actorSystem);
            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);

            _notifier = new AtomEventNotifier(actorFeedFactory);
            _retriever = new AtomDocumentRetriever(actorFeedFactory, atomDocumentFactory);

            Console.WriteLine("Setup with {0}, {1}, {2}", args[0], args[1], args[2]);

            Console.Title = string.Join(" ", args);

            Thread.Sleep(TimeSpan.FromSeconds(10));

            _subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new UserId(Guid.NewGuid().ToString()),
                new SubscriptionId("2"),
                new AllEventMatcher());

            _notifier.Create(_subscriptionMessage);

            if (args.Length > 3 && args[3] == "pollHead")
            {
                Get();
            }
            else
            {
                Notify(args[0]);
            }

        }

        private static void Notify(string port)
        {
            var i = 0;
            while (true)
            {
                _notifier.Notify(_subscriptionMessage, new DummyDomainEvent(port + ":" + (++i).ToString()));

                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }
        }

        private async static Task Get()
        {
            while (true)
            {
                Console.WriteLine(await _retriever.GetSerialisedHeadDocument(_subscriptionMessage.SubscriptionId));

                Thread.Sleep(TimeSpan.FromMilliseconds(200));
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
