using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Reflection;
using Magnum.Extensions;
using MassTransit.Integration.Composition;
using Microsoft.Practices.ServiceLocation;
using ToDos.Domain;
using ToDos.Domain.CommandHandling;
using ToDos.Domain.EventHandling;

namespace ToDos.Infrastructure.IoC
{
    public class MefBootstrapper<TRootModel> : Bootstrapper<TRootModel>
    {
        protected ExportProvider Provider;

        // ReSharper disable EmptyConstructor 
        // ReSharper disable RedundantBaseConstructorCall
        public MefBootstrapper() : base()
        {
        }

        public MefBootstrapper(Assembly executingAssembly) : base(executingAssembly)
        {
        }

        // ReSharper restore RedundantBaseConstructorCall
        // ReSharper restore EmptyConstructor

        protected override void Configure(Assembly executingAssembly)
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new RegistrationBuilder().Registrate().GetCatalog());

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            var batch = new CompositionBatch();

            BatchConfig(container, batch);

            container.Compose(batch);
            container.SatisfyImportsOnce(this);
            Provider = container;
            ServiceLocator.SetLocatorProvider(() => new MefServiceLocator(container)); // provide common service locator, if needed
        }

        protected virtual void BatchConfig(CompositionContainer container, CompositionBatch batch)
        {
            Func<IToDosContext> toDosContextFunc = () =>
            {
                var context = GetExportedValue<IToDosContext>();
                context.InitializeLoopback(sbc => sbc.LoadFrom(container, type => (type.Implements<IToDosCommandHandler>() || type.Implements<IToDosEventHandler>()))); // register specific handlers for ToDos context to MassTransit 
                //context.Initialize(sbc => sbc.LoadFrom(container, type => type.Implements<IToDosCommandHandler>()), sbc => sbc.LoadFrom(container, type => type.Implements<IToDosEventHandler>())); // register specific command and event handlers for ToDos context to MassTransit 
                return context;
            };
            batch.AddExportedValue(new Tuple<Func<IToDosContext>>(toDosContextFunc)); //Tuple envelope is a stupid workaround
        }

        protected override T GetExportedValue<T>()
        {
            return Provider.GetExportedValue<T>();
        }
    }
}