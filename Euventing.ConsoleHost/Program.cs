using System;
using System.Linq;
using System.Threading;
using Euventing.Atom;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
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

            var subsystemConfig = new AtomSubsystemConfiguration();
            var eventSystemFactory = new EventSystemFactory(actorSystem, new[] { subsystemConfig });

            Console.Title = string.Join(" ", args);
            Thread.Sleep(TimeSpan.FromSeconds(10));

          
            var subscriptionId = new SubscriptionId(GetValueFromCommandLine("subscriptionId", args));
            var subscriptionManager = eventSystemFactory.GetSubscriptionManager();


            var currentSubscription = subscriptionManager.GetSubscriptionDetails(new SubscriptionQuery(subscriptionId)).Result;

            if (currentSubscription is NullSubscription)
            {
                _subscriptionMessage = new SubscriptionMessage(
                    new AtomNotificationChannel(),
                    subscriptionId,
                    new AllEventMatcher());

                eventSystemFactory.GetSubscriptionManager().CreateSubscription(_subscriptionMessage);
            }

            Thread.Sleep(1000);

            var notifier = eventSystemFactory.GetEventPublisher();

            var i = 0;
            while (true)
            {
                notifier.PublishMessage(new DummyDomainEvent(GetPortFromCommandLine(args) + ":" + (++i).ToString()));
                LogManager.GetLogger("").Info("Raising event with id" + i);
                Thread.Sleep(TimeSpan.FromMilliseconds(5));
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
