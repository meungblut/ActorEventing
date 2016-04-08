using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Feed
{
    public class UnshardedAtomFeedActorRefFactory : IAtomFeedActorRefFactory
    {
        public UnshardedAtomFeedActorRefFactory(IAtomDocumentSettings atomDocumentSettings)
        {
            
        }
        public IActorRef GetActorRef(IActorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
