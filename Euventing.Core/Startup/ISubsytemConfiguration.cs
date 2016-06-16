using Akka.Actor;

namespace Eventing.Core.Startup
{
    public interface ISubsytemConfiguration
    {
        void Configure(ActorSystem actorSystem);
    }
}
