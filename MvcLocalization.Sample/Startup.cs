using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcLocalization.Sample.Startup))]
namespace MvcLocalization.Sample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
