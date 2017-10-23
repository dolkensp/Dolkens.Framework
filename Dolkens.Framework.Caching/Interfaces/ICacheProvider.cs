using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Caching.Interfaces
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Raw get/set access to the cache
        /// </summary>
        [Obsolete("Remove this when possible", false)]
        ICache Cache { get; }

        /// <summary>
        /// Get the default cache settings for the configured caching provider
        /// </summary>
        ICacheSettings DefaultSettings { get; }

        /// <summary>
        /// Get the default dependency tracking for the configured caching provider
        /// </summary>
        ICacheDependency DefaultDependency { get; }

        /// <summary>
        /// Add the given cache keys to the list of keys that are tracked for the current cached method
        /// </summary>
        /// <param name="cacheKeys"></param>
        void AddTracking(params String[] cacheKeys);

        /// <summary>
        /// Return the cache key for a given method, with given arguments
        /// </summary>
        /// <param name="method">The method to cache</param>
        /// <param name="args">The arguments passed to the givem method</param>
        /// <returns>A cache key for the given method</returns>
        String BuildCacheKey(String method, params Object[] args);

        /// <summary>
        /// Return the cache key for a given delegate, with given arguments
        /// </summary>
        /// <typeparam name="TResult">The return type of the given delegate</typeparam>
        /// <param name="delegate">The delegate to cache</param>
        /// <param name="args">The arguments passed to the given delegate</param>
        /// <returns>A cache key for the given delegate</returns>
        String BuildCacheKey<TResult>(MethodDelegate<TResult> @delegate, params Object[] args);

        TResult GetCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args);

        TResult UpdateCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, TResult data, params Object[] args);

        void DeleteCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args);
    }
}
