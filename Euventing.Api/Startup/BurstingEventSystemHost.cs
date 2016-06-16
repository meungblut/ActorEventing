using Eventing.Api.WebApi;
using Eventing.Atom;
using Eventing.Atom.Burst;
using Eventing.Atom.Burst.Feed;
using Eventing.Atom.Burst.Subscription;
using Eventing.Atom.Document;
using Eventing.Atom.Logging;
using Eventing.Core;
using Eventing.Core.Logging;
using Eventing.Core.Subscriptions;
using AtomDocumentRetriever = Eventing.Atom.Burst.AtomDocumentRetriever;

namespace Eventing.Api.Startup
{
    public class BurstingEventSystemHost : EventSystemHostBase
    {
        public BurstingEventSystemHost(int akkahostingPort, string actorSystemName, string persistenceSectionName, string seedNodes, int apiHostingPort, int entriesPerDocument) : 
            base(akkahostingPort, actorSystemName, persistenceSectionName, seedNodes, apiHostingPort, entriesPerDocument)
        {
        }

        protected override void ConfigureIoc()
        {
            var actorSystemFactory = new ShardedActorSystemFactory(AkkaHostingPort, ActorSystemName, PersistenceSectionName, AkkaSeedNodes);
            var actorSystem = actorSystemFactory.GetActorSystem();
            var atomSettings = new ConfigurableAtomDocumentSettings(EntriesPerDocument);

            var subscriptionManager = new SubscriptionManager(actorSystem, atomSettings);
            var eventPublisher = new EventPublisher(actorSystem);
            var loggingEventPublisher = new LoggingEventPublisherDecorator(eventPublisher);
            IocContainer.Register<IEventPublisher>(loggingEventPublisher);

            IocContainer.Register<ISubscriptionManager>(subscriptionManager);

            var documentRetriever = new LoggingAtomDocumentRetrieverDecorator(new AtomDocumentRetriever(subscriptionManager, actorSystem.Log, new InMemoryAtomDocumentRepository()));
            IocContainer.Register<IAtomDocumentRetriever>(documentRetriever);

            IocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);
        }
    }
}
