﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Caching.Interfaces
{
    public interface ICache
    {
        Object Get(String key, String regionName = null);
        Boolean Add(CacheItem item, CacheItemPolicy policy);
        Object Remove(String key, String regionName = null);
    }
}
