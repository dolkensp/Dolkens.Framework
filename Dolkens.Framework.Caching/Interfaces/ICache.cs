using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Caching.Interfaces
{
    public interface ICache
    {
        Object this[String key] { get; set; }

        Object Get(String key);
        void Add(String key, Object value, ICacheSettings settings);
        void Remove(String key);
    }
}
