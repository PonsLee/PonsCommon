using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace PonsUtil.MefDI
{
    public class MefServiceLocatorBase : ServiceLocatorImplBase
    {
        private readonly ExportProvider _provider;

        public MefServiceLocatorBase(ExportProvider provider)
        {
            _provider = provider;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key == null)
                key = AttributedModelServices.GetContractName(serviceType);

            var exports = _provider.GetExports<object>(key).ToArray();

            if (exports.Any())
                return exports.First().Value;

            throw new ActivationException(string.Format("Could not locate any instances of contract {0}", key));
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var exports = _provider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return exports;
        }
    }
}