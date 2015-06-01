using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PonsUtil.Security;

namespace Ado.Net
{
    public class ConnectionStrings
    {
        public static string DefaultExecuteConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["defailt"].ToString().Trim()); }
        }

        public static string AbcExecuteConnString
        {
            get { return DES.Decrypt3DES(ConfigurationManager.ConnectionStrings["abc"].ToString().Trim()); }
        }
    }
}
