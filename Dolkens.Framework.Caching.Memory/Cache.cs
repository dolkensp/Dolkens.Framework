using System;
using Dolkens.Framework.Caching.Interfaces;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Memory
{
    public class Cache : ICache
    {
        private ASP.MemoryCache _cache = new ASP.MemoryCache("Dolkens.Framework.Caching.Memory.Cache", null);

        public Cache() : base() { }

        public Object this[String key]
        {
            get { return this._cache[key]; }
            set { this._cache[key] = value; }
        }

        public void Add(String key, Object value, ICacheSettings settings)
        {
            this._cache.Add(key, value, settings.AbsoluteExpiration);
        }

        public Object Get(String key)
        {
            return this._cache.Get(key);
        }

        public void Remove(String key)
        {
            this._cache.Remove(key);
        }
    }
}