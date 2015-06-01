using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PonsUtil
{
    /// <summary>
    /// dump the content of an object using JSON format.
    /// </summary>
    public class JsonObjectFormatter : ICustomFormatter
    {
        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            try
            {
                return JsonConvert.SerializeObject(arg);
            }
            catch (Exception e)
            {
                try
                {
                    return JsonConvert.SerializeObject(e);
                }
                catch { }
                return "\"Error in dump object.\"";
            }
        }
        #endregion
    }
}
