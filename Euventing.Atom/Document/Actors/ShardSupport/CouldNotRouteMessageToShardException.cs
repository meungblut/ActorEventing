using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document.Actors.ShardSupport
{
    public class CouldNotRouteMessageToShardException : Exception
    {
        public CouldNotRouteMessageToShardException(object shardRouter, object unroutableMessage) 
            : base(string.Format("Could not route message {0} from message extractor {1}", unroutableMessage.GetType(), shardRouter.GetType()))
        {
        }
    }
}
