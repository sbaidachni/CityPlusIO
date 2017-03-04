using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReportingWebApp.Startup))]
namespace ReportingWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
