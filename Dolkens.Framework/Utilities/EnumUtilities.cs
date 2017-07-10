using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dolkens.Framework.Utilities
{
    internal static class EnumUtilities<TEnum> where TEnum : struct
    {
        private static Bitwise bitwise = new Bitwise(typeof(TEnum));
        
        public static Boolean HasFlag(Object value, Object flag)
        {
            return bitwise.EQUALS(bitwise.AND(value, flag), flag);
        }

        public static TEnum AddFlag(Object value, Object flag)
        {
            return (TEnum)bitwise.OR(value, flag);
        }
        
        public static IEnumerable<TEnum> ToFlags(Enum value)
        {
            Object match = 0;

            if (bitwise.EQUALS(value, 0))
            {
                yield return (TEnum)(Object)value;
            }
            else
            {
                foreach (var flag in Enum.GetValues(typeof(TEnum)).Cast<TEnum>().OrderByDescending(e => e))
                {
                    if (EnumUtilities<TEnum>.HasFlag(value, flag) && // If the flag is a match
                       !bitwise.EQUALS(flag, 0))                     // and the flag isn't equal to 0 (special case)
                    {
                        match = bitwise.OR(match, flag);

                        yield return flag;
                    }
                }
            }
        }
    }

    public static class EnumUtilities
    {
        private static IEnumerable<Enum> ToFlags(Enum value)
        {
            var enumType = value.GetType();
            var underlyingType = Enum.GetUnderlyingType(enumType);
            Object match = 0;

            var bitwise = new Bitwise(enumType);
            
            if (bitwise.EQUALS(value, 0))
            {
                yield return value;
            }
            else
            {
                foreach (var flag in Enum.GetValues(enumType).Cast<Enum>().OrderByDescending(e => e))
                {
                    if (bitwise.EQUALS(bitwise.AND(value, flag), flag) && // If the flag is a match
                       !bitwise.EQUALS(flag, 0))                          // and the flag isn't equal to 0 (special case)
                    {
                        yield return flag;
                        match = bitwise.OR(match, flag);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an Enum Flag, to a list of Enums
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum[] ToFlagsArray<TEnum>(this Enum value) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException($"`{typeof(TEnum).Name}` is not a System.Enum", "value");

            return EnumUtilities<TEnum>.ToFlags(value).ToArray();
        }

        /// <summary>
        /// Converts a list of Enum Flags, to a single Enum.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static TEnum FromFlagsArray<TEnum>(this IEnumerable<TEnum> flags) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException($"`{typeof(TEnum).Name}` is not a System.Enum", "flags");

            var match = default(TEnum);

            foreach (var flag in flags.OrderByDescending(e => e))
            {
                match = EnumUtilities<TEnum>.AddFlag(match, flag);
            }

            return match;
        }

        public static String ToDescription(Enum value, Boolean expandFlags = true)
        {
            if (value == null) return String.Empty;

            var enumType = value.GetType();

            if (expandFlags && enumType.IsDefined(typeof(FlagsAttribute), true))
            {
                return String.Join(", ", EnumUtilities.ToFlags(value).Select(e =>
                {
                    var fieldName = e.ToString();

                    DescriptionAttribute[] attributes = (DescriptionAttribute[])enumType
                        ?.GetField(fieldName)
                        ?.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    return attributes?.Length > 0 ? attributes[0].Description : fieldName;
                }));
            }
            else
            {
                var fieldName = value.ToString();

                DescriptionAttribute[] attributes = (DescriptionAttribute[])enumType
                    ?.GetField(fieldName)
                    ?.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes?.Length > 0 ? attributes[0].Description : fieldName;
            }
        }

        public static TEnum ToEnum<TEnum>(Enum value, TEnum @default) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException($"`{typeof(TEnum).Name}` is not a System.Enum");

            return value.ToString().ToEnum<TEnum>(@default, true);
        }

        public static TEnum? ToEnum<TEnum>(Enum value, TEnum? @default) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException($"`{typeof(TEnum).Name}` is not a System.Enum");

            return value.ToString().ToEnum<TEnum>(true) ?? @default;
        }

        public static TEnum? ToEnum<TEnum>(String input, Boolean ignoreCase = true) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException($"`{typeof(TEnum).Name}` is not a System.Enum");

            TEnum buffer;

            if (Enum.TryParse<TEnum>(input, ignoreCase, out buffer))
                return buffer;

            return null;
        }

        public static TResult ToEnum<TResult>(String input, TResult @default, Boolean ignoreCase = true) where TResult : struct
        {
            return input.ToEnum<TResult>(ignoreCase) ?? @default;
        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.EnumUtilities;

    public static partial class _Proxy
    {
        public static TEnum[] ToFlagsArray<TEnum>(this Enum value) where TEnum : struct { return DDRIT.ToFlagsArray<TEnum>(value); }

        public static TEnum FromFlagsArray<TEnum>(this IEnumerable<TEnum> flags) where TEnum : struct { return DDRIT.FromFlagsArray<TEnum>(flags); }

        /// <summary>
        /// Return the friendly description of an enum value, if it has been decorated with the DescriptionAttribute,
        /// otherwise, return the internal string representation of the enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToDescription(this Enum value, Boolean expandFlags = true) { return DDRIT.ToDescription(value, expandFlags); }

        public static TEnum ToEnum<TEnum>(this Enum value, TEnum @default) where TEnum : struct { return DDRIT.ToEnum<TEnum>(value, @default); }

        public static TEnum? ToEnum<TEnum>(this Enum value, TEnum? @default) where TEnum : struct { return DDRIT.ToEnum<TEnum>(value, @default); }

        public static TEnum ToEnum<TEnum>(this String input, TEnum @default, Boolean ignoreCase = true) where TEnum : struct { return DDRIT.ToEnum<TEnum>(input, @default, ignoreCase); }

        public static TEnum? ToEnum<TEnum>(this String input, Boolean ignoreCase = true) where TEnum : struct { return DDRIT.ToEnum<TEnum>(input, ignoreCase); }
    }
}