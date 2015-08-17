using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WriteIO.Startup))]
namespace WriteIO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
