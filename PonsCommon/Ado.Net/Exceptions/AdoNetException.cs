using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ado.Net.Exceptions
{
    public class AdoNetException : Exception
    {
        public AdoNetException()
        {
        }

        public AdoNetException(string message)
            : base(message)
        {
        }
    }
}
