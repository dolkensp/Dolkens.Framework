using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Stub
{
    public class CacheSettings : ICacheSettings
    {
        public DateTime AbsoluteExpiration { get; set; }
        public String CacheKeyOverride { get; set; }
        public ICacheDependency Dependencies { get; set; }
        public Int32 LockTimeout { get; set; }
        public CacheItemPriority Priority { get; set; }
        public TimeSpan SlidingExpiration { get; set; }

        public CacheItemPolicy GetCacheItemPolicy()
        {
            return new CacheItemPolicy { };
        }
    }
}