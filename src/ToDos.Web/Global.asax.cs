﻿using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using ToDos.Web.Injection.Installers;
using ToDos.Web.Injection.WebAPI;

namespace ToDos.Web
{
    // For Castle.Windsor WebApi Injection resolver
    // thanks to: http://blog.ploeh.dk/2012/10/03/DependencyInjectioninASP.NETWebAPIwithCastleWindsor/

    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly IWindsorContainer container;

        public WebApiApplication()
        {
            this.container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
        }

        public override void Dispose()
        {
            this.container.Dispose();
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
            // Configure all AutoMapper Profiles
            AutoMapperConfig.Configure();
            // Install DI mapper


            this.container.Install(
                        new MappersInstaller(),
                        new CommandStackInstaller(),
                        new QueryStackInstaller(),
                        new QueryStackPollingClientInstaller(),
                        new EventStoreInstaller(),
                        new ControllersInstaller(),
                        new LegacyMigrationInstaller()
                        );

            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(this.container));

            // Use of static reference to Di container to pass to SignalR or other Owin Module (e.g. Hangfire).
            // I don't like this approach because it forces to expose the DI container as public throughout all 
            // the application, and it's not what I want to do
            HostingEnvironment.Name = "AspNET";
            HostingEnvironment.DIContainer = container;

            //Create Event Store
            container.Resolve<IStoreEvents>();
        }

    }

    internal static class HostingEnvironment
    {
        public static string Name { get; set; }
        public static IWindsorContainer DIContainer { get; set; }
    }
}