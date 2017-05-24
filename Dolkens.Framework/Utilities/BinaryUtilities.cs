using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dolkens.Framework.Utilities
{
    public static class BinaryUtilities
    {
        /// <summary>
        /// Attempt to serialize any object into a Binary Encoded string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToBinary(Object input, Base64FormattingOptions base64FormattingOptions = Base64FormattingOptions.None)
        {
            if (input == null)
                return String.Empty;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, input);

                return Convert.ToBase64String(ms.ToArray(), base64FormattingOptions);
            }
        }

        /// <summary>
        /// Attempt to deserialize any Binary Encoded string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromBinary(String input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return String.Empty;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Byte[] bytes = Convert.FromBase64String(input);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Attempt to deserialize any Binary Encoded string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromBinary<TResult>(String input)
        {
            Object buffer = BinaryUtilities.FromBinary(input);

            return (buffer == null) ? default(TResult) : (TResult)buffer;

        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.BinaryUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Attempt to serialize any object into a Binary Encoded string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToBinary(this Object input) { return DDRIT.ToBinary(input); }

        /// <summary>
        /// Attempt to deserialize any Binary Encoded string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromBinary(this String input) { return DDRIT.FromBinary(input); }

        /// <summary>
        /// Attempt to deserialize any Binary Encoded string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromBinary<TResult>(this String input) { return DDRIT.FromBinary<TResult>(input); }
    }
}
