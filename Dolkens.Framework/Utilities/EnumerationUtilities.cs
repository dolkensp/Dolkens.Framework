using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Utilities
{
    public static class EnumerationUtilities
    {
        public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, TSource @default)
        {
            return source.Count() > 0 ? source.First() : @default;
        }

        public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, Boolean> predicate, TSource @default)
        {
            source = source.Where(predicate);
            return source.Count() > 0 ? source.First() : @default;
        }
    }
}

namespace System.Linq
{
    using DDRIT = Dolkens.Framework.Utilities.EnumerationUtilities;

    public static class _Proxy
    {
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable&lt;<typeparamref name="TSource"/>&gt; to return the first element of.</param>
        /// <param name="default">The default value to return if the source is empty.</param>
        /// <returns><paramref name="default"/> if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource @default)
        {
            return DDRIT.FirstOrDefault(source, @default);
        }

        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable&lt;<typeparamref name="TSource"/>&gt; to return the first element of.</param>
        /// <param name="default">The default value to return if the source is empty.</param>
        /// <returns><paramref name="default"/> if <paramref name="source"/> is empty or if no element passes the test specified by <paramref name="predicate"/>;
        /// otherwise, the first element in <paramref name="source"/> that passes the test specified by <param name="predicate"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, Boolean> predicate, TSource @default)
        {
            return DDRIT.FirstOrDefault(source, predicate, @default);
        }
    }
}
