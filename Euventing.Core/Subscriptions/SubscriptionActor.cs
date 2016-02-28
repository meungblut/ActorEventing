using System;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using Akka.Persistence;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionActor : PersistentActor
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly NotifierFactory notifierFactory;
        private ILoggingAdapter loggingAdapter;

        public override string PersistenceId { get; }

        public SubscriptionActor()
        {
            loggingAdapter = Context.GetLogger();
            loggingAdapter.Info("SUBSCRIPTION actor path is " + Self.Path);
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
            notifierFactory = new NotifierFactory();
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        private void Process(SubscribeAck eventToProcess)
        {

        }

        private void Process(SubscriptionMessage subscriptionMessage)
        {
            var notifier = notifierFactory.GetNotifierFor(subscriptionMessage.NotificationChannel.GetType());
            notifier.Create(subscriptionMessage);

            Persist(subscriptionMessage, MutateInternalState);
        }

        private void SubscribeToClusterWideBroadcastDomainEvent()
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Subscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(DomainEvent eventToProcess)
        {
            Console.WriteLine("Publishing message in subscription actor");
            var notifier = notifierFactory.GetNotifierFor(subscriptionMessage.NotificationChannel.GetType());
            notifier.Notify(subscriptionMessage, eventToProcess);
        }

        private void Process(SubscriptionQuery query)
        {
            if (subscriptionMessage == null)
                Sender.Tell(new NullSubscription(), Context.Self);
            else
                Sender.Tell(subscriptionMessage, Context.Self);
        }

        private void Process(object unhandledObject)
        {
            loggingAdapter.Info("Subscription Actor. Unhandled command " + unhandledObject.GetType());
        }

        private void MutateInternalState(SubscriptionMessage message)
        {
            this.subscriptionMessage = message;

            SubscribeToClusterWideBroadcastDomainEvent();
        }

        private void MutateInternalState(object unhandledObject)
        {
            loggingAdapter.Info("Subscription Actor. Unhandled persistence command " + unhandledObject.GetType());
        }
    }
}
