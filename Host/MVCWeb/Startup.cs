using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCWeb.Startup))]
namespace MVCWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
