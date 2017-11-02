using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Loansv2.Startup))]
namespace Loansv2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
