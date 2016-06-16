using System;
using Akka.Actor;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst.Feed
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
