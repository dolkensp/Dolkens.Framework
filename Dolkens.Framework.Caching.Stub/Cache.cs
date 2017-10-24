using System;
using Dolkens.Framework.Caching.Interfaces;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Stub
{
    public class Cache : ICache
    {
        public Object this[String key]
        {
            get { return null; }
            set { }
        }

        public Boolean Add(ASP.CacheItem item, ASP.CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }

        public Object Get(String key, String regionName = null)
        {
            return null;
        }

        public Object Remove(String key, String regionName = null)
        {
            return null;
        }

        public ICacheDependency DefaultDependency
        {
            get { return new CacheDependency { }; }
        }

        public ICacheSettings DefaultSettings
        {
            get { return new CacheSettings { }; }
        }
    }
}