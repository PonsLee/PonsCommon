using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.MefDI
{
    public class MefHelper
    {
        public static CompositionContainer CreateContainer(string forlder = "")
        {
            var catalog = new SafeDirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, forlder));
            var container = new CompositionContainer(catalog);
            return container;
        }

        public static CompositionContainer InitMefComposite(object attributedPart, string forlder = "")
        {
            var container = CreateContainer(forlder);
            container.ComposeParts(attributedPart);
            return container;
        }
    }
}
