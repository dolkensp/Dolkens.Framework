using Dolkens.Framework.Caching.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Redis
{
    public class Cache : ICache
    {
        private ASP.MemoryCache _local = new ASP.MemoryCache("Dolkens.Framework.Caching.Redis.Cache", null);

        private ConnectionMultiplexer _redis;
        private IDatabase _cache;
        private JsonSerializerSettings _json;

        public Cache()
        {
            var connectionStringOrName = "Dolkens.Framework.Caching.Redis";

            if (ConfigurationManager.ConnectionStrings[connectionStringOrName] != null)
            {
                connectionStringOrName = ConfigurationManager.ConnectionStrings[connectionStringOrName].ConnectionString;
            }

            this._redis = ConnectionMultiplexer.Connect(connectionStringOrName);
            this._cache = this._redis.GetDatabase();
            this._json = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            };
        }

        public Object this[String key]
        {
            get { return this.Get(key); }
            set { this.Add(key, value, CacheUtils.DefaultSettings); }
        }

        public Object Get(String key)
        {
            var localCache = this._local.Get(key);
            if (localCache != null) return localCache;

            var cache = this._cache.StringGet(key);
            if (cache.HasValue || cache.IsNull) return null;
            return JsonConvert.DeserializeObject(cache, this._json);
        }

        public void Add(String key, Object value, ICacheSettings settings)
        {
            this._local.Add(key, value, settings.AbsoluteExpiration);
            this._cache.StringSet(key, JsonConvert.SerializeObject(value, Formatting.None, this._json), settings.SlidingExpiration, When.Always, CommandFlags.FireAndForget);
        }

        public void Remove(String key)
        {
            this._local.Remove(key);
            this._cache.KeyDelete(key, CommandFlags.FireAndForget);
        }
    }
}