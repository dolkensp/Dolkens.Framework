using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Dolkens.Framework.Utilities
{
    public static class ParserUtilities
    {
        internal static Dictionary<Type, MethodInfo> _parserMap = new Dictionary<Type, MethodInfo>();
        internal static Dictionary<Type, MethodInfo> _byteArrayMap = new Dictionary<Type, MethodInfo>();
        internal static Dictionary<Type, ConstructorInfo> _byteConstructorMap = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Converts a string into a target type.
        /// </summary>
        /// <param name="type">The type we wish to return.</param>
        /// <param name="input">The string value we wish to convert.</param>
        /// <returns>An object containing the data represented by the input string, in the input type.</returns>
        public static Object Parse(String input, Type type, Object @default = null)
        {
            // Early exit for Strings
            if (type == typeof(String))
                return input;

            Boolean isNullable = false;

            // Add support for Nullable types
            if (type.IsNullable())
            {
                if (String.IsNullOrWhiteSpace(input))
                    return null;

                type = type.GetGenericArguments()[0];

                isNullable = true;
            }

            if (input == null)
                return @default;

            try
            {
                // Add some extra datetime support
                if (type == typeof(DateTime))
                {
                    DateTime result = DateTime.MinValue;

                    if (DateTime.TryParseExact(input, Utils.DateTimeFormats, null, DateTimeStyles.None, out result))
                        return result;
                    else
                        return DateTime.Parse(input);
                }

                // If it's an enumeration, we'll try this.
                if (type.IsEnum)
                    return Enum.Parse(type, input);

                MethodInfo parseMethod;

                // Throw in a little static reflection caching
                if (!ParserUtilities._parserMap.TryGetValue(type, out parseMethod))
                {
                    // Attempt to find inbuilt parsing methods
                    parseMethod = type.GetMethod("Parse", new Type[] { typeof(String) });

                    ParserUtilities._parserMap[type] = parseMethod;
                }

                // If there's inbuilt parsing methods, attempt to use those.
                if (parseMethod != null)
                    return parseMethod.Invoke(null, new Object[] { input });
            }
            catch (Exception ex)
            {
                // Bubble errors if we have no default
                if (!isNullable && (@default == null)) ExceptionDispatchInfo.Capture(ex).Throw();

                return @default;
            }

            // And if we weren't clever enough to cast them ourselves, we'll die gracefully and let people know WTF happened.
            throw new NotImplementedException(String.Format("Unable to parse `{0}` types", type.FullName));
        }

        public static TResult To<TResult>(String input, TResult @default = default(TResult))
        {
            Type type = typeof(TResult);

            return (TResult)ParserUtilities.Parse(input, type, @default);
        }

        public static Boolean ToBoolean(String input, Boolean @default)
        {
            return input.ToBoolean() ?? @default;
        }

        public static Boolean? ToBoolean(String input)
        {
            Boolean result = false;

            if (Boolean.TryParse(input, out result))
                return result;

            return null;
        }

        public static Int16? ToInt16(Enum input)
        {
            if (input == null)
                return null;

            return (Int16)(Object)input;
        }

        public static Int16 ToInt16(Enum input, Int16 @default)
        {
            return input.ToInt16() ?? @default;
        }

        public static Int16? ToInt16(String input)
        {
            Int16 result = 0;

            if (Int16.TryParse(input, out result))
                return result;

            return null;
        }

        public static Int16 ToInt16(String input, Int16 @default)
        {
            return input.ToInt16() ?? @default;
        }

        public static Int32? ToInt32(String input)
        {
            Int32 result = 0;

            if (Int32.TryParse(input, out result))
                return result;

            return null;
        }

        public static Int32? ToInt32(Enum input)
        {
            if (input == null)
                return null;

            return (Int32)(Object)input;
        }

        public static Int32 ToInt32(Enum input, Int32 @default)
        {
            return input.ToInt32() ?? @default;
        }

        public static Int64? ToInt64(Enum input)
        {
            if (input == null)
                return null;

            return (Int64)(Object)input;
        }

        public static Int64 ToInt64(Enum input, Int64 @default)
        {
            return input.ToInt64() ?? @default;
        }

        public static Int32 ToInt32(String input, Int32 @default)
        {
            return input.ToInt32() ?? @default;
        }

        public static Int64? ToInt64(String input)
        {
            Int64 result = 0;

            if (Int64.TryParse(input, out result))
                return result;

            return null;
        }

        public static Int64 ToInt64(String input, Int64 @default)
        {
            return input.ToInt64() ?? @default;
        }

        public static Single? ToSingle(String input)
        {
            Single result = 0;

            if (Single.TryParse(input, out result))
                return result;

            return null;
        }

        public static Single ToSingle(String input, Single @default)
        {
            return input.ToSingle() ?? @default;
        }

        public static Double? ToDouble(String input)
        {
            Double result = 0;

            if (Double.TryParse(input, out result))
                return result;

            return null;
        }

        public static Double ToDouble(String input, Double @default)
        {
            return input.ToDouble() ?? @default;
        }

        public static Decimal? ToDecimal(String input)
        {
            Decimal result = 0;

            if (Decimal.TryParse(input, out result))
                return result;

            return null;
        }

        public static Decimal ToDecimal(String input, Decimal @default)
        {
            return input.ToDecimal() ?? @default;
        }

        public static DateTime? ToDateTime(String input)
        {
            DateTime result = DateTime.MinValue;

            if (DateTime.TryParseExact(input, Utils.DateTimeFormats, null, DateTimeStyles.None, out result))
                return result;

            if (DateTime.TryParse(input, out result))
                return result;

            return null;
        }

        public static DateTime ToDateTime(String input, DateTime @default)
        {
            return input.ToDateTime() ?? @default;
        }

        public static DateTimeOffset? ToDateTimeOffset(String input)
        {
            DateTimeOffset result = DateTimeOffset.MinValue;

            if (DateTimeOffset.TryParseExact(input, Utils.DateTimeFormats, null, DateTimeStyles.None, out result))
                return result;

            if (DateTimeOffset.TryParse(input, out result))
                return result;

            return null;
        }

        public static DateTimeOffset ToDateTimeOffset(String input, DateTimeOffset @default)
        {
            return input.ToDateTime() ?? @default;
        }

        public static DateTimeOffset? ToDateTimeOffset(DateTime? input)
        {
            if (input == null)
                return null;

            return new DateTimeOffset(input.Value);
        }

        public static DateTimeOffset ToDateTimeOffset(DateTime input) { return new DateTimeOffset(input); }

        public static TimeSpan? ToTimeSpan(String input)
        {
            TimeSpan result = TimeSpan.MinValue;

            if (TimeSpan.TryParse(input, out result))
                return result;

            return null;
        }

        public static TimeSpan ToTimeSpan(String input, TimeSpan @default)
        {
            return input.ToTimeSpan() ?? @default;
        }
    }
}


namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.ParserUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Converts a string into a target type.
        /// </summary>
        /// <param name="type">The type we wish to return.</param>
        /// <param name="input">The string value we wish to convert.</param>
        /// <returns>An object containing the data represented by the input string, in the input type.</returns>
        public static Object Parse(this String input, Type type, Object @default = null) { return DDRIT.Parse(input, type, @default); }

        public static TResult To<TResult>(this String input, TResult @default = default(TResult)) { return DDRIT.To<TResult>(input, @default); }

        public static Boolean? ToBoolean(this String input) { return DDRIT.ToBoolean(input); }

        public static Boolean ToBoolean(this String input, Boolean @default) { return DDRIT.ToBoolean(input, @default); }

        public static Int16? ToInt16(this String input) { return DDRIT.ToInt16(input); }

        public static Int16 ToInt16(this String input, Int16 @default) { return DDRIT.ToInt16(input, @default); }

        public static Int16? ToInt16(this Enum input) { return DDRIT.ToInt16(input); }

        public static Int16 ToInt16(this Enum input, Int16 @default) { return DDRIT.ToInt16(input, @default); }

        public static Int32? ToInt32(this String input) { return DDRIT.ToInt32(input); }

        public static Int32 ToInt32(this String input, Int32 @default) { return DDRIT.ToInt32(input, @default); }

        public static Int32? ToInt32(this Enum input) { return DDRIT.ToInt32(input); }

        public static Int32 ToInt32(this Enum input, Int32 @default) { return DDRIT.ToInt32(input, @default); }

        public static Int64? ToInt64(this String input) { return DDRIT.ToInt64(input); }

        public static Int64 ToInt64(this String input, Int64 @default) { return DDRIT.ToInt64(input, @default); }

        public static Int64? ToInt64(this Enum input) { return DDRIT.ToInt64(input); }

        public static Int64 ToInt64(this Enum input, Int64 @default) { return DDRIT.ToInt64(input, @default); }

        public static Single? ToSingle(this String input) { return DDRIT.ToSingle(input); }

        public static Single ToSingle(this String input, Single @default) { return DDRIT.ToSingle(input, @default); }

        public static Double? ToDouble(this String input) { return DDRIT.ToDouble(input); }

        public static Double ToDouble(this String input, Double @default) { return DDRIT.ToDouble(input, @default); }

        public static Decimal? ToDecimal(this String input) { return DDRIT.ToDecimal(input); }

        public static Decimal ToDecimal(this String input, Decimal @default) { return DDRIT.ToDecimal(input, @default); }

        public static DateTime? ToDateTime(this String input) { return DDRIT.ToDateTime(input); }

        public static DateTime ToDateTime(this String input, DateTime @default) { return DDRIT.ToDateTime(input, @default); }

        public static DateTimeOffset? ToDateTimeOffset(this DateTime? input) { return DDRIT.ToDateTimeOffset(input); }

        public static DateTimeOffset ToDateTimeOffset(this DateTime input) { return DDRIT.ToDateTimeOffset(input); }

        public static DateTimeOffset? ToDateTimeOffset(this String input) { return DDRIT.ToDateTimeOffset(input); }

        public static DateTimeOffset ToDateTimeOffset(this String input, DateTime @default) { return DDRIT.ToDateTimeOffset(input, @default); }

        public static TimeSpan? ToTimeSpan(this String input) { return DDRIT.ToTimeSpan(input); }

        public static TimeSpan ToTimeSpan(this String input, TimeSpan @default) { return DDRIT.ToTimeSpan(input, @default); }
    }
}