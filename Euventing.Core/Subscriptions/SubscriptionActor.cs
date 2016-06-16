using Akka.Cluster.Tools.PublishSubscribe;
using Eventing.Core.Messages;
using Eventing.Core.Notifications;

namespace Eventing.Core.Subscriptions
{
    public class SubscriptionActor : PersistentActorBase
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly NotifierFactory notifierFactory;

        public SubscriptionActor()
        {
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
            LoggingAdapter.Debug("Received subscribe ack " + subscriptionMessage.SubscriptionId.Id);
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LoggingAdapter.Debug("Received delete subscription message " + subscriptionMessage.SubscriptionId.Id);
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Unsubscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(SubscriptionMessage subscription)
        {
            LoggingAdapter.Debug("Creating a subscription with id " + subscription.SubscriptionId.Id);

            if (subscriptionMessage != null)
            {
                LoggingAdapter.Debug("Dumping out of subscription for " + subscription.SubscriptionId.Id +
                    " as subscription Id wasn't null ");

                return;
            }

            LoggingAdapter.Debug("Creating docs for " + subscription.SubscriptionId.Id);

            var notifier = notifierFactory.GetNotifierFor(subscription.NotificationChannel.GetType());
            notifier.Create(subscription);

            LoggingAdapter.Debug("Persisting " + subscription.SubscriptionId.Id);
            Persist(subscription, MutateInternalState);
        }

        private void SubscribeToClusterWideBroadcastDomainEvent()
        {
            LoggingAdapter.Debug("Subscribed to cluster domain events " + subscriptionMessage.SubscriptionId.Id);

            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Subscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(DomainEvent eventToProcess)
        {
            LoggingAdapter.Debug("Received an event notification " + eventToProcess.Id + " on subscription " + subscriptionMessage.SubscriptionId.Id);
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
            LoggingAdapter.Debug("Subscription Actor. Unhandled command " + unhandledObject.GetType());
        }

        private void MutateInternalState(SubscriptionMessage message)
        {
            this.subscriptionMessage = message;
            LoggingAdapter.Debug("Persist finished " + subscriptionMessage.SubscriptionId.Id);

            SubscribeToClusterWideBroadcastDomainEvent();
        }

        private void MutateInternalState(object unhandledObject)
        {
            LoggingAdapter.Debug("Subscription Actor. Unhandled persistence command " + unhandledObject.GetType());
        }
    }
}
