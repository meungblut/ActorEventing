using Akka.Actor;

namespace Eventing.Atom.Burst.Feed
{
    public interface IAtomFeedActorRefFactory
    {
        IActorRef GetActorRef(IActorContext context);
    }
}