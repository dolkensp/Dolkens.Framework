#define IMAGING
#define IMAGEPROCESSOR_BUGGED

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DDRIT = Dolkens.Framework.Utilities.ExtensionMethods;

#if SERVICESTACK
using ServiceStack;
using Serializers = ServiceStack.Text;
#else
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
#endif

using System.Drawing;
#if IMAGING
using ImageProcessor;
using ImageProcessor.Imaging;
#endif
using System.Security.Cryptography;

namespace Dolkens.Framework.Utilities
{
    /// <summary>
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2008 Peter Dolkens
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Serialization

        #region XML

        /// <summary>
        /// Attempt to serialize any object into a XML string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToXML(this Object input)
        {
            XmlSerializer serializer = new XmlSerializer(input.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using (var xmlTextWriter = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    serializer.Serialize(xmlTextWriter, input, ns);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Attempt to deserialize any XML string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromXML(this String input, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            byte[] bytes = Encoding.UTF8.GetBytes(input);

            using (var stream = new MemoryStream(bytes))
            {
                return serializer.Deserialize(stream);
            }

        }

        /// <summary>
        /// Attempt to deserialize any XML string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromXML<TResult>(this String input)
        {
            Object buffer = DDRIT.FromXML(input, typeof(TResult));

            return (buffer == null) ? default(TResult) : (TResult)buffer;
        }

        #endregion

        #region Base64

        public static String ToBase64(this Object input, Base64FormattingOptions base64FormattingOptions = Base64FormattingOptions.None)
        {
            if (input == null)
                return String.Empty;

            Type type = input.GetType();
            // typeof(TObject);

            // Add support for Nullable types
            if (type.IsNullable())
                type = type.GetGenericArguments()[0];

            MethodInfo methodInfo;

            // Throw in a little static reflection caching
            if (!ParserUtilities._byteArrayMap.TryGetValue(type, out methodInfo))
            {
                // Attempt to find inbuilt ToByteArray methods
                methodInfo = type.GetMethod("ToByteArray", new Type[] { });

                // Null the mapping if the return type is wrong
                if ((methodInfo == null) || (methodInfo.ReturnType != typeof(Byte[])))
                    ParserUtilities._byteArrayMap[type] = null;
                else
                    ParserUtilities._byteArrayMap[type] = methodInfo;
            }

            // Then use the inbuilt methods
            if (methodInfo != null)
                return Convert.ToBase64String((Byte[])methodInfo.Invoke(input, null), base64FormattingOptions);

            // Throw in a little static reflection caching
            if (!ParserUtilities._parserMap.TryGetValue(type, out methodInfo))
            {
                // Attempt to find inbuilt parsing methods
                methodInfo = type.GetMethod("Parse", new Type[] { typeof(String) });

                ParserUtilities._parserMap[type] = methodInfo;
            }

            // If there's inbuilt parsing methods, assume there is a useful ToString method
            if (methodInfo != null)
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(input.ToString()), base64FormattingOptions);

            return BinaryUtilities.ToBinary(input, base64FormattingOptions);
        }

        public static TResult FromBase64<TResult>(this String input)
        {
            Type type = typeof(TResult);

            Object buffer = DDRIT.FromBase64(input, type);

            return (buffer == null) ? default(TResult) : (TResult)buffer;
        }

        public static Object FromBase64(this String input, Type type)
        {
            // Early exit for empty values
            if (String.IsNullOrEmpty(input))
                return null;

            // Add support for Nullable types
            if (type.IsNullable())
                type = type.GetGenericArguments()[0];

            // Early exit for Byte arrays
            if (type == typeof(Byte[]))
                return Convert.FromBase64String(input);

            ConstructorInfo constructorInfo;

            // Throw in a little static reflection caching
            if (!ParserUtilities._byteConstructorMap.TryGetValue(type, out constructorInfo))
            {
                // Attempt to find inbuilt constructor that takes a byte array
                constructorInfo = type.GetConstructor(new Type[] { typeof(Byte[]) });

                // And if they return Byte[]
                ParserUtilities._byteConstructorMap[type] = constructorInfo;
            }

            // Early exit for ByteArray friendly objects
            if (constructorInfo != null)
                return Activator.CreateInstance(type, Convert.FromBase64String(input));

            MethodInfo parseMethod;

            // Throw in a little static reflection caching
            if (!ParserUtilities._parserMap.TryGetValue(type, out parseMethod))
            {
                // Attempt to find inbuilt parsing methods
                parseMethod = type.GetMethod("Parse", new Type[] { typeof(String) });

                ParserUtilities._parserMap[type] = parseMethod;
            }

            // If there's inbuilt parsing methods, assume there is a useful ToString method
            if (parseMethod != null)
                return parseMethod.Invoke(null, new Object[] { Encoding.UTF8.GetString(Convert.FromBase64String(input)) });

            return input.FromBinary();
        }

        #endregion
        
        #endregion

        #region Type Extensions

        private static BindingFlags targetFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// Get a list of all non-system dependencies for a given type.
        /// </summary>
        /// <param name="type">The type to interrogate</param>
        /// <returns>An enumerable of all the dependencies referenced by the specified type.</returns>
        public static IEnumerable<Type> GetDependencies(this Type type)
        {
            return DDRIT.GetDependencies(type, new HashSet<Type> { });
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
                foreach (var t in DDRIT.GetDependencies(type.GetGenericTypeDefinition(), dependencies))
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
                    foreach (var t in DDRIT.GetDependencies(parameter.ParameterType, dependencies))
                    {
                        yield return t;
                    }
                }
            }

            foreach (var method in type.GetMethods(targetFlags))
            {
                if (!method.IsSpecialName)
                {
                    foreach (var t in DDRIT.GetDependencies(method.ReturnType, dependencies))
                    {
                        yield return t;
                    }
                }

                foreach (var parameter in method.GetParameters())
                {
                    foreach (var t in DDRIT.GetDependencies(parameter.ParameterType, dependencies))
                    {
                        yield return t;
                    }
                }
            }

            foreach (var field in type.GetFields(targetFlags))
            {
                foreach (var t in DDRIT.GetDependencies(field.FieldType, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var property in type.GetProperties(targetFlags))
            {
                foreach (var t in DDRIT.GetDependencies(property.PropertyType, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var @interface in type.GetInterfaces())
            {
                foreach (var t in DDRIT.GetDependencies(@interface, dependencies))
                {
                    yield return t;
                }
            }

            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var t in DDRIT.GetDependencies(argument, dependencies))
                {
                    yield return t;
                }
            }

            if (type.BaseType != null)
            {
                foreach (var t in DDRIT.GetDependencies(type.BaseType, dependencies))
                {
                    yield return t;
                }
            }
        }
        
        private static Dictionary<Type, Attribute> _getAttributeLookup = null;
        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            if (_getAttributeLookup == null)
            {
                _getAttributeLookup = new Dictionary<Type, Attribute>();
            }

            if (!_getAttributeLookup.Keys.Contains(type))
            {
                TAttribute attr = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
                if (attr == null)
                    throw new Exception();
                _getAttributeLookup[type] = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
            }

            return _getAttributeLookup[type] as TAttribute;
        }

        public static String GetPropertyValue(this Object obj, PropertyInfo property)
        {
            Object propValue = property.GetValue(obj, null);
            if (propValue != null)
            {
                //special collection handling needs serialization
                if (propValue is ICollection)
                {
                    return propValue.ToJSON();
                }
                else
                {
                    //use the type converter to get the correct converter for this type
                    TypeConverter tc = TypeDescriptor.GetConverter(property.PropertyType);

                    //if this is not sufficient for the types we use, we may have to look at beefing up our type conversion
                    return tc.ConvertToInvariantString(propValue);
                }

            }
            return null;
        }

        public static Boolean IsNullable(this Type baseType)
        {
            return baseType.IsGenericType && (baseType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static String SafeToString(this Object content)
        {
            if (content == null)
                return String.Empty;

            return content.ToString();
        }

        public static String GetFriendlyTypeName(this Type type)
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

        private static void GetFriendlyTypeName(Type type, StringBuilder sb)
        {
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
        }

        #endregion

        #region File.IO Extensions

        public static String FilePathToUri(this String path)
        {
            return path.Replace("\\", "/").Replace("./", "");
        }

        public static String GetRelativePath(this DirectoryInfo sourcePath, DirectoryInfo destinationPath)
        {
            StringBuilder path = new StringBuilder(260);

            if (PathRelativePathTo(
                path,
                sourcePath.FullName,
                FILE_ATTRIBUTE_DIRECTORY,
                destinationPath.FullName,
                FILE_ATTRIBUTE_DIRECTORY) == 0)
                throw new ArgumentException("Paths must have a common prefix");

            return path.ToString();
        }

        public static String GetRelativePath(this DirectoryInfo sourcePath, FileInfo destinationPath)
        {
            StringBuilder path = new StringBuilder(260);

            if (PathRelativePathTo(
                path,
                sourcePath.FullName,
                FILE_ATTRIBUTE_DIRECTORY,
                destinationPath.FullName,
                FILE_ATTRIBUTE_NORMAL) == 0)
                throw new ArgumentException("Paths must have a common prefix");

            return path.ToString();
        }

        public static String GetRelativePath(this FileInfo sourcePath, DirectoryInfo destinationPath)
        {
            StringBuilder path = new StringBuilder(260);

            if (PathRelativePathTo(
                path,
                sourcePath.FullName,
                FILE_ATTRIBUTE_NORMAL,
                destinationPath.FullName,
                FILE_ATTRIBUTE_DIRECTORY) == 0)
                throw new ArgumentException("Paths must have a common prefix");

            return path.ToString();
        }

        public static String GetRelativePath(this FileInfo sourcePath, FileInfo destinationPath)
        {
            StringBuilder path = new StringBuilder(260);

            if (PathRelativePathTo(
                path,
                sourcePath.FullName,
                FILE_ATTRIBUTE_NORMAL,
                destinationPath.FullName,
                FILE_ATTRIBUTE_NORMAL) == 0)
                throw new ArgumentException("Paths must have a common prefix");

            return path.ToString();
        }

        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;

        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern int PathRelativePathTo(StringBuilder pszPath,
            string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);

        #endregion
        
        #region Stream Extensions

        public static Byte[] ReadAllBytes(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Int64 oldPosition = stream.Position;
                stream.Position = 0;
                stream.CopyTo(ms);
                stream.Position = oldPosition;
                return ms.ToArray();
            }
        }

        #endregion
        
        #region Natural Language Extensions

        /// <summary>
        /// Converts an input integer to its ordinal number
        /// </summary>
        /// <param name="input">The integer to convert</param>
        /// <returns>Returns a string of the ordinal</returns>
        public static String ToOrdinal(this Int32 input)
        {
            return input + ToOrdinalSuffix(input);
        }

        /// <summary>
        /// Converts an input integer to its ordinal suffix
        /// Useful if you need to format the suffix separately of the number itself
        /// </summary>
        /// <param name="input">The integer to convert</param>
        /// <returns>Returns a string of the ordinal suffix</returns>
        public static String ToOrdinalSuffix(this Int32 input)
        {
            //TODO: this only handles English ordinals - in future we may wish to consider the culture

            //note, we are allowing zeroth - http://en.wikipedia.org/wiki/Zeroth
            if (input < 0)
                throw new ArgumentOutOfRangeException("input", "Ordinal numbers cannot be negative");

            //first check special case, if the result ends in 11, 12, 13, should be th
            switch (input % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            //else we just check the last digit
            switch (input % 10)
            {
                case 1:
                    return ("st");
                case 2:
                    return ("nd");
                case 3:
                    return ("rd");
                default:
                    return ("th");
            }
        }

        #endregion

        #region Image Extensions

#if IMAGING

        public static ImageFactory WeightedCrop(this ImageFactory imageFactory, Size targetSize, Point focalPoint, Boolean forceResize = false)
        {
            imageFactory.WeightedCrop((Double)targetSize.Width / (Double)targetSize.Height, focalPoint);

            // Shrink if the image is too large, or we are upscaling
            if (forceResize || (targetSize.Height < imageFactory.Image.Height) || (targetSize.Width < imageFactory.Image.Width))
                imageFactory.Resize(targetSize);

            return imageFactory;
        }

        public static ImageFactory WeightedCrop(this ImageFactory imageFactory, Double targetAspectRatio, Point focalPoint)
        {
            Double focalX = focalPoint.X;
            Double focalY = focalPoint.Y;

            Double centerX = imageFactory.Image.Width / 2;
            Double centerY = imageFactory.Image.Height / 2;

            Boolean cropX = (imageFactory.Image.Width / imageFactory.Image.Height) > targetAspectRatio;

            Boolean leftHalf = (focalPoint.X < centerX);
            Boolean topHalf = (focalPoint.Y < centerY);

            // How far around the center point do we expand for maximum coverage
            Double expandX = cropX ? centerX : (imageFactory.Image.Height * targetAspectRatio / 2);
            Double expandY = cropX ? (imageFactory.Image.Width / targetAspectRatio / 2) : centerY;

            // Ensure we're not too tall
            if (expandY > centerY)
            {
                expandY = centerY;
                expandX = expandY * targetAspectRatio;
            }

            // Ensure we're not too wide
            if (expandX > centerX)
            {
                expandX = centerX;
                expandY = expandX / targetAspectRatio;
            }

            if (focalX - expandX < 0)
                focalX = expandX;

            if (focalX + expandX > imageFactory.Image.Width)
                focalX = imageFactory.Image.Width - expandX;

            if (focalY - expandY < 0)
                focalY = expandY;

            if (focalY + expandY > imageFactory.Image.Height)
                focalY = imageFactory.Image.Height - expandY;

#if IMAGEPROCESSOR_BUGGED
            // The bugged version adds your initial point to your width calculation if you try to center your crops

            imageFactory.Crop(new CropLayer(
                Convert.ToSingle(focalX - expandX),
                Convert.ToSingle(focalY - expandY),
                imageFactory.Image.Width,
                imageFactory.Image.Height,
                CropMode.Pixels
                ));

            imageFactory.Crop(new CropLayer(
                0,
                0,
                Convert.ToSingle(expandX * 2),
                Convert.ToSingle(expandY * 2),
                CropMode.Pixels
                ));
#else
            imageFactory.Crop(new CropLayer(
                Convert.ToSingle(focalX - expandX),
                Convert.ToSingle(focalY - expandY),
                Convert.ToSingle(focalX + expandX),
                Convert.ToSingle(focalY + expandY),
                CropMode.Pixels
                );
#endif

            return imageFactory;
        }

        /// <summary>
        /// Gets the largest possible crop, centered on the focal point
        /// </summary>
        /// <param name="currentSize">The current dimensions of the image</param>
        /// <param name="focalPoint">The center-point of the image</param>
        /// <param name="targetAspectRatio">The target aspect ratio (width / height)</param>
        /// <returns></returns>
        private static Rectangle GetCenteredCrop(Size currentSize, Point focalPoint, Double targetAspectRatio)
        {
            Boolean leftHalf = (focalPoint.X < currentSize.Width / 2);
            Boolean topHalf = (focalPoint.Y < currentSize.Height / 2);

            Double expandX = leftHalf ? focalPoint.X : (currentSize.Width - focalPoint.X);
            Double expandY = topHalf ? focalPoint.Y : (currentSize.Height - focalPoint.Y);

            Boolean cropX = (expandX / expandY) > targetAspectRatio;

            Double adjustedX = cropX ? expandX : (expandY * targetAspectRatio);
            Double adjustedY = cropX ? (expandX * targetAspectRatio) : expandY;

            if (adjustedX > expandX)
            {
                adjustedX = expandX;
                adjustedY = adjustedX / targetAspectRatio;
            }
            else if (adjustedY > expandY)
            {
                adjustedY = expandY;
                adjustedX = adjustedY * targetAspectRatio;
            }

            return new Rectangle(
                Convert.ToInt32(focalPoint.X - adjustedX),
                Convert.ToInt32(focalPoint.Y - adjustedY),
                Convert.ToInt32(adjustedX * 2),
                Convert.ToInt32(adjustedY * 2));
        }

        public static ImageFactory Optimize(this ImageFactory imageFactory, Stream outStream, Int32 quality = 75)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageFactory.Save(ms);

                ms.Seek(0, SeekOrigin.Begin);

                using (BitMiracle.LibJpeg.JpegImage image = new BitMiracle.LibJpeg.JpegImage(ms))
                {
                    image.WriteJpeg(
                        outStream,
                        new BitMiracle.LibJpeg.CompressionParameters
                        {
                            Quality = quality,
                            SimpleProgressive = true,
                            SmoothingFactor = 0
                        });
                }
            }

            return imageFactory;
        }

#endif

        #endregion
    }
}

#region Namespace Proxies

namespace System
{
    public static partial class _Proxy
    {
        /// <summary>
        /// Attempt to serialize any object into a XML string.
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>A string containing the serialized object.</returns>
        public static String ToXML(this Object input) { return DDRIT.ToXML(input); }

