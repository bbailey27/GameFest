using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GameFestWebApp.Startup))]
namespace GameFestWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
