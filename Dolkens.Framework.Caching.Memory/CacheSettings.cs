using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Configuration;
using System.Linq;
using ASP = System.Runtime.Caching;

namespace Dolkens.Framework.Caching.Memory
{
    public class CacheSettings : ICacheSettings
    {
        public String CacheKeyOverride { get; set; }

        internal TimeSpan _absoluteOffset = ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Default.AbsoluteOffset"].ToTimeSpan(TimeSpan.FromMinutes(10)); // Default to 10 minute expiration
        private DateTime _absoluteExpiration = ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Default.AbsoluteExpiration"].ToDateTime(DateTime.MinValue);
        public DateTime AbsoluteExpiration
        {
            get { return this._absoluteExpiration != DateTime.MinValue ? this._absoluteExpiration : DateTime.Now.Add(this._absoluteOffset); }
            set { this._absoluteExpiration = value; }
        }

        private TimeSpan _slidingExpiration = ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Default.SlidingExpiration"].ToTimeSpan(TimeSpan.Zero); 
        public TimeSpan SlidingExpiration { get; set; }

        private ASP.CacheItemPriority _priority = ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Default.Priority"].ToEnum(ASP.CacheItemPriority.Default, true);
        public ASP.CacheItemPriority Priority { get; set; }

        public ICacheDependency Dependencies { get; set; }

        public ASP.CacheItemPolicy GetCacheItemPolicy()
        {
            ASP.CacheItemPolicy policy = new ASP.CacheItemPolicy
            {
                AbsoluteExpiration = this._absoluteOffset == TimeSpan.Zero ? this.AbsoluteExpiration : DateTime.Now.Add(this._absoluteOffset),
                SlidingExpiration = this.SlidingExpiration,
                Priority = this.Priority,
                // TODO: Add Callback Support
            };

            // TODO: Add dependency tracking
            // if (this.Dependencies.CacheKeys != null && this.Dependencies.CacheKeys.Count() > 0)
            // {
            //     policy.ChangeMonitors.Add(ASP.MemoryCache.Default.CreateCacheEntryChangeMonitor(this.Dependencies.CacheKeys.Distinct()));
            // }

            // TODO: Add more Support for more ChangeMonitors (Files + SQL)

            return policy;
        }

        private Int32? _lockTimeout;
        public Int32 LockTimeout
        {
            get { return (this._lockTimeout = this._lockTimeout ?? ConfigurationManager.AppSettings["Dolkens.Framework.Caching.LockTimeout"].ToInt32()) ?? 500; }
            set { this._lockTimeout = value; }
        }
    }
}