        /// <summary>
        /// Attempt to deserialize any XML string into an object.
        /// </summary>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An Object deserialized from the string.</returns>
        public static Object FromXML(this String input, Type type) { return DDRIT.FromXML(input, type); }

        /// <summary>
        /// Attempt to deserialize any XML string into an object.
        /// </summary>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        /// <param name="input">The string to deserialize to an object.</param>
        /// <returns>An Object of type T deserialized from the string.</returns>
        public static TResult FromXML<TResult>(this String input) { return DDRIT.FromXML<TResult>(input); }

        /// <summary>
        /// Attempts to serialize any object into a Binary Encoded string.
        /// Produces slightly shorter output for types that implement a Parse and ToString method, and objects of type Byte[].
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ToBase64(this Object input) { return DDRIT.ToBase64(input); }

        public static TResult FromBase64<TResult>(this String input) { return DDRIT.FromBase64<TResult>(input); }

        public static Object FromBase64(this String input, Type type) { return DDRIT.FromBase64(input, type); }

        public static Boolean IsNullable(this Type baseType) { return DDRIT.IsNullable(baseType); }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute { return DDRIT.GetAttribute<TAttribute>(type); }

        /// <summary>
        /// Get a list of all non-system dependencies for a given type.
        /// </summary>
        /// <param name="type">The type to interrogate</param>
        /// <returns>An enumerable of all the dependencies referenced by the specified type.</returns>
        public static IEnumerable<Type> GetDependencies(this Type type) { return DDRIT.GetDependencies(type); }

