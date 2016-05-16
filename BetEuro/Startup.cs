using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BetEuro.Startup))]
namespace BetEuro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
