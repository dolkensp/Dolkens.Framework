using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Memory
{
    public class Cache : ASP.MemoryCache
    {
        public Cache() : base("Dolkens.Framework.Caching.Memory.Cache", null) { }
    }
}