using System;
using System.Linq;
using System.Threading;
using Euventing.Atom;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Euventing.Core.Startup;
using NLog;
using Euventing.Api.Startup;

namespace Euventing.ConsoleHost
{
    class Program
    {
        private static SubscriptionMessage _subscriptionMessage;

        static void Main(string[] args)
        {
            var akkaSystemName = GetValueFromCommandLine("akkaSystemName", args);
            var seedNodes = GetValueFromCommandLine("seedNodes", args);
            var akkaPortNumber = GetPortFromCommandLine(args);
            var persistence = GetValueFromCommandLine("persistence", args);

            var eventSystemHost = new EventSystemHost(akkaPortNumber, akkaSystemName, persistence, seedNodes, 3601);
            eventSystemHost.Start();

            Console.Title = string.Join(" ", args);
            Thread.Sleep(TimeSpan.FromSeconds(10));

            var subscriptionId = new SubscriptionId(GetValueFromCommandLine("subscriptionId", args));

            if (! string.IsNullOrEmpty(subscriptionId.Id))
            {
                Console.WriteLine("Getting subscriptionId");
                SubscriptionManager subscriptionManager = eventSystemHost.Get<SubscriptionManager>();
                
                var currentSubscription =
                    subscriptionManager.GetSubscriptionDetails(new SubscriptionQuery(subscriptionId)).Result;

                if (currentSubscription is NullSubscription)
                {
                    _subscriptionMessage = new SubscriptionMessage(
                        new AtomNotificationChannel(),
                        subscriptionId,
                        new AllEventMatcher());

                    subscriptionManager.CreateSubscription(_subscriptionMessage);
                }

                Thread.Sleep(1000);

                EventPublisher notifier = eventSystemHost.Get<EventPublisher>();

                var i = 0;
                while (true)
                {
                    notifier.PublishMessage(new DummyDomainEvent(GetPortFromCommandLine(args) + ":" + (++i).ToString()));
                    LogManager.GetLogger("").Info("Raising event with id" + i);
                    Thread.Sleep(TimeSpan.FromMilliseconds(5));
                }
            }

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private static int GetPortFromCommandLine(string[] args)
        {
            return int.Parse(GetValueFromCommandLine("portNumber", args));
        }

        private static string GetValueFromCommandLine(string switchPrefix, string[] args)
        {
            string value = args.FirstOrDefault(x => x.StartsWith(switchPrefix));

            if (value == null)
                return string.Empty;

            return value.Split(new[] { '/' })[1];
        }
    }
}
