﻿#define CACHENULLS
using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace Dolkens.Framework.Caching
{
    public class CacheProvider
    {
#if CACHENULLS
        public static Object NULL_OBJECT = new { IsNull = true };
#endif

        public const String CacheTrackingKey = "Dolkens.Framework.Caching.CacheTracking";

        public static String CACHEKEY_SEPARATOR1 = "|";
        public static String CACHEKEY_SEPARATOR2 = ",";
        public static String RegionName = null;

        private ICache _cache;
        // public static ObjectCache Cache
        // {
        //     // HACK: This should be private and methods below should be used for cache access
        //     get
        //     {
        //         if (CacheProvider._cache == null)
        //             CacheProvider._cache = Activator.CreateInstance(CacheProvider._cacheType) as ObjectCache;
        // 
        //         return CacheProvider._cache;
        //     }
        // }

        private IServiceProvider _serviceProvider;

        private Dictionary<Object, Object> _tracker = new Dictionary<Object, Object> { { CacheProvider.CacheTrackingKey, new String[] { } } };

        internal CacheProvider()
        {
            this._serviceProvider = new DefaultServiceProvider { };

            this._cache = this._serviceProvider.GetService(typeof(ICache)) as ICache;

            if (this._cache == null)
            {
                throw new Exception("Unable to create ICache service.");
            }
        }

        private Hashtable _lockTable = new Hashtable();

        public CacheProvider(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;

            this._cache = this._serviceProvider.GetService(typeof(ICache)) as ICache;

            if (this._cache == null)
            {
                throw new Exception("Unable to create ICache service.");
            }
        }

        public ICacheSettings DefaultSettings
        {
            get { return this._serviceProvider.GetService(typeof(ICacheSettings)) as ICacheSettings; }
        }

        public ICacheDependency DefaultDependency
        {
            get { return this._serviceProvider.GetService(typeof(ICacheDependency)) as ICacheDependency; }
        }

        internal IEnumerable<String> SwapTracking(IEnumerable<String> newList = null)
        {
            IEnumerable<String> buffer = this._tracker[CacheProvider.CacheTrackingKey] as IEnumerable<String> ?? new String[] { };

            this._tracker[CacheProvider.CacheTrackingKey] = (newList ?? new String[] { }).ToArray();

            return buffer.Distinct<String>().ToArray<String>();
        }

        public void AddTracking(params String[] cacheKeys)
        {
            IEnumerable<String> buffer = this._tracker[CacheProvider.CacheTrackingKey] as IEnumerable<String> ?? new String[] { };

            buffer = buffer.Union(cacheKeys).ToArray();

            this._tracker[CacheProvider.CacheTrackingKey] = buffer;
        }

        public String BuildCacheKey(String method, params Object[] args)
        {
            StringBuilder keyBuilder = new StringBuilder();

            keyBuilder.Append(method);

            for (Int32 i = 0, j = args.Length; i < j; i++)
            {
                if (args[i] is Array)
                    foreach (Object arg in args[i] as Array)
                        keyBuilder.AppendFormat("{0}{1}", CacheProvider.CACHEKEY_SEPARATOR2, arg);
                else
                    keyBuilder.AppendFormat("{0}{1}", CacheProvider.CACHEKEY_SEPARATOR1, args[i]);
            }

            // Prepend prefix to key
            keyBuilder.Insert(0, "method|");

            // TODO: Hash when this key is too long

            return keyBuilder.ToString();
        }

        public String BuildCacheKey<TResult>(MethodDelegate<TResult> @delegate, params Object[] args)
        {
            return this.BuildCacheKey(String.Format("{0}.{1}", @delegate.Method.DeclaringType.GetFriendlyTypeName(), @delegate.Method.Name), args);
        }

        public TResult GetCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;
            Boolean inLock = false;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(@delegate, args);

            #endregion

            // Add tracking key immediately
            // No point only adding it on success, because if there's an error, we don't want to cache the error state, so the dependency SHOULD fail
            this.AddTracking(cacheKey);

            #region Non Locking Cache Retrieval

            // Load data from cache
            Object buffer = this._cache.Get(cacheKey);

#if CACHENULLS
            if (buffer == CacheProvider.NULL_OBJECT)
                return default(TResult);
#endif

            if (buffer is TResult)
                return (TResult)buffer;

            #endregion

            #region Locking Cache Retrieval

            if (buffer == null && ConfigurationManager.AppSettings["Dolkens.Framework.Caching.Lock"].ToBoolean(true))
            {
                if (this._lockTable[cacheKey] == null)
                {
                    // Only process a single thread for a particular cacheKey at a time
                    lock (this._lockTable)
                    {
                        if (this._lockTable[cacheKey] == null)
                        {
                            this._lockTable[cacheKey] = this._lockTable[cacheKey] ?? new Object();
                        }
                    }
                }

                inLock = Monitor.TryEnter(this._lockTable[cacheKey], settings.LockTimeout);

                buffer = this._cache.Get(cacheKey);

                if (buffer == CacheProvider.NULL_OBJECT)
                {
                    if (inLock)
                        Monitor.Exit(this._lockTable[cacheKey]);

                    return default(TResult);
                }

                if (buffer is TResult)
                {
                    if (inLock)
                        Monitor.Exit(this._lockTable[cacheKey]);

                    return (TResult)buffer;
                }
            }

            #endregion

            #region Retrieve Fresh Data

            if (buffer == null)
            {
                // Swap in a clean tracking list
                IEnumerable<String> trackingList = this.SwapTracking(null);

                try
                {
                    buffer = @delegate(args);
                }
                catch (Exception ex)
                {
                    // Release the lock
                    if (inLock)
                        Monitor.Exit(this._lockTable[cacheKey]);

                    // Bubble exception
                    throw ex;
                }
                finally
                {
                    // Return tracking list to normal
                    trackingList = this.SwapTracking(trackingList);
                }

                #region Cache Storage

#if !CACHENULLS
                // Early exit for empty result
                // TODO: Consider adding option to cache NULL data here
                if (buffer == null)
                    return buffer = null;
#endif

                #region Ensure we have a dependency and that we have a cachekey collection to add to

                settings.Dependencies = settings.Dependencies ?? this.DefaultDependency;
                settings.Dependencies.CacheKeys = settings.Dependencies.CacheKeys ?? new String[] { };

                #endregion

                settings.Dependencies.CacheKeys = settings.Dependencies.CacheKeys.Union(trackingList).Distinct().ToArray();

                CacheItem cacheItem = null;

#if CACHENULLS
                if (buffer == null)
                    cacheItem = new CacheItem(cacheKey, CacheProvider.NULL_OBJECT, "CacheUtils");
                else
#endif
                cacheItem = new CacheItem(cacheKey, buffer, "CacheUtils");

                // Save Data To Cache
                this._cache.Add(cacheItem, settings.GetCacheItemPolicy());

                #endregion

            }

            #endregion

            // Release the lock
            if (inLock)
                Monitor.Exit(this._lockTable[cacheKey]);

            return (TResult)buffer;
        }

        public TResult UpdateCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, TResult data, params Object[] args)
        {
            // TODO: Support Locking Here

            var buffer = data;

            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(@delegate, args);

            #endregion

            #region Cache Storage

#if !CACHENULLS
            // Early exit for empty result
            // TODO: Consider adding option to cache NULL data here
            if (buffer == null)
                return buffer = null;
#endif

            #region Ensure we have a dependency and that we have a cachekey collection to add to

            settings.Dependencies = settings.Dependencies ?? this.DefaultDependency;
            settings.Dependencies.CacheKeys = settings.Dependencies.CacheKeys ?? new String[] { };

            #endregion

            CacheItem cacheItem = null;

#if CACHENULLS
            if (buffer == null)
                cacheItem = new CacheItem(cacheKey, CacheProvider.NULL_OBJECT, "CacheUtils");
            else
#endif
            cacheItem = new CacheItem(cacheKey, buffer, "CacheUtils");

            // Save Data To Cache
            this._cache.Add(cacheItem, settings.GetCacheItemPolicy());

            #endregion

            return buffer;
        }

        public void DeleteCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // TODO: Support Locking Here

            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(@delegate, args);

            #endregion

            this._cache.Remove(cacheKey);
        }

        #region public static TResult GetCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private TResult GetCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

            #endregion

            settings.CacheKeyOverride = cacheKey;

            return this.GetCachedData<TResult>(settings, @delegate, args);
        }

        public TResult GetCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                args);
        }

        public TResult GetCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                arg1, args);
        }

        public TResult GetCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                arg1, arg2, args);
        }

        public TResult GetCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                arg1, arg2, arg3, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                arg1, arg2, arg3, arg4, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return this.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion

        #region public TResult UpdateCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private TResult UpdateCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, TResult data, params Object[] args)
        {
            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

            #endregion

            settings.CacheKeyOverride = cacheKey;

            return this.GetCachedData<TResult>(settings, @delegate, args);
        }

        public TResult UpdateCachedData<TResult>(ICacheSettings settings, Func<TResult> method, TResult data, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                data,
                args);
        }

        public TResult UpdateCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, TResult data, T arg1, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                data,
                arg1, args);
        }

        public TResult UpdateCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, TResult data, T1 arg1, T2 arg2, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                data,
                arg1, arg2, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                data,
                arg1, arg2, arg3, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                data,
                arg1, arg2, arg3, arg4, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                data,
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return this.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion

        #region public TResult DeleteCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private void DeleteCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // Default cache settings
            if (settings == null)
                settings = this.DefaultSettings;

            #region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = this.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

            #endregion

            settings.CacheKeyOverride = cacheKey;

            this.DeleteCachedData<TResult>(settings, @delegate, args);
        }

        public void DeleteCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                args);
        }

        public void DeleteCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                arg1, args);
        }

        public void DeleteCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                arg1, arg2, args);
        }

        public void DeleteCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                arg1, arg2, arg3, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                arg1, arg2, arg3, arg4, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            this.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion
    }
}
