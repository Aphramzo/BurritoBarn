using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BurritoBarn.Startup))]
namespace BurritoBarn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
