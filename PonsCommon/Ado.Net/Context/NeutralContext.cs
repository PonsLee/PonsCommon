using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ado.Net.Context
{
    public class NeutralContext
    {
        private static readonly Hashtable Contexts = new Hashtable();

        /// <summary>
        /// 获取key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(object key)
        {
            return Contexts[key];
        }

        /// <summary>
        /// 设置指定key的值为value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Put(object key, object value)
        {
            if (Contexts.Contains(key))
            {
                Contexts[key] = value;
            }
            else
            {
                Contexts.Add(key, value);
            }
        }

        /// <summary>
        /// 移除指定key值
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(object key)
        {
            if (Contexts.Contains(key))
            {
                Contexts.Remove(key);
            }
        }
    }
}
