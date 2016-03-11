using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Atom.Document.Actors.ShardSupport.Document;
using Euventing.Core;
using Euventing.Core.Startup;
using TinyIoC;

namespace Euventing.Api.Startup
{
    public class EventSystemHost
    {
        private readonly int akkaHostingPort;
        private readonly string actorSystemName;
        private readonly string persistenceSectionName;
        private readonly string akkaSeedNodes;
        private readonly int apiHostingPort;
        private TinyIocContainerImplementation iocContainer;
        private WebApiSelfHost webApiHost;

        public EventSystemHost(
            int akkahostingPort, 
            string actorSystemName, 
            string persistenceSectionName, 
            string seedNodes,
            int apiHostingPort)
        {
            this.akkaHostingPort = akkahostingPort;
            this.actorSystemName = actorSystemName;
            this.persistenceSectionName = persistenceSectionName;
            this.akkaSeedNodes = seedNodes;
            this.apiHostingPort = apiHostingPort;
        }

        public void Start()
        {
            iocContainer = new TinyIocContainerImplementation(new TinyIoCContainer());

            var actorSystemFactory = new ShardedActorSystemFactory(akkaHostingPort, actorSystemName, persistenceSectionName, akkaSeedNodes);
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

            webApiHost = new WebApiSelfHost(apiHostingPort, iocContainer);
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
