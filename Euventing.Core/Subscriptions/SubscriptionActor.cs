﻿using System;
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
            if (message is SubscriptionQuery)
            {
                if (subscriptionMessage == null)
                    Sender.Tell(new NullSubscription(), Context.Self);
                else
                    Sender.Tell(subscriptionMessage, Context.Self); 
            }

            if (message is SubscriptionMessage)
            {
                this.subscriptionMessage = (SubscriptionMessage) message;
            }
            if (message as string == "snap")
                SaveSnapshot(this);
            else
                return false;

            return true;
        }

        public override string PersistenceId { get; }
    }
}
