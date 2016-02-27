using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Euventing.Atom;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using System.Threading.Tasks;
using Euventing.Core.Startup;
using NLog;

namespace Euventing.ConsoleHost
{
    class Program
    {
        private static SubscriptionMessage _subscriptionMessage;

        static void Main(string[] args)
        {
            var actorSystemFactory = new ShardedActorSystemFactory(GetPortFromCommandLine(args),
                GetValueFromCommandLine("akkaSystemName", args),
                "sqlite",
                GetValueFromCommandLine("seedNodes", args));

            var actorSystem = actorSystemFactory.GetActorSystem();

            LogManager.GetLogger("").Info("Matt - creating system");

            var subsystemConfig = new AtomSubsystemConfiguration();
            var eventSystemFactory = new EventSystemFactory(actorSystem, new[] { subsystemConfig });

            Console.Title = string.Join(" ", args);
            LogManager.GetLogger("").Info("Matt - about to sleep");
            Thread.Sleep(TimeSpan.FromSeconds(10));
            LogManager.GetLogger("").Info("Matt - finished sleeping");


            var subscriptionId = GetValueFromCommandLine("subscriptionId", args);

            LogManager.GetLogger("").Info("Matt: subscribing to " + subscriptionId);

            _subscriptionMessage = new SubscriptionMessage(
                new AtomNotificationChannel(),
                new SubscriptionId(subscriptionId),
                new AllEventMatcher());

            eventSystemFactory.GetSubscriptionManager().CreateSubscription(_subscriptionMessage);
            LogManager.GetLogger("").Info("MAtt: subscription finished " + subscriptionId);

            var notifier = eventSystemFactory.GetEventPublisher();

            var i = 0;
            while (true)
            {
                notifier.PublishMessage(new DummyDomainEvent(GetPortFromCommandLine(args) + ":" + (++i).ToString()));

                Thread.Sleep(TimeSpan.FromMilliseconds(50));
            }
        }

        private static int GetPortFromCommandLine(string[] args)
        {
            return int.Parse(GetValueFromCommandLine("portNumber", args));
        }

        private static string GetValueFromCommandLine(string switchPrefix, string[] args)
        {
            return args.First(x => x.StartsWith(switchPrefix)).Split(new[] { '/' })[1];
        }
    }
}
