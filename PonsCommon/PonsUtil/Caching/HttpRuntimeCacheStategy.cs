using System;
using System.Web;
using System.Web.Caching;


namespace PonsUtil.Caching
{
    public class HttpRuntimeCacheStategy : ICacheStrategy
    {
        private static readonly HttpRuntimeCacheStategy instance = new HttpRuntimeCacheStategy();
        protected static volatile Cache webCache = HttpRuntime.Cache;

        #region ICacheStrategy 成员

        public TimeSpan _DefaultExpires = TimeSpan.Zero;
        public TimeSpan DefaultExpires
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

        public void Set(string key, object value)
        {
            this.Set(key, value, _DefaultExpires);
        }

        public void Set(string key, object value, TimeSpan timeSpan)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            var callBack = new CacheItemRemovedCallback(onRemove);
            webCache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.Zero);
        }

        public void Set(string key, object value, DateTime dateTime)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            var callBack = new CacheItemRemovedCallback(onRemove);
            webCache.Insert(key, value, null, dateTime, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public T Get<T>(string key)
        {
            return (T)webCache.Get(key);
        }

        public object Remove(string key)
        {
            return webCache.Remove(key);
        }

        public T Remove<T>(string key)
        {
            return (T)webCache.Remove(key);
        }

        #endregion

        public void onRemove(string key, object val, CacheItemRemovedReason reason)
        {
            switch (reason)
            {
                case CacheItemRemovedReason.DependencyChanged:
                    break;
                case CacheItemRemovedReason.Expired:
                    {
                        break;
                    }
                case CacheItemRemovedReason.Removed:
                    {
                        break;
                    }
                case CacheItemRemovedReason.Underused:
                    {
                        break;
                    }
                default: break;
            }

        }
    }
}
