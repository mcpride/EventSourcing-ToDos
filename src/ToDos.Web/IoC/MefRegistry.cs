using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Web.Http.Controllers;
using AutoMapper;
using MS.EventSourcing.Infrastructure.EventHandling;
using MS.EventSourcing.Infrastructure.MassTransit;
using ToDos.Web.Controllers;
using ToDos.Web.Worker;

namespace ToDos.Web.IoC
{
    public static class MefRegistry
    {
        public static RegistrationBuilder Registrate(this RegistrationBuilder registration)
        {
            RegisterMappers(registration);
            RegisterControllers(registration);
            return registration;
        }

        public static ComposablePartCatalog GetCatalog(this RegistrationBuilder registration)
        {
            return new AggregateCatalog(
                new AssemblyCatalog(typeof(DomainEvent).Assembly, registration),
                new AssemblyCatalog(typeof(Bus).Assembly, registration),
                new AssemblyCatalog(typeof(ToDoListController).Assembly, registration),
                new AssemblyCatalog(typeof(Mapper).Assembly, registration)
                );
        }

        private static void RegisterMappers(RegistrationBuilder registration)
        {
            registration.ForType(typeof(Mapper)).ExportProperties<IMappingEngine>(info => info.Name == "Engine");
        }

        private static void RegisterControllers(RegistrationBuilder registration)
        {
            registration.ForTypesDerivedFrom<IHttpController>()
                .SetCreationPolicy(CreationPolicy.NonShared).Export().ExportInterfaces();

            registration.ForType<ToDoWorker>()
                .SetCreationPolicy(CreationPolicy.NonShared).Export();
        }
    }
}