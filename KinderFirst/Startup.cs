using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KinderFirst.Startup))]
namespace KinderFirst
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
