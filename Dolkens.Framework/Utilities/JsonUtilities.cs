using Newtonsoft.Json;
using System;

namespace Dolkens.Framework.Utilities
{
    public static partial class JsonUtilities
    {
        /// <summary>
        /// Attempt to serialize any object into a JSON string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToJSON(Object input, Formatting formatting = Formatting.Indented, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            return JsonConvert.SerializeObject(input, formatting, new JsonSerializerSettings { TypeNameHandling = typeNameHandling });
        }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromJSON(String input, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            return JsonConvert.DeserializeObject(input, new JsonSerializerSettings { TypeNameHandling = typeNameHandling });
        }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromJSON(String input, Type type, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            return JsonConvert.DeserializeObject(input, type, new JsonSerializerSettings { TypeNameHandling = typeNameHandling });
        }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromJSON<TResult>(String input, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            return JsonConvert.DeserializeObject<TResult>(input, new JsonSerializerSettings { TypeNameHandling = typeNameHandling });
        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.JsonUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Attempt to serialize any object into a JSON string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToJSON(this Object input, Formatting formatting = Formatting.Indented, TypeNameHandling typeNameHandling = TypeNameHandling.None) { return DDRIT.ToJSON(input, formatting, typeNameHandling); }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromJSON(this String input, TypeNameHandling typeNameHandling = TypeNameHandling.None) { return DDRIT.FromJSON(input, typeNameHandling); }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromJSON(this String input, Type type, TypeNameHandling typeNameHandling = TypeNameHandling.None) { return DDRIT.FromJSON(input, type, typeNameHandling); }

        /// <summary>
        /// Attempt to deserialize any JSON string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromJSON<TResult>(this String input, TypeNameHandling typeNameHandling = TypeNameHandling.None) { return DDRIT.FromJSON<TResult>(input, typeNameHandling); }
    }
}