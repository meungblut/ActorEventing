using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json;
using Owin;

namespace Euventing.Api.WebApi
{
    public class WebApiOwinConfiguration : IOwinConfiguration
    {
        public void Configure(IAppBuilder builder, IIocContainer resolver)
        {
            var webApiConfiguration = this.ConfigureWebApi(resolver);
            builder.UseWebApi(webApiConfiguration);
        }

        public void Cleanup(IIocContainer kernel)
        {
            DoNothing();
        }

        private HttpConfiguration ConfigureWebApi(IIocContainer resolver)
        {
            var config = new HttpConfiguration();

            MapRoutes(config);
            ReplaceControllerActivatorWithIocCOntrollerActivator(resolver, config);
            LoadMediaFormatters(resolver, config);
            LoadMessageHandlers(resolver, config);
            LoadJsonFormatters(resolver, config);

            return config;
        }

        private void DoNothing()
        {
        }

        private void LoadMessageHandlers(IIocContainer resolver, HttpConfiguration config)
        {
            var delegatingHandlers = resolver.GetAll<DelegatingHandler>();

            foreach (var delegatingHandler in delegatingHandlers)
            {
                config.MessageHandlers.Add(delegatingHandler);
            }
        }

        private static void LoadJsonFormatters(IIocContainer resolver, HttpConfiguration config)
        {
            var converters = resolver.GetAll<JsonConverter>();
            foreach (var converter in converters)
            {
                config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(converter);
            }
        }

        private static void LoadMediaFormatters(IIocContainer resolver, HttpConfiguration config)
        {
            var formatters = resolver.GetAll<MediaTypeFormatter>();
            foreach (var mediaTypeFormatter in formatters)
            {
                config.Formatters.Add(mediaTypeFormatter);
            }
        }

        private static void MapRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
        }

        private static void ReplaceControllerActivatorWithIocCOntrollerActivator(
            IIocContainer resolver,
            HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpControllerActivator), new IocHttpWebApiControllerActivator(resolver));
        }
    }
}
