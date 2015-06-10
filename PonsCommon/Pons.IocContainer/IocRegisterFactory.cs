using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pons.IocContainer
{
    public class IocRegisterFactory
    {
        public static IocRegister Current
        {
            get { return new AutofacRegister(); }
        }
    }
}
