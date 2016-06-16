using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Owin;

namespace Eventing.Api.WebApi
{
    public class OwinStartup
    {
        private readonly string uri;
        private readonly IIocContainer iocResolver;
        private IDisposable app;
        private IEnumerable<IOwinConfiguration> startupConfigurationStrategies;

        public OwinStartup(string uri, IIocContainer iocResolver)
        {
            this.uri = uri;
            this.iocResolver = iocResolver;
        }

        public void Start()
        {
            this.app = WebApp.Start(this.uri, this.Configure);
        }

        public void Stop()
        {
            foreach (var owinAppBuilderDecoratorConfiguration in this.startupConfigurationStrategies)
            {
                owinAppBuilderDecoratorConfiguration.Cleanup(this.iocResolver);
            }

            this.app.Dispose();
        }

        private void Configure(IAppBuilder appBuilder)
        {
            this.RunStartupStrategies(appBuilder);
        }

        private void RunStartupStrategies(IAppBuilder appBuilder)
        {
            this.startupConfigurationStrategies = this.GetConfigurationObjects();

            foreach (var owinAppBuilderDecoratorConfiguration in this.startupConfigurationStrategies)
            {
                owinAppBuilderDecoratorConfiguration.Configure(appBuilder, this.iocResolver);
            }
        }

        private IEnumerable<IOwinConfiguration> GetConfigurationObjects()
        {
            IEnumerable<IOwinConfiguration> configurationObjects = this.iocResolver.GetAll<IOwinConfiguration>();

            if (!configurationObjects.Any())
            {
                throw new NoStartupConfigurationsException("Nothing registered in " + configurationObjects.GetType());
            }
            return configurationObjects;
        }
    }
}
