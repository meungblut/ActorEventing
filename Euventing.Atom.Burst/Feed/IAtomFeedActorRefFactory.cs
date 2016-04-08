using Akka.Actor;

namespace Euventing.Atom.Burst.Feed
{
    public interface IAtomFeedActorRefFactory
    {
        IActorRef GetActorRef(IActorContext context);
    }
}