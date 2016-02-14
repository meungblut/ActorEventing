using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public class SubscriptionActor : PersistentActor
    {
        protected override bool ReceiveRecover(object message)
        {
            throw new NotImplementedException();
        }

        protected override bool ReceiveCommand(object message)
        {
            throw new NotImplementedException();
        }

        public override string PersistenceId { get; }
    }
}