        public static String GetFriendlyTypeName(this Type type) { return DDRIT.GetFriendlyTypeName(type); }

        public static String GetPropertyValue(this Object obj, PropertyInfo property) { return DDRIT.GetPropertyValue(obj, property); }

        public static String FilePathToUri(this String path) { return DDRIT.FilePathToUri(path); }

        public static Byte[] ReadAllBytes(this Stream stream) { return DDRIT.ReadAllBytes(stream); }

        /// <summary>
        /// Converts an input integer to its ordinal number
        /// </summary>
        /// <param name="input">The integer to convert</param>
        /// <returns>Returns a string of the ordinal</returns>
        public static String ToOrdinal(this Int32 input) { return DDRIT.ToOrdinal(input); }

        /// <summary>
        /// Converts an input integer to its ordinal suffix
        /// Useful if you need to format the suffix separately of the number itself
        /// </summary>
        /// <param name="input">The integer to convert</param>
        /// <returns>Returns a string of the ordinal suffix</returns>
        public static String ToOrdinalSuffix(this Int32 input) { return DDRIT.ToOrdinalSuffix(input); }

    }
}

namespace System.IO
{
    public static class _Proxy
    {
        public static String GetRelativePath(this DirectoryInfo sourcePath, DirectoryInfo destinationPath) { return DDRIT.GetRelativePath(sourcePath, destinationPath); }

        public static String GetRelativePath(this DirectoryInfo sourcePath, FileInfo destinationPath) { return DDRIT.GetRelativePath(sourcePath, destinationPath); }

        public static String GetRelativePath(this FileInfo sourcePath, DirectoryInfo destinationPath) { return DDRIT.GetRelativePath(sourcePath, destinationPath); }

        public static String GetRelativePath(this FileInfo sourcePath, FileInfo destinationPath) { return DDRIT.GetRelativePath(sourcePath, destinationPath); }
    }
}

#if IMAGING
namespace ImageProcessor
{
    public static class _Proxy
    {
        public static ImageFactory WeightedCrop(this ImageFactory imageFactory, Size targetSize, Point focalPoint, Boolean forceResize = false) { return DDRIT.WeightedCrop(imageFactory, targetSize, focalPoint, forceResize); }

        public static ImageFactory WeightedCrop(this ImageFactory imageFactory, Double targetAspectRatio, Point focalPoint) { return DDRIT.WeightedCrop(imageFactory, targetAspectRatio, focalPoint); }

        public static ImageFactory Optimize(this ImageFactory imageFactory, Stream outStream, Int32 quality = 75) { return DDRIT.Optimize(imageFactory, outStream, quality); }
    }
}
#endif

#endregion