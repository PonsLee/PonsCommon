using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.Caching
{
    public abstract class BaseCacheStrategy : ICacheStrategy
    {
        public TimeSpan _DefaultExpires = TimeSpan.Zero;
        public virtual TimeSpan DefaultExpires
        {
            get
            {
                return _DefaultExpires;
            }
            set
            {
                _DefaultExpires = value;
            }
        }

        public abstract void Set(string key, object value);

        public abstract void Set(string key, object value, TimeSpan timeSpan);

        public abstract void Set(string key, object value, DateTime dateTime);

        public abstract T Get<T>(string key);

        public abstract object Remove(string key);

        public abstract T Remove<T>(string key);
    }
}
