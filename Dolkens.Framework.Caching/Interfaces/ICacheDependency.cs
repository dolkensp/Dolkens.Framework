using System;
using System.Collections.Generic;

namespace Dolkens.Framework.Caching.Interfaces
{
    // Summary:
    //     Establishes a dependency relationship between an item stored in an ASP.NET
    //     application's System.Web.Caching.Cache object and a file, cache key, an array
    //     of either, or another System.Web.Caching.CacheDependency object. The System.Web.Caching.CacheDependency
    //     class monitors the dependency relationships so that when any of them changes,
    //     the cached item will be automatically removed.
    public interface ICacheDependency
    {
        IEnumerable<String> Filenames { get; set; }
        IEnumerable<String> CacheKeys { get; set; }
    }
}