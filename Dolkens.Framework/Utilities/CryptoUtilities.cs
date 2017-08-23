using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Dolkens.Framework.Utilities
{
    public static class CryptoUtilities
    {
        /// <summary>
        /// From http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa/14333437#14333437
        /// </summary>
        /// <param name="bytes">Array of bytes to convert to hex string</param>
        /// <returns>A hex string representation of the input bytes</returns>
        public static String ToHex(Byte[] bytes)
        {
            Char[] buffer = new Char[bytes.Length * 2];
            Int32 b;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                buffer[i * 2] = (Char)(87 + b + (((b - 10) >> 31) & -39));
                b = bytes[i] & 0xF;
                buffer[i * 2 + 1] = (Char)(87 + b + (((b - 10) >> 31) & -39));
            }

            return new String(buffer);
        }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An MD5 hash of the given string</returns>
        public static String ToMD5(String input)
        {
            using (var md5 = MD5.Create())
            {
                Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                Byte[] hash = md5.ComputeHash(inputBytes);

                return hash.ToHex();
            }
        }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA1 hash of the given string</returns>
        public static String ToSHA1(String input)
        {
            using (var sha = SHA1.Create())
            {
                Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                Byte[] hash = sha.ComputeHash(inputBytes);

                return hash.ToHex();
            }
        }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA256 hash of the given string</returns>
        public static String ToSHA256(String input)
        {
            using (var sha = SHA256.Create())
            {
                Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                Byte[] hash = sha.ComputeHash(inputBytes);

                return hash.ToHex();
            }
        }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA384 hash of the given string</returns>
        public static String ToSHA384(String input)
        {
            using (var sha = SHA384.Create())
            {
                Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                Byte[] hash = sha.ComputeHash(inputBytes);

                return hash.ToHex();
            }
        }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA512 hash of the given string</returns>
        public static String ToSHA512(String input)
        {
            using (var sha = SHA512.Create())
            {
                Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                Byte[] hash = sha.ComputeHash(inputBytes);

                return hash.ToHex();
            }
        }

        ///// <summary>
        ///// Return the hash of a string value
        ///// </summary>
        ///// <param name="input">The string to hash</param>
        ///// <returns>An HMACSHA1 hash of the given string</returns>
        //public static String ToHMACMD5(this String input, String key)
        //{
        //    using (var sha = HMACMD5.Create())
        //    {
        //        Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        //        Byte[] hash = sha.ComputeHash(inputBytes);

        //        return hash.ToHex();
        //    }
        //}

        ///// <summary>
        ///// Return the hash of a string value
        ///// </summary>
        ///// <param name="input">The string to hash</param>
        ///// <returns>An HMACSHA1 hash of the given string</returns>
        //public static String ToHMACSHA1(this String input, String key)
        //{
        //    using (var sha = HMACSHA1.Create())
        //    {
        //        Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        //        Byte[] hash = sha.ComputeHash(inputBytes);

        //        return hash.ToHex();
        //    }
        //}

        ///// <summary>
        ///// Return the hash of a string value
        ///// </summary>
        ///// <param name="input">The string to hash</param>
        ///// <returns>An HMACSHA384 hash of the given string</returns>
        //public static String ToHMACSHA256(this String input, String key)
        //{
        //    using (var sha = HMACSHA384.Create())
        //    {
        //        Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        //        Byte[] hash = sha.ComputeHash(inputBytes);

        //        return hash.ToHex();
        //    }
        //}

        ///// <summary>
        ///// Return the hash of a string value
        ///// </summary>
        ///// <param name="input">The string to hash</param>
        ///// <returns>An HMACSHA384 hash of the given string</returns>
        //public static String ToHMACSHA384(this String input, String key)
        //{
        //    using (var sha = HMACSHA384.Create())
        //    {
        //        Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        //        Byte[] hash = sha.ComputeHash(inputBytes);

        //        return hash.ToHex();
        //    }
        //}

        ///// <summary>
        ///// Return the hash of a string value
        ///// </summary>
        ///// <param name="input">The string to hash</param>
        ///// <returns>An HMACSHA512 hash of the given string</returns>
        //public static String ToHMACSHA512(this String input, String key)
        //{
        //    using (var sha = HMACSHA512.Create())
        //    {
        //        Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        //        Byte[] hash = sha.ComputeHash(inputBytes);

        //        return hash.ToHex();
        //    }
        //}
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.CryptoUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Converts a the input <paramref name="bytes"/> to a hex string.
        /// </summary>
        /// <param name="bytes">Array of bytes to convert to hex string.</param>
        /// <returns>A hex string representation of the input <paramref name="bytes"/>.</returns>
        public static String ToHex(this Byte[] bytes) { return DDRIT.ToHex(bytes); }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An MD5 hash of the given string</returns>
        public static String ToMD5(this String input) { return DDRIT.ToMD5(input); }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA1 hash of the given string</returns>
        public static String ToSHA1(this String input) { return DDRIT.ToSHA1(input); }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA256 hash of the given string</returns>
        public static String ToSHA256(this String input) { return DDRIT.ToSHA256(input); }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA384 hash of the given string</returns>
        public static String ToSHA384(this String input) { return DDRIT.ToSHA384(input); }

        /// <summary>
        /// Return the hash of a string value
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>An SHA512 hash of the given string</returns>
        public static String ToSHA512(this String input) { return DDRIT.ToSHA512(input); }
    }

}
