using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Core.Messages;
using Euventing.Core.Subscriptions;

namespace Euventing.Atom.Burst
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private ActorSystem system;
        const string localSubscriptionNode = "subscriptionManager";

        public SubscriptionManager(ActorSystem actorSystem)
        {
            system = actorSystem;
        }

        public void CreateSubscription(SubscriptionMessage subscriptionMessage)
        {
        }

        public void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
