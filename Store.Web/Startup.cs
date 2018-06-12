using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Store.Web.Startup))]
namespace Store.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
