using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.Caching
{
    /// <summary>
    /// 缓存策略接口
    /// </summary>
    public interface ICacheStrategy
    {
        TimeSpan DefaultExpires { get; set; }
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan timeSpan);
        void Set(string key, object value, DateTime dateTime);
        T Get<T>(string key);
        object Remove(string key);
        T Remove<T>(string key);
    }
}
