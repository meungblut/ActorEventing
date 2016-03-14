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
            loggingAdapter.Info("Received subscribe ack " + subscriptionMessage.SubscriptionId.Id);
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            loggingAdapter.Info("Received delete subscription message " + subscriptionMessage.SubscriptionId.Id);
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Unsubscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(SubscriptionMessage subscription)
        {
            loggingAdapter.Info("Creating a subscription with id " + subscription.SubscriptionId.Id);

            if (subscriptionMessage != null)
            {
                loggingAdapter.Info("Dumping out of subscription for " + subscription.SubscriptionId.Id +
                    " as subscription Id wasn't null ");

                return;
            }

            loggingAdapter.Info("Creating docs for " + subscription.SubscriptionId.Id);

            var notifier = notifierFactory.GetNotifierFor(subscription.NotificationChannel.GetType());
            notifier.Create(subscription);

            loggingAdapter.Info("Persisting " + subscription.SubscriptionId.Id);
            Persist(subscription, MutateInternalState);
        }

        private void SubscribeToClusterWideBroadcastDomainEvent()
        {
            loggingAdapter.Info("Subscribed to cluster domain events " + subscriptionMessage.SubscriptionId.Id);

            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Subscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(DomainEvent eventToProcess)
        {
            loggingAdapter.Info("Received an event notification " + eventToProcess.Id + " on subscription " + subscriptionMessage.SubscriptionId.Id);
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
            loggingAdapter.Info("Persist finished " + subscriptionMessage.SubscriptionId.Id);

            SubscribeToClusterWideBroadcastDomainEvent();
        }

        private void MutateInternalState(object unhandledObject)
        {
            loggingAdapter.Info("Subscription Actor. Unhandled persistence command " + unhandledObject.GetType());
        }
    }
}
