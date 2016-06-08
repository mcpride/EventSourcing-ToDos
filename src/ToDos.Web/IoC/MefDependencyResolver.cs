using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Web.Http.Dependencies;

namespace ToDos.Web.IoC
{
    public sealed class MefDependencyResolver : IDependencyResolver
    {
        private readonly CompositionContainer _parentContainer;

        private MefDependencyResolver(CompositionContainer parentContainer)
        {
            _parentContainer = parentContainer;
            Initialize(parentContainer.Catalog);
        }

        public MefDependencyResolver(ComposablePartCatalog catalog)
        {
            Initialize(catalog);
        }

        private void Initialize(ComposablePartCatalog catalog)
        {
            Container = new CompositionContainer(catalog,
                CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            var batch = new CompositionBatch();
            Container.Compose(batch);
        }

        public CompositionContainer Container { get; private set; }

        #region IDependencyResolver Members

        public IDependencyScope BeginScope()
        {
            return new MefDependencyResolver(_parentContainer);
        }

        public object GetService(Type type)
        {
            var export = Container
                .GetExports(type, null, null).SingleOrDefault();

            return null != export ? export.Value : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            var exports = Container.GetExports(type, null, null);
            var exportList = new List<object>();
            // ReSharper disable PossibleMultipleEnumeration
            if (exports.Any()) exportList.AddRange(exports.Select(export => export.Value));
            // ReSharper restore PossibleMultipleEnumeration
            return exportList;
        }

        public void Dispose()
        {
            Container.Dispose();
            Container = null;
        }

        #endregion
    }
}