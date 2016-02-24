using System;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Persistence;
using Euventing.Core.Messages;
using Euventing.Core.Notifications;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionActor : PersistentActor
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly NotifierFactory notifierFactory;

        public override string PersistenceId { get; }

        public SubscriptionActor()
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
            notifierFactory = new NotifierFactory();
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SubscriptionMessage)
                this.subscriptionMessage = (SubscriptionMessage)message;

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        private void Process(SubscribeAck eventToProcess)
        {

        }

        private void Process(SubscriptionMessage subscriptionMessage)
        {
            this.subscriptionMessage = subscriptionMessage;
            SubscribeToClusterWideBroadcastDomainEvent();
        }

        private void SubscribeToClusterWideBroadcastDomainEvent()
        {
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Subscribe("publishedEventsTopic", Self), Self);
        }

        private void Process(DomainEvent eventToProcess)
        {
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
    }
}
