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
            ShardedAtomDocumentFactory atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            ShardedAtomFeedFactory atomFeedFactory = new ShardedAtomFeedFactory(actorSystem, atomDocumentFactory);
            var settings = new AtomNotificationSettings(atomFeedFactory);

            var atomRetriever = new AtomDocumentRetriever(atomFeedFactory, atomDocumentFactory);

            iocContainer.Register<SubscriptionManager>(subscriptionManager);
            iocContainer.Register<EventPublisher>(eventPublisher);
            iocContainer.Register<ShardedAtomDocumentFactory>(atomDocumentFactory);
            iocContainer.Register<ShardedAtomFeedFactory>(atomFeedFactory);
            iocContainer.Register<AtomDocumentRetriever>(atomRetriever);

            iocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);

            webApiHost = new WebApiSelfHost(3600, iocContainer);
            webApiHost.Start();
        }

        public T Get<T>()
        {
            return (T)iocContainer.GetService(typeof(T));
        }

        public void Stop()
        {
            webApiHost.Stop();
        }
    }
}
