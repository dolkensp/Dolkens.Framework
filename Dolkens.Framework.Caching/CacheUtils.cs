#define BACKWARDS_COMPATIBLE
using Dolkens.Framework.Caching.Interfaces;
using Dolkens.Framework.Caching.Providers;
using System;
using System.Threading;

namespace Dolkens.Framework.Caching
{
    public delegate ReturnType MethodDelegate<ReturnType>(params Object[] args);

    public static class CacheUtils
    {
        private static ThreadLocal<ICacheProvider> _provider = new ThreadLocal<ICacheProvider> { };
        private static ICacheProvider UnderlyingProvider
        {
            get { return CacheUtils._provider.Value = CacheUtils._provider.Value ?? CacheUtils.ServiceProvider.GetService(typeof(ICacheProvider)) as ICacheProvider; { }; }
        }

        [Obsolete("Remove this when possible", false)]
        public static ICache Cache { get { return CacheUtils.UnderlyingProvider.Cache; } }

        public static IServiceProvider ServiceProvider { get; set; } = new DefaultServiceProvider { };

        /// <summary>
        /// Get the default cache settings for the configured caching provider
        /// </summary>
        public static ICacheSettings DefaultSettings {  get { return CacheUtils.UnderlyingProvider.DefaultSettings; } }

        /// <summary>
        /// Add the given cache keys to the list of keys that are tracked for the current cached method
        /// </summary>
        /// <param name="cacheKeys"></param>
        public static void AddTracking(params String[] cacheKeys) { CacheUtils.UnderlyingProvider.AddTracking(cacheKeys); }

        /// <summary>
        /// Return the cache key for a given delegate, with given arguments
        /// </summary>
        /// <typeparam name="TResult">The return type of the given delegate</typeparam>
        /// <param name="delegate">The delegate to cache</param>
        /// <param name="args">The arguments passed to the given delegate</param>
        /// <returns>A cache key for the given delegate</returns>
        public static String BuildCacheKey<TResult>(MethodDelegate<TResult> @delegate, params Object[] args) { return CacheUtils.UnderlyingProvider.BuildCacheKey<TResult>(@delegate, args); }

        public static TResult GetCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args) { return CacheUtils.UnderlyingProvider.GetCachedData<TResult>(settings, @delegate, args); }

        public static TResult UpdateCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, TResult data, params Object[] args) { return CacheUtils.UnderlyingProvider.UpdateCachedData<TResult>(settings, @delegate, data, args); }

        public static void DeleteCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args) { CacheUtils.UnderlyingProvider.DeleteCachedData<TResult>(settings, @delegate, args); }

#if BACKWARDS_COMPATIBLE

#region public static TResult GetCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private static TResult GetCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // Default cache settings
            settings = settings ?? CacheUtils.UnderlyingProvider.DefaultSettings;

#region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = CacheUtils.UnderlyingProvider.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

#endregion

            settings.CacheKeyOverride = cacheKey;

            return CacheUtils.UnderlyingProvider.GetCachedData<TResult>(settings, @delegate, args);
        }

        public static TResult GetCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                args);
        }

        public static TResult GetCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                arg1, args);
        }

        public static TResult GetCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                arg1, arg2, args);
        }

        public static TResult GetCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                arg1, arg2, arg3, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                arg1, arg2, arg3, arg4, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return CacheUtils.GetCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

#endregion

#region public static TResult UpdateCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private static TResult UpdateCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, TResult data, params Object[] args)
        {
            // Default cache settings
            settings = settings ?? CacheUtils.UnderlyingProvider.DefaultSettings;

#region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = CacheUtils.UnderlyingProvider.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

#endregion

            settings.CacheKeyOverride = cacheKey;

            return CacheUtils.UnderlyingProvider.GetCachedData<TResult>(settings, @delegate, args);
        }

        public static TResult UpdateCachedData<TResult>(ICacheSettings settings, Func<TResult> method, TResult data, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                data,
                args);
        }

        public static TResult UpdateCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, TResult data, T arg1, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                data,
                arg1, args);
        }

        public static TResult UpdateCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, TResult data, T1 arg1, T2 arg2, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                data,
                arg1, arg2, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                data,
                arg1, arg2, arg3, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                data,
                arg1, arg2, arg3, arg4, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                data,
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return CacheUtils.UpdateCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                data,
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

#endregion

#region public static TResult DeleteCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        private static void DeleteCachedData_Proxy<TResult>(ICacheSettings settings, String methodType, String methodName, MethodDelegate<TResult> @delegate, params Object[] args)
        {
            // Default cache settings
            settings = settings ?? CacheUtils.UnderlyingProvider.DefaultSettings;

#region Build CacheKey

            String cacheKey = settings.CacheKeyOverride;

            if (String.IsNullOrWhiteSpace(cacheKey))
                cacheKey = CacheUtils.UnderlyingProvider.BuildCacheKey(String.Format("{0}.{1}", methodType, methodName), args);

#endregion

            settings.CacheKeyOverride = cacheKey;

            CacheUtils.UnderlyingProvider.DeleteCachedData<TResult>(settings, @delegate, args);
        }

        public static void DeleteCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke())),
                args);
        }

        public static void DeleteCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1))),
                arg1, args);
        }

        public static void DeleteCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2))),
                arg1, arg2, args);
        }

        public static void DeleteCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3))),
                arg1, arg2, arg3, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4))),
                arg1, arg2, arg3, arg4, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5))),
                arg1, arg2, arg3, arg4, arg5, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6))),
                arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            CacheUtils.DeleteCachedData_Proxy(
                settings,
                method.Method.DeclaringType.GetFriendlyTypeName(),
                method.Method.Name,
                ((Object[] argv) => (method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16))),
                arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

#endregion

#endif
    }
}