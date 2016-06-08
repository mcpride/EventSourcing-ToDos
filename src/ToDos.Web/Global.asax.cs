using System.ComponentModel.Composition.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ToDos.Web.App_Start;

namespace ToDos.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public override void Dispose()
        {
            HostingEnvironment.DIContainer = null;
            base.Dispose();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = MefConfig.Configure(GlobalConfiguration.Configuration);
            // Configure all AutoMapper Profiles
            AutoMapperConfig.Configure();
            // Install DI mapper


            // Use of static reference to Di container to pass to SignalR or other Owin Module (e.g. Hangfire).
            // I don't like this approach because it forces to expose the DI container as public throughout all 
            // the application, and it's not what I want to do
            HostingEnvironment.Name = "AspNET";
            HostingEnvironment.DIContainer = container;

            //Create Event Store
            //container.Resolve<IStoreEvents>();
        }

    }

    internal static class HostingEnvironment
    {
        public static string Name { get; set; }
        public static CompositionContainer DIContainer { get; set; }
    }
}