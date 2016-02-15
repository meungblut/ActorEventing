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
        private SubscriptionMessage message;

        //recovery stream
        protected override bool ReceiveRecover(object message)
        {
            throw new NotImplementedException();
        }

        //Inbound messages stream
        protected override bool ReceiveCommand(object message)
        {
            if (message as string == "snap")
                SaveSnapshot(this);
            else
                return false;

            return true;
        }

        public override string PersistenceId { get; }
    }
}
