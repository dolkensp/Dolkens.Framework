using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Caching.Providers
{
    internal class DefaultServiceProvider : IServiceProvider
    {
        private static Object _cache;

        public Object GetService(Type serviceType)
        {
            if (serviceType == typeof(ICacheProvider))
            {
                return new TrackingCacheProvider { };
            }

            if (serviceType == typeof(ICache))
            {
                return _cache = _cache ?? Activator.CreateInstance(Assembly.Load(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Assembly"] ?? "Dolkens.Framework.Caching.Memory").GetType(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Type"] ?? "Dolkens.Framework.Caching.Memory.Cache"));
            }

            if (serviceType == typeof(ICacheDependency))
            {
                return Activator.CreateInstance(Assembly.Load(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Assembly"] ?? "Dolkens.Framework.Caching.Memory").GetType(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.DependencyType"] ?? "Dolkens.Framework.Caching.Memory.CacheDependency"));
            }

            if (serviceType == typeof(ICacheSettings))
            {
                return Activator.CreateInstance(Assembly.Load(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Assembly"] ?? "Dolkens.Framework.Caching.Memory").GetType(ConfigurationManager.AppSettings["Dolkens.Framework.Caching.SettingsType"] ?? "Dolkens.Framework.Caching.Memory.CacheSettings"));
            }

            throw new NotImplementedException();
        }
    }
}
