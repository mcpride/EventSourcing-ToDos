using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Web.Http;
using ToDos.Web.IoC;

namespace ToDos.Web.App_Start
{
    public static class MefConfig
    {
        public static CompositionContainer Configure(HttpConfiguration config)
        {
            var resolver = new MefDependencyResolver(new RegistrationBuilder().Registrate().GetCatalog());
            config.DependencyResolver = resolver;
            return resolver.Container;
        }
    }
}