using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Utilities
{
    public static class DictionaryUtilities
    {
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> input, TKey key, TValue @default)
        {
            TValue result;

            if (input.TryGetValue(key, out result))
                return result;

            return @default;
        }
    }
}

namespace System.Collections.Generic
{
    using DDRIT = Dolkens.Framework.Utilities.DictionaryUtilities;

    public static partial class _Proxy
    {
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> input, TKey key, TValue @default) { return DDRIT.GetValue<TKey, TValue>(input, key, @default); }
    }
}
