using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(UvlotExt.Startup))]
[assembly: OwinStartupAttribute("UvlotExtConfig", typeof(UvlotExt.Startup))]
namespace UvlotExt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
