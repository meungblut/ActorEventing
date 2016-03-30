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
using Euventing.Core.Subscriptions;
using Euventing.Test.Shared;

namespace Euventing.ConsoleHost
{
    public class Program
    {
        private static SubscriptionMessage _subscriptionMessage;

        public static void Main(string[] args)
        {
            var akkaSystemName = GetValueFromCommandLine("akkaSystemName", args);
            var seedNodes = GetValueFromCommandLine("seedNodes", args);
            var akkaPortNumber = GetIntFromCommandLine(args, "portNumber");
            var entriesPerDocument = GetIntFromCommandLine(args, "entriesPerDocument");
            var persistence = GetValueFromCommandLine("persistence", args);

            var eventSystemHost = new BurstingEventSystemHost(akkaPortNumber, akkaSystemName, persistence, seedNodes, 3601, entriesPerDocument);
            var EventRaisingController = eventSystemHost.Get<EventRaisingController>();
            eventSystemHost.Start();

            Console.Title = string.Join(" ", args);
            Thread.Sleep(TimeSpan.FromSeconds(5));

            var subscriptionId = new SubscriptionId(GetValueFromCommandLine("subscriptionId", args));

            if (! string.IsNullOrEmpty(subscriptionId.Id))
            {
                Console.WriteLine("Getting subscriptionId");
                ISubscriptionManager shardedSubscriptionManager = eventSystemHost.Get<ISubscriptionManager>();
                
                var currentSubscription =
                    shardedSubscriptionManager.GetSubscriptionDetails(new SubscriptionQuery(subscriptionId)).Result;

                if (currentSubscription is NullSubscription)
                {
                    _subscriptionMessage = new SubscriptionMessage(
                        new AtomNotificationChannel(),
                        subscriptionId,
                        new AllEventMatcher());

                    shardedSubscriptionManager.CreateSubscription(_subscriptionMessage);
                }

                Thread.Sleep(1000);

                IEventPublisher notifier = eventSystemHost.Get<IEventPublisher>();

                var i = 0;
                while (true)
                {
                    notifier.PublishMessage(new DummyDomainEvent(akkaPortNumber + ":" + (++i).ToString()));
                    LogManager.GetLogger("").Info("Raising event with id" + i);
                }
            }

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private static int GetIntFromCommandLine(string[] args, string prefix)
        {
            return int.Parse(GetValueFromCommandLine(prefix, args));
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
