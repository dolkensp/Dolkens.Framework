using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Configuration;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Interfaces
{
    public interface ICacheSettings
    {
        String CacheKeyOverride { get; set; }
        DateTime AbsoluteExpiration { get; set; }
        TimeSpan SlidingExpiration { get; set; }
        ICacheDependency Dependencies { get; set; }
        ASP.CacheItemPriority Priority { get; set; }

        ASP.CacheItemPolicy GetCacheItemPolicy();

        Int32 LockTimeout { get; set; }
    }
}