using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace PonsUtil.Caching
{
    public class CachingManager
    {
        private static ICacheStrategy cs;
        private static volatile CachingManager instance = null;
        private static object lockHelper = new object();

        static CachingManager()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("CacheStrategy"))
            {
                string CacheStrategy = ConfigurationManager.AppSettings["CacheStrategy"];
                var items = CacheStrategy.Split(',');
                cs = (ICacheStrategy)Assembly.Load(items[1]).CreateInstance(items[0]);
            }
            else
            {
                cs = new HttpRuntimeCacheStategy();
            }
            DefaultExpires = new TimeSpan(0, 3, 0);
        }


        public static CachingManager GetCachingService()
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new CachingManager();
                    }
                }
            }

            return instance;
        }



        #region ICacheStrategy 成员

        public static TimeSpan DefaultExpires
        {
            get
            {
                return cs.DefaultExpires;
            }
            set
            {
                cs.DefaultExpires = value;
            }
        }

        public static void Set(string key, object value)
        {
            lock (lockHelper)
            {
                cs.Set(key, value);
            }
        }

        public static void Set(string key, object value, TimeSpan timeSpan)
        {
            lock (lockHelper)
            {
                cs.Set(key, value, timeSpan);
            }
        }

        public static void Set(string key, object value, DateTime dateTime)
        {
            lock (lockHelper)
            {
                cs.Set(key, value, dateTime);
            }
        }

        public static T Get<T>(string key)
        {
            lock (lockHelper)
            {
                return cs.Get<T>(key);
            }
        }

        public static object Remove(string key)
        {
            lock (lockHelper)
            {
                return cs.Remove(key);
            }
        }

        public static T Remove<T>(string key)
        {
            lock (lockHelper)
            {
                return cs.Remove<T>(key);
            }
        }

        #endregion
    }
}
