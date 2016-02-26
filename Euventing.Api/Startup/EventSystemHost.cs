using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Atom.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.Startup;
using TinyIoC;

namespace Euventing.Api.Startup
{
    public class EventSystemHost
    {
        private static TinyIocContainerImplementation iocContainer;
        private static WebApiSelfHost webApiHost;

        public void Start()
        {
            iocContainer = new TinyIocContainerImplementation(new TinyIoCContainer());

            var actorSystemFactory = new ShardedActorSystemFactory(6483, "akkaSystem", "sqlite", "127.0.0.1:6483");
            var actorSystem = actorSystemFactory.GetActorSystem();

            var subscriptionManager = new SubscriptionManager(actorSystem);
            var eventPublisher = new EventPublisher(actorSystem);
            ShardedAtomFeedFactory atomFeedFactory = new ShardedAtomFeedFactory(actorSystem);
            ShardedAtomDocumentFactory atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            var settings = new AtomNotificationSettings(atomFeedFactory);

            iocContainer.Register<SubscriptionManager>(subscriptionManager);
            iocContainer.Register<EventPublisher>(eventPublisher);
            iocContainer.Register<ShardedAtomDocumentFactory>(atomDocumentFactory);
            iocContainer.Register<ShardedAtomFeedFactory>(atomFeedFactory);

            iocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);

            webApiHost = new WebApiSelfHost(3600, iocContainer);
            webApiHost.Start();
        }

        public void Stop()
        {
            webApiHost.Stop();
        }
    }
}
