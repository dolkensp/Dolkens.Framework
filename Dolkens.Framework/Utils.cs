﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dolkens.Framework
{
    public static class Utils
    {
        /// <summary>
        /// Private DataStore for the DateTimeFormats property.
        /// </summary>
        private static String[] _dateTimeFormats = new String[] { 
            @"yyyy-MM-dd\THHmm", // 2010-12-31T2359
        };

        /// <summary>
        /// Custom DateTime formats supported by the parser.
        /// </summary>
        public static String[] DateTimeFormats
        {
            get { return Utils._dateTimeFormats; }
            set { Utils._dateTimeFormats = value; }
        }

        #region Gets the build date and time (by reading the COFF header)

        // http://msdn.microsoft.com/en-us/library/ms680313

        struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };

        public static DateTime GetBuildDateTime(Assembly assembly)
        {
            if (File.Exists(assembly.Location))
            {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
                using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    fileStream.Position = 0x3C;
                    fileStream.Read(buffer, 0, 4);
                    fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                    fileStream.Read(buffer, 0, 4); // "PE\0\0"
                    fileStream.Read(buffer, 0, buffer.Length);
                }
                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));

                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
                }
                finally
                {
                    pinnedBuffer.Free();
                }
            }
            return new DateTime();
        }

        #endregion

        public static T Min<T>(T first, T second) where T : IComparable<T>
        {
            return Comparer<T>.Default.Compare(first, second) > 0 ? second : first;
        }

        public static T Max<T>(T first, T second) where T : IComparable<T>
        {
            return Comparer<T>.Default.Compare(first, second) > 0 ? first : second;
        }
    }
}