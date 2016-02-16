using System;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Persistence;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionActor : PersistentActor
    {
        private SubscriptionMessage subscriptionMessage;

        public SubscriptionActor()
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
        }

        //recovery stream
        protected override bool ReceiveRecover(object message)
        {
            if (message is SubscriptionMessage)
                this.subscriptionMessage = (SubscriptionMessage)message;

            return true;
        }

        //Inbound messages stream
        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                return false;
            }
            if (message is DomainEvent)
            {
                Console.Write("************************* " + message.GetType().ToString());
                string s = "";
            }
            else if (message is SubscriptionQuery)
            {
                if (subscriptionMessage == null)
                    Sender.Tell(new NullSubscription(), Context.Self);
                else
                    Sender.Tell(subscriptionMessage, Context.Self); 
            }
            else if (message is SubscriptionMessage)
            {
                this.subscriptionMessage = (SubscriptionMessage) message;
                var mediator = DistributedPubSub.Get(Context.System).Mediator;
                mediator.Tell(new Subscribe("publishedEventsTopic", Self), Self);
            }
            else if (message is SubscribeAck)
            {
                
            }
            else
            {

                return false;
            }
            return true;
        }

        public override string PersistenceId { get; }
    }
}
