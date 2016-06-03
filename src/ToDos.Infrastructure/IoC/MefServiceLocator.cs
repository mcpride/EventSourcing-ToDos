using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace ToDos.Infrastructure.IoC
{
    public class MefServiceLocator : ServiceLocatorImplBase
    {
        private readonly ExportProvider _provider;

        public static Func<string> ErrorCannotLocateInstancesOfContract =
            () => "Could not locate any instances of contract {0}";

        public MefServiceLocator(ExportProvider provider)
        {
            _provider = provider;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key == null)
            {
                key = AttributedModelServices.GetContractName(serviceType);
            }

            var exports = _provider.GetExports<object>(key);

            // ReSharper disable PossibleMultipleEnumeration
            if (exports.Any())
            {
                return exports.First().Value;
            }
            // ReSharper restore PossibleMultipleEnumeration

            throw new ActivationException(string.Format(ErrorCannotLocateInstancesOfContract(), key));
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var exports = _provider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return exports;
        }
    }
}