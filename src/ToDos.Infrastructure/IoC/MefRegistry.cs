using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using MassTransit;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EF;
using MS.EventSourcing.Infrastructure.EventHandling;
using ToDos.Domain.CommandHandling;
using ToDos.Domain.EventHandling;
using ToDos.Infrastructure.Domain;
using ToDos.Infrastructure.Domain.CommandHandling;
using ToDos.Infrastructure.EventHandling;
using Bus = MS.EventSourcing.Infrastructure.MassTransit.Bus;

namespace ToDos.Infrastructure.IoC
{
    public static class MefRegistry
    {
        public static RegistrationBuilder Registrate(this RegistrationBuilder registration)
        {
            RegisterEventStore(registration);
            RegisterToDosContext(registration);
            RegisterToDosService(registration);

            return registration;
        }

        public static ComposablePartCatalog GetCatalog(this RegistrationBuilder registration)
        {
            return new AggregateCatalog(
                new AssemblyCatalog(typeof(IToDosEventHandler).Assembly, registration),
                new AssemblyCatalog(typeof(ToDosContext).Assembly, registration),
                new AssemblyCatalog(typeof(DomainEvent).Assembly, registration),
                new AssemblyCatalog(typeof(Bus).Assembly, registration),
                new AssemblyCatalog(typeof(EventStore).Assembly, registration)
                );
        }

        private static void RegisterEventStore(RegistrationBuilder registration)
        {
            RegisterStores(registration);

            registration.ForType<DomainRepository>()
                .SelectConstructor(
                    builder => new DomainRepository(builder.Import<IEventStore>(), builder.Import<ISnapshotStore>()))
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();
        }

        private static void RegisterStores(RegistrationBuilder registration)
        {
            RegisterInMemoryStores(registration);
            //RegisterDbStores(registration);
        }

        private static void RegisterInMemoryStores(RegistrationBuilder registration)
        {
            registration.ForType<InMemoryEventStore>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();

            registration.ForType<InMemorySnapshotStore>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();
        }

        private static void RegisterDbStores(RegistrationBuilder registration)
        {
            registration.ForType<EventStore>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();

            registration.ForType<SnapshotStore>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();
        }

        private static void RegisterToDosContext(RegistrationBuilder registration)
        {
            registration.ForType<ToDosCommandBus>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();

            registration.ForType<ToDosEventBus>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();

            registration.ForType<ToDosContext>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export()
                .ExportInterfaces();

            registration.ForTypesDerivedFrom<IToDosCommandHandler>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export<IConsumer>(builder => builder
                    .AddMetadata("ContractType", type => type));

            registration.ForTypesDerivedFrom<IToDosEventHandler>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export<IConsumer>(builder => builder
                    .AddMetadata("ContractType", type => type));
        }

        private static void RegisterToDosService(RegistrationBuilder registration)
        {
            registration.ForType<ToDosService>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export()
                .ExportInterfaces();
        }
    }
}