using Microsoft.Owin;
using Owin;
using ToDos.Web;

[assembly: OwinStartup(typeof(Startup))]
namespace ToDos.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
