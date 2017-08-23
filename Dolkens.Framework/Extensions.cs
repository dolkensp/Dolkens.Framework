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
using DDRIT = Dolkens.Framework.Extensions.ExtensionMethods;

#if SERVICESTACK
using ServiceStack;
using Serializers = ServiceStack.Text;
#else
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
#endif

using System.Drawing;
using Dolkens.Framework.Utilities;
#if IMAGING
using ImageProcessor;
using ImageProcessor.Imaging;
#endif
using System.Security.Cryptography;

namespace Dolkens.Framework.Extensions
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

        #endregion

        public static String SafeToString(this Object content)
        {
            if (content == null)
                return String.Empty;

            return content.ToString();
        }

        #region Stream Extensions

        public static Byte[] ReadAllBytes(this Stream stream, Int32 bufferSize = 81920)
        {
            using (var ms = new MemoryStream { })
            {
                var oldPosition = stream.Position;
                stream.Position = 0;
                stream.CopyTo(ms, bufferSize);
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
            return $"{input}{input.ToOrdinalSuffix()}";
        }

        /// <summary>
        /// Converts an input integer to its ordinal suffix
        /// Useful if you need to format the suffix separately of the number itself
        /// </summary>
        /// <param name="input">The integer to convert</param>
        /// <returns>Returns a string of the ordinal suffix</returns>
        public static String ToOrdinalSuffix(this Int32 input)
        {
            // TODO: this only handles English ordinals - in future we may wish to consider the culture

            // note, we are allowing zeroth - http://en.wikipedia.org/wiki/Zeroth
            if (input < 0)
                throw new ArgumentOutOfRangeException("input", "Ordinal numbers cannot be negative");

            // first check special case, if the result ends in 11, 12, 13, should be th
            switch (input % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            // else we just check the last digit
            switch (input % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
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