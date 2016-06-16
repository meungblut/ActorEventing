using Owin;

namespace Eventing.Api.WebApi
{
    public interface IOwinConfiguration
    {
        void Configure(IAppBuilder builder, IIocContainer kernel);

        void Cleanup(IIocContainer kernel);
    }
}
