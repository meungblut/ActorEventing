using Owin;

namespace Euventing.Api.WebApi
{
    public interface IOwinConfiguration
    {
        void Configure(IAppBuilder builder, IIocContainer kernel);

        void Cleanup(IIocContainer kernel);
    }
}
