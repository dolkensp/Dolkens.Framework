using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Utilities
{
    public class TypeUtilities
    {
        private static BindingFlags targetFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        private static Dictionary<Type, Attribute> _getAttributeLookup = new Dictionary<Type, Attribute> { };

        /// <summary>
        /// Get a list of all non-system dependencies for a given type.
        /// </summary>
        /// <param name="type">The type to interrogate</param>
        /// <returns>An enumerable of all the dependencies referenced by the specified type.</returns>
        public static IEnumerable<Type> GetDependencies(Type type)
        {
            return TypeUtilities.GetDependencies(type, new HashSet<Type> { });
        }

        public static TAttribute GetAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (!_getAttributeLookup.Keys.Contains(type))
            {
                TAttribute attr = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
                if (attr == null)
                    throw new Exception();
                _getAttributeLookup[type] = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
            }

            return _getAttributeLookup[type] as TAttribute;
        }

        //public static String GetPropertyValue(Object obj, PropertyInfo property)
        //{
        //    Object propValue = property.GetValue(obj, null);
        //    if (propValue != null)
        //    {
        //        //special collection handling needs serialization
        //        if (propValue is ICollection)
        //        {
        //            return propValue.ToJSON();
        //        }
        //        else
        //        {
        //            //use the type converter to get the correct converter for this type
        //            TypeConverter tc = TypeDescriptor.GetConverter(property.PropertyType);

        //            //if this is not sufficient for the types we use, we may have to look at beefing up our type conversion
        //            return tc.ConvertToInvariantString(propValue);
        //        }

        //    }
        //    return null;
        //}

        public static Boolean IsNullable(Type baseType)
        {
            return baseType.IsGenericType && (baseType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static String GetFriendlyTypeName(Type type)
        {
            StringBuilder sb = new StringBuilder();

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();

                sb.AppendFormat("{0}.{1}<", genericType.Namespace, genericType.Name);

                Boolean first = true;

                foreach (Type typeArgument in type.GetGenericArguments())
                {
                    if (!first)
                        sb.Append(",");

                    GetFriendlyTypeName(typeArgument, sb);

                    first = false;
                }

                sb.Append(">");
            }
            else
                sb.AppendFormat("{0}.{1}", type.Namespace, type.Name);

            return sb.ToString();
        }

        private static IEnumerable<Type> GetDependencies(Type type, HashSet<Type> dependencies)
        {
            if (type == null || type.IsGenericParameter ||
                (type.FullName != null && type.FullName.StartsWith("System.")) ||
                (type.Namespace != null && type.Namespace.StartsWith("System.")) ||
                dependencies.Contains(type))
                yield break;

            dependencies.Add(type);

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                foreach (var t in TypeUtilities.GetDependencies(type.GetGenericTypeDefinition(), dependencies))
                {
                    yield return t;
                }
            }
            else
            {
                yield return type;
            }

            foreach (var constructor in type.GetConstructors())
            {
                foreach (var parameter in constructor.GetParameters())
                {
                    foreach (var t in TypeUtilities.GetDependencies(parameter.ParameterType, dependencies))
                    {
                        yield return t;
                    }
                }
            }

            foreach (var method in type.GetMethods(targetFlags))
            {
                if (!method.IsSpecialName)
                {
                    foreach (var t in TypeUtilities.GetDependencies(method.ReturnType, dependencies))
                    {
                        yield return t;
                    }
                }

                foreach (var parameter in method.GetParameters())
                {
                    foreach (var t in TypeUtilities.GetDependencies(parameter.ParameterType, dependencies))
                    {
                        yield return t;
                    }
                }
            }

            foreach (var field in type.GetFields(targetFlags))
            {
                foreach (var t in TypeUtilities.GetDependencies(field.FieldType, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var property in type.GetProperties(targetFlags))
            {
                foreach (var t in TypeUtilities.GetDependencies(property.PropertyType, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var @interface in type.GetInterfaces())
            {
                foreach (var t in TypeUtilities.GetDependencies(@interface, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var t in TypeUtilities.GetDependencies(argument, dependencies))
                {
                    yield return t;
                }
            }

            if (type.BaseType != null)
            {
                foreach (var t in TypeUtilities.GetDependencies(type.BaseType, dependencies))
                {
                    yield return t;
                }
            }
        }

        private static void GetFriendlyTypeName(Type type, StringBuilder sb)
        {
            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();

                sb.AppendFormat("{0}.{1}<", genericType.Namespace, genericType.Name);

                Boolean first = true;

                foreach (Type typeArgument in type.GetGenericArguments())
                {
                    if (!first) sb.Append(",");

                    TypeUtilities.GetFriendlyTypeName(typeArgument, sb);

                    first = false;
                }

                sb.Append(">");
            }
            else
            {
                sb.AppendFormat("{0}.{1}", type.Namespace, type.Name);
            }
        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.TypeUtilities;

    public static partial class _Proxy
    {
        public static Boolean IsNullable(this Type baseType) { return DDRIT.IsNullable(baseType); }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute { return DDRIT.GetAttribute<TAttribute>(type); }

        /// <summary>
        /// Get a list of all non-system dependencies for a given type.
        /// </summary>
        /// <param name="type">The type to interrogate</param>
        /// <returns>An enumerable of all the dependencies referenced by the specified type.</returns>
        public static IEnumerable<Type> GetDependencies(this Type type) { return DDRIT.GetDependencies(type); }

        public static String GetFriendlyTypeName(this Type type) { return DDRIT.GetFriendlyTypeName(type); }

        // public static String GetPropertyValue(this Object obj, PropertyInfo property) { return DDRIT.GetPropertyValue(obj, property); }
    }
}