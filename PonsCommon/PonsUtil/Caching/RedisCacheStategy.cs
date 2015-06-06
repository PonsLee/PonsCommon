using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using PonsUtil.Json;
using ServiceStack.Redis;

namespace PonsUtil.Caching
{
    public class RedisCacheStategy : BaseCacheStrategy
    {
        /// <summary>
        /// 静态化的Redis连接池，将来这里可能会很复杂，根据KEY的不同来找不同的服务器集群
        /// </summary>
        private static Hashtable REDIS_CLIENT_POOL = InitClientPool();

        private static string space = "SESSION";

        public RedisCacheStategy()
        {
            bool hasRedisKey = false;
            foreach (string key in ConfigurationManager.AppSettings.Keys)
            {
                if (key.StartsWith("redis_server_"))
                {
                    hasRedisKey = true;
                    break;
                }
            }
            if (!hasRedisKey)
            {
                throw new Exception("未配置 AppSettings[\"redis_server_\"]");
            }
        }

        /// <summary>
        /// 初始化Redis的连接池
        /// </summary>
        /// <returns></returns>
        private static Hashtable InitClientPool()
        {
            var ht = new Hashtable();

            foreach (KeyValuePair<string, string> host in RedisConfig.RedisHostList)
            {
                string this_key_space = host.Key;
                string this_host_ip = host.Value;
                var poolConfig = new RedisClientManagerConfig();
                poolConfig.MaxReadPoolSize = RedisConfig.RedisMaxReadPool;
                poolConfig.MaxWritePoolSize = RedisConfig.RedisMaxWritePool;

                var thisClient = new PooledRedisClientManager(new string[] { this_host_ip }, new string[] { this_host_ip }, poolConfig);

                ht.Add(this_key_space, thisClient);
            }

            return ht;
        }

        /// <summary>
        /// 从当前的池子里为一个keyspace找一个客户端
        /// </summary>
        /// <returns></returns>
        private static RedisNativeClient GetNativeClientForKeySpace()
        {
            var random = new Random();
            int index = random.Next(0, RedisConfig.KeyList.Count);
            string keySpace = RedisConfig.KeyList[index];
            var thisClient = (PooledRedisClientManager)REDIS_CLIENT_POOL[keySpace];

            return (RedisNativeClient)(thisClient.GetClient());
        }

        public override void Set(string key, object value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonHelper.ConvertToJosnString(value));
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                client.Set(key, bytes);
                client.Expire(key, Convert.ToInt32(_DefaultExpires.TotalSeconds));
            }
        }

        public override void Set(string key, object value, TimeSpan timeSpan)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonHelper.ConvertToJosnString(value));
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                client.Set(key, bytes);
                client.Expire(key, Convert.ToInt32(timeSpan.TotalSeconds));
            }
        }

        public override void Set(string key, object value, DateTime dateTime)
        {
            int totalSecond = dateTime <= DateTime.Now ? 0 : Convert.ToInt32((dateTime - DateTime.Now).TotalSeconds);
            byte[] bytes = Encoding.UTF8.GetBytes(JsonHelper.ConvertToJosnString(value));
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                client.Set(key, bytes);
                client.Expire(key, totalSecond);
            }
        }

        public override T Get<T>(string key)
        {
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                byte[] redisContent = client.Get(key);
                if (redisContent == null)
                {
                    return default(T);
                }
                var content = Encoding.UTF8.GetString(redisContent, 0, redisContent.Length);
                return JsonHelper.ConvertToObject<T>(content);
            }
        }

        public override object Remove(string key)
        {
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                return client.Del(key);
            }
        }

        public override T Remove<T>(string key)
        {
            using (RedisNativeClient client = GetNativeClientForKeySpace())
            {
                byte[] redisContent = client.Get(key);
                if (redisContent == null)
                {
                    return default(T);
                }
                var content = Encoding.UTF8.GetString(redisContent, 0, redisContent.Length);
                client.Del(key);
                return JsonHelper.ConvertToObject<T>(content);
            }
        }
    }
}
