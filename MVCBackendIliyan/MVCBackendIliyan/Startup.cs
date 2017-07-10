using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCBackendIliyan.Startup))]
namespace MVCBackendIliyan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
