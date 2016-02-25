using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Euventing.Api.WebApi
{
    public class IocHttpWebApiControllerActivator : IHttpControllerActivator
    {
        private readonly IIocContainer container;

        public IocHttpWebApiControllerActivator(IIocContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller =
                (IHttpController)this.container.GetService(controllerType);

            return controller;
        }
    }
}
