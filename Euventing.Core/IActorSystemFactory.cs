using Akka.Actor;

namespace Eventing.Core
{
    public interface IActorSystemFactory
    {
        ActorSystem GetActorSystem();
    }
}
