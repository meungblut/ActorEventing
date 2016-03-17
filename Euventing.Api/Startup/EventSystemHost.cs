using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors.ShardSupport.Document;
using Euventing.Atom.Logging;
using Euventing.Core;
using Euventing.Core.Logging;
using Euventing.Core.Publishing;
using Euventing.Core.Subscriptions;
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
        private readonly int entriesPerDocument;

        public EventSystemHost(
            int akkahostingPort, 
            string actorSystemName, 
            string persistenceSectionName, 
            string seedNodes,
            int apiHostingPort,
            int entriesPerDocument)
        {
            this.entriesPerDocument = entriesPerDocument;
            this.akkaHostingPort = akkahostingPort;
            this.actorSystemName = actorSystemName;
            this.persistenceSectionName = persistenceSectionName;
            this.akkaSeedNodes = seedNodes;
            this.apiHostingPort = apiHostingPort;

            ConfigureIoc();

        }

        private void ConfigureIoc()
        {
            iocContainer = new TinyIocContainerImplementation(new TinyIoCContainer());

            var actorSystemFactory = new ShardedActorSystemFactory(akkaHostingPort, actorSystemName, persistenceSectionName, akkaSeedNodes);
            var actorSystem = actorSystemFactory.GetActorSystem();

            var subscriptionManager = new SingleShardedSubscriptionManager(actorSystem);
            var eventPublisher = new DistributedPubSubEventPublisher(actorSystem);
            var loggingEventPublisher = new LoggingEventPublisherDecorator(eventPublisher);

            ShardedAtomDocumentFactory atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            ShardedAtomFeedFactory atomFeedFactory = new ShardedAtomFeedFactory(actorSystem, atomDocumentFactory, new ConfigurableAtomDocumentSettings(entriesPerDocument));

            var settings = new AtomNotificationSettings(atomFeedFactory);

            var atomRetriever = new AtomDocumentRetriever(atomFeedFactory, atomDocumentFactory);
            var loggingAtomRetriever = new LoggingAtomDocumentRetrieverDecorator(atomRetriever);

            iocContainer.Register<SingleShardedSubscriptionManager>(subscriptionManager);
            iocContainer.Register<IEventPublisher>(loggingEventPublisher);
            iocContainer.Register<ShardedAtomDocumentFactory>(atomDocumentFactory);
            iocContainer.Register<ShardedAtomFeedFactory>(atomFeedFactory);
            iocContainer.Register<IAtomDocumentRetriever>(loggingAtomRetriever);

            iocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);
        }

        public void Start()
        {
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
