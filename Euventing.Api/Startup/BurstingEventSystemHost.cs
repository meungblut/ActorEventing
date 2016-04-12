using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Atom.Burst;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Logging;
using Euventing.Core;
using Euventing.Core.Logging;
using Euventing.Core.Subscriptions;

namespace Euventing.Api.Startup
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

            var subscriptionManager = new BurstSubscriptionManager(actorSystem, atomSettings);
            var eventPublisher = new BurstableEventPublisher(actorSystem);
            var loggingEventPublisher = new LoggingEventPublisherDecorator(eventPublisher);
            IocContainer.Register<IEventPublisher>(loggingEventPublisher);

            IocContainer.Register<ISubscriptionManager>(subscriptionManager);

            var documentRetriever = new LoggingAtomDocumentRetrieverDecorator(new BurstAtomDocumentRetriever(subscriptionManager, actorSystem.Log));
            IocContainer.Register<IAtomDocumentRetriever>(documentRetriever);

            IocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);
        }
    }
}
