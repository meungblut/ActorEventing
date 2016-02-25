using System;

namespace Euventing.Api.WebApi
{
    public class WebApiSelfHost
    {
        internal readonly Uri BaseAddress;
        private static OwinStartup selfHostStarter;

        private readonly int portToHostOn;

        private readonly IIocContainer iocContainer;

        public WebApiSelfHost(int port, IIocContainer container)
        {
            this.iocContainer = container;
            this.portToHostOn = port;
            this.BaseAddress = new Uri("http://localhost:" + port);
        }

        public void Start()
        {
            selfHostStarter = new OwinStartup("http://+:" + this.portToHostOn, this.iocContainer);
            selfHostStarter.Start();
        }

        public void Stop()
        {
            selfHostStarter.Stop();
        }
    }
}
