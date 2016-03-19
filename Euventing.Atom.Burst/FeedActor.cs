using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;

namespace Euventing.Atom.Burst
{
    public class FeedActor : PersistentActor
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
