using Eventing.Api.WebApi;
using Eventing.Atom;
using Eventing.Atom.Document;
using Eventing.Atom.Document.Actors.ShardSupport.Document;
using Eventing.Atom.Logging;
using Eventing.Core;
using Eventing.Core.Logging;
using Eventing.Core.Publishing;
using Eventing.Core.Subscriptions;

namespace Eventing.Api.Startup
{
    public class EventSystemHost : EventSystemHostBase
    {
        public EventSystemHost(
            int akkahostingPort,
            string actorSystemName,
            string persistenceSectionName,
            string seedNodes,
            int apiHostingPort,
            int entriesPerDocument)
            : base(akkahostingPort, actorSystemName, persistenceSectionName, seedNodes, apiHostingPort, entriesPerDocument)
        {
            
        }

        protected override void ConfigureIoc()
        {
            var actorSystemFactory = new ShardedActorSystemFactory(AkkaHostingPort, ActorSystemName, PersistenceSectionName, AkkaSeedNodes);
            var actorSystem = actorSystemFactory.GetActorSystem();

            var subscriptionManager = new ShardedSubscriptionManager(actorSystem);
            var eventPublisher = new DistributedPubSubEventPublisher(actorSystem);
            var loggingEventPublisher = new LoggingEventPublisherDecorator(eventPublisher);

            var atomDocumentFactory = new ShardedAtomDocumentFactory(actorSystem);
            var atomFeedFactory = new ShardedAtomFeedFactory(actorSystem, atomDocumentFactory, new ConfigurableAtomDocumentSettings(EntriesPerDocument));

            var settings = new AtomNotificationSettings(atomFeedFactory);

            var atomRetriever = new AtomDocumentRetriever(atomFeedFactory, atomDocumentFactory);
            var loggingAtomRetriever = new LoggingAtomDocumentRetrieverDecorator(atomRetriever);

            IocContainer.Register<ISubscriptionManager>(subscriptionManager);
            IocContainer.Register<IEventPublisher>(loggingEventPublisher);
            IocContainer.Register<ShardedAtomDocumentFactory>(atomDocumentFactory);
            IocContainer.Register<ShardedAtomFeedFactory>(atomFeedFactory);
            IocContainer.Register<IAtomDocumentRetriever>(loggingAtomRetriever);

            IocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);
        }
    }
}
