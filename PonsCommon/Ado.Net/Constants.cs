using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ado.Net
{
    public class Constants
    {
        public const string NC_UNITOFWORK_TRANSACTION_CONNECTION = "NC_UNITOFWORK_TRANSACTION_CONNECTION";

        public const string NC_UNITOFWORK_STACK = "NC_UNITOFWORK_STACK";

        /// <summary>
        /// NC_UNITOFWORK_TRANSACTION_CONNECTION Thread
        /// </summary>
        public static object NcUnitofworkTransactionConnectionThread
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }

        public static object NcUnitofworkStackThread
        {
            get { return NC_UNITOFWORK_STACK + Thread.CurrentThread.ManagedThreadId; }
        }
    }
}
