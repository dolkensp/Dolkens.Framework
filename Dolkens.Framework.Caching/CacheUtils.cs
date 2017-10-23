using Dolkens.Framework.Caching.Interfaces;
using System;
using System.Threading;

namespace Dolkens.Framework.Caching
{
    public delegate ReturnType MethodDelegate<ReturnType>(params Object[] args);

    public static class CacheUtils
    {
        private static ThreadLocal<CacheProvider> _provider = new ThreadLocal<CacheProvider> { };
        public static CacheProvider Provider
        {
            get
            {
                return (CacheUtils._provider.Value = CacheUtils._provider.Value ?? new CacheProvider { });
            }
        }

        public static ICacheSettings DefaultSettings {  get { return CacheUtils.Provider.DefaultSettings; } }

        public static void AddTracking(params String[] cacheKeys) { CacheUtils.Provider.AddTracking(cacheKeys); }

        public static String BuildCacheKey(String method, params Object[] args) { return CacheUtils.Provider.BuildCacheKey(method, args); }

        public static String BuildCacheKey<TResult>(MethodDelegate<TResult> @delegate, params Object[] args) { return CacheUtils.Provider.BuildCacheKey<TResult>(@delegate, args); }

        public static TResult GetCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args) { return CacheUtils.Provider.GetCachedData<TResult>(settings, @delegate, args); }

        public static TResult UpdateCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, TResult data, params Object[] args) { return CacheUtils.Provider.UpdateCachedData<TResult>(settings, @delegate, data, args); }

        public static void DeleteCachedData<TResult>(ICacheSettings settings, MethodDelegate<TResult> @delegate, params Object[] args) { CacheUtils.Provider.DeleteCachedData<TResult>(settings, @delegate, args); }

        #region public TResult GetCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        public static TResult GetCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<TResult>(settings, method, args);
        }

        public static TResult GetCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T, TResult>(settings, method, arg1, args);
        }

        public static TResult GetCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, TResult>(settings, method, arg1, arg2, args);
        }

        public static TResult GetCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, TResult>(settings, method, arg1, arg2, arg3, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, TResult>(settings, method, arg1, arg2, arg3, arg4, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static TResult GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return CacheUtils.Provider.GetCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion

        #region public TResult UpdateCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        public static TResult UpdateCachedData<TResult>(ICacheSettings settings, Func<TResult> method, TResult data, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<TResult>(settings, method, data, args);
        }

        public static TResult UpdateCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, TResult data, T arg1, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T, TResult>(settings, method, data, arg1, args);
        }

        public static TResult UpdateCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, TResult data, T1 arg1, T2 arg2, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, TResult>(settings, method, data, arg1, arg2, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, TResult>(settings, method, data, arg1, arg2, arg3, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, TResult>(settings, method, data, arg1, arg2, arg3, arg4, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static TResult UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, TResult data, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            return CacheUtils.Provider.UpdateCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(settings, method, data, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion

        #region public TResult DeleteCachedData<...>(CacheSettings settings, ..., params Object[] args)
        // NOTE: This section looks horrible - it is, but it's how .Net natively supports Function and Action classes - up to 16 generic parameters.
        //       Maybe one day we won't need these.

        public static void DeleteCachedData<TResult>(ICacheSettings settings, Func<TResult> method, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<TResult>(settings, method, args);
        }

        public static void DeleteCachedData<T, TResult>(ICacheSettings settings, Func<T, TResult> method, T arg1, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T, TResult>(settings, method, arg1, args);
        }

        public static void DeleteCachedData<T1, T2, TResult>(ICacheSettings settings, Func<T1, T2, TResult> method, T1 arg1, T2 arg2, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, TResult>(settings, method, arg1, arg2, args);
        }

        public static void DeleteCachedData<T1, T2, T3, TResult>(ICacheSettings settings, Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, TResult>(settings, method, arg1, arg2, arg3, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, TResult>(settings, method, arg1, arg2, arg3, arg4, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, args);
        }

        public static void DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(ICacheSettings settings, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, params Object[] args)
        {
            CacheUtils.Provider.DeleteCachedData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(settings, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, args);
        }

        #endregion
    }

}