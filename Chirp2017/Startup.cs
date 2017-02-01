using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Chirp2017.Startup))]
namespace Chirp2017
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
