using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Api.WebApi;
using TinyIoC;

namespace Euventing.Api.Startup
{
    public abstract class EventSystemHostBase
    {
        protected readonly int AkkaHostingPort;
        protected readonly string ActorSystemName;
        protected readonly string PersistenceSectionName;
        protected readonly string AkkaSeedNodes;
        private readonly int apiHostingPort;
        protected TinyIocContainerImplementation IocContainer;
        protected WebApiSelfHost WebApiHost;
        protected readonly int EntriesPerDocument;

        protected EventSystemHostBase(
            int akkahostingPort,
            string actorSystemName,
            string persistenceSectionName,
            string seedNodes,
            int apiHostingPort,
            int entriesPerDocument)
        {
            this.EntriesPerDocument = entriesPerDocument;
            this.AkkaHostingPort = akkahostingPort;
            this.ActorSystemName = actorSystemName;
            this.PersistenceSectionName = persistenceSectionName;
            this.AkkaSeedNodes = seedNodes;
            this.apiHostingPort = apiHostingPort;

            IocContainer = new TinyIocContainerImplementation(new TinyIoCContainer());

            ConfigureIoc();
        }

        protected abstract void ConfigureIoc();

        public void Start()
        {
            WebApiHost = new WebApiSelfHost(apiHostingPort, IocContainer);
            WebApiHost.Start();
        }

        public T Get<T>()
        {
            return (T)IocContainer.GetService(typeof(T));
        }

        public void Stop()
        {
            WebApiHost.Stop();
        }
    }
}
