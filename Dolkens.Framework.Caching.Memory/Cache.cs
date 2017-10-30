using System;
using Dolkens.Framework.Caching.Interfaces;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Memory
{
    public class Cache : ASP.MemoryCache, ICache
    {
        public Cache() : base("Dolkens.Framework.Caching.Memory.Cache", null) { }

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