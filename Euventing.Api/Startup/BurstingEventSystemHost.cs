﻿using Euventing.Api.WebApi;
using Euventing.Atom;
using Euventing.Atom.Burst;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
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
            var factory = new ShardedAtomFeedFactory(actorSystem, new ConfigurableAtomDocumentSettings(1000));

            var subscriptionManager = new BurstSubscriptionManager(actorSystem, factory);
            var eventPublisher = new BurstableEventPublisher(actorSystem);
            var loggingEventPublisher = new LoggingEventPublisherDecorator(eventPublisher);

            IocContainer.Register<ISubscriptionManager>(subscriptionManager);
            IocContainer.Register<IEventPublisher>(loggingEventPublisher);

            var documentRetriever = new BurstAtomDocumentRetriever(factory);
            IocContainer.Register<IAtomDocumentRetriever>(documentRetriever);

            IocContainer.RegisterMultiple<IOwinConfiguration, WebApiOwinConfiguration>(IocLifecycle.PerRequest);
        }
    }
}
