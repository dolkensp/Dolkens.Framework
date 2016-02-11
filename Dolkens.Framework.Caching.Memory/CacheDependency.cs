using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Collections.Generic;

namespace Dolkens.Framework.Caching.Memory
{
    public class CacheDependency : ICacheDependency
    {
        public IEnumerable<String> Filenames { get; set; }
        public IEnumerable<String> CacheKeys { get; set; }
    }
}