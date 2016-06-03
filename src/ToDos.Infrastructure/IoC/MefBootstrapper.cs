using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Reflection;
using Magnum.Extensions;
using MassTransit.Integration.Composition;
using MassTransit.SubscriptionConfigurators;
using Microsoft.Practices.ServiceLocation;
using ToDos.Infrastructure.Domain;
using ToDos.Infrastructure.EventHandling;

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

            //var assemblyCatalog = new AssemblyCatalog(executingAssembly);
            //catalog.Catalogs.Add(assemblyCatalog);
            //var assemblyPath = Path.GetDirectoryName(executingAssembly.Location);
            //// ReSharper disable AssignNullToNotNullAttribute
            //var assemblyDirectoryCatalog = new DirectoryCatalog(assemblyPath);
            //// ReSharper restore AssignNullToNotNullAttribute
            //catalog.Catalogs.Add(assemblyDirectoryCatalog);

            var registryCatalog = new RegistrationBuilder().Registrate().GetCatalog();

            catalog.Catalogs.Add(registryCatalog);

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            var batch = new CompositionBatch();

            BatchConfig(container, batch);

            //batch.AddExportedValue(container);
            container.Compose(batch);
            container.SatisfyImportsOnce(this);
            Provider = container;
            ServiceLocator.SetLocatorProvider(() => new MefServiceLocator(container));
        }

        protected virtual void BatchConfig(CompositionContainer container, CompositionBatch batch)
        {
            //Func<IToDosContext> toDosContextFunc = () =>
            //{
            //    var context = container.GetExportedValue<IToDosContext>();
            //    context.Initialize(sbc => sbc.LoadFrom(container, type => type.Implements<IToDosEventHandler>()));
            //    return context;
            //};

            //batch.AddExportedValue(toDosContextFunc);
            batch.AddPart(this);
        }

        [Export(typeof(Func<IToDosContext>))]
        public IToDosContext CreateToDosContext()
        {
            var context = Provider.GetExportedValue<IToDosContext>();
            context.Initialize(sbc => sbc.LoadFrom(Provider, type => type.Implements<IToDosEventHandler>()));
            return context;
        }

        protected override T GetExportedValue<T>()
        {
            return Provider.GetExportedValue<T>();
        }
    }
}