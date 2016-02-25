using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Core;
using Euventing.Core.Startup;
using TinyIoC;

namespace Euventing.Api.Startup
{
    public class EventSystemHost
    {
        private static TinyIocContainerImplementation iocContainer;
        private static WebApiSelfHost webApiHost;

        public static void Start()
        {
            iocContainer = new TinyIocContainerImplementation(new TinyIoCContainer());

            var actorSystemFactory = new ShardedActorSystemFactory(1234, "akkaSystem",
                "sqlite", "127.0.0.1:1234");
            var actorSystem = actorSystemFactory.GetActorSystem();
            var subsystemConfig = new AtomSubsystemConfiguration();
            var eventSystemFactory = new EventSystemFactory(actorSystem, new[] { subsystemConfig });

            iocContainer.Register<EventSystemFactory>(eventSystemFactory);

            webApiHost = new WebApiSelfHost(3600, iocContainer);
            webApiHost.Start();
        }

        public static void Stop()
        {
            webApiHost.Stop();
        }
    }
}
