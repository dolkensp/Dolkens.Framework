using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dolkens.Framework.Telemetry
{
    internal class LastException
    {
        static FieldInfo myThreadPointerFieldInfo;
        static int myThreadOffset = -1;

        static LastException()
        {
            if (myThreadPointerFieldInfo == null)
            {
                // Dont read the don´t or you will get scared. 
                // I prefer to read this more like: If you dont know the rules you will
                // get biten by the next .NET Framework update
                myThreadPointerFieldInfo = typeof(Thread).GetField("DONT_USE_InternalThread", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if (myThreadOffset == -1)
            {
                ClrContext context = ClrContext.None;
                context |= (IntPtr.Size == 8) ? ClrContext.Is64Bit : ClrContext.Is32Bit;
                context |= (Environment.Version.Major == 2) ? ClrContext.IsNet2 : ClrContext.None;
                context |= (Environment.Version.Major == 4) ? ClrContext.IsNet4 : ClrContext.None;
                context |= (Environment.Version.Major == 4 && Environment.Version.Revision == 42000) ? ClrContext.IsNet4_6 : ClrContext.None;

                switch (context)
                {
                    case ClrContext.Is32Bit | ClrContext.IsNet4 | ClrContext.IsNet4_6:
                        myThreadOffset = 0x18C;
                        break;
                    case ClrContext.Is32Bit | ClrContext.IsNet4:
                        myThreadOffset = 0x188;
                        break;
                    case ClrContext.Is32Bit | ClrContext.IsNet2:
                        myThreadOffset = 0x180;
                        break;
                    case ClrContext.Is64Bit | ClrContext.IsNet4 | ClrContext.IsNet4_6:
                        myThreadOffset = 0x258;
                        break;
                    case ClrContext.Is64Bit | ClrContext.IsNet4:
                        myThreadOffset = 0x250;
                        break;
                    case ClrContext.Is64Bit | ClrContext.IsNet2:
                        myThreadOffset = 0x240;
                        break;
                }
            }
        }

        public static Exception GetLastException()
        {
            var value = (IntPtr)myThreadPointerFieldInfo.GetValue(Thread.CurrentThread);
            var exHandle = Marshal.ReadIntPtr(value, myThreadOffset);
            if (exHandle == IntPtr.Zero)
                return null;
            var result = GCHandle.FromIntPtr(exHandle);
            return (Exception)result.Target;
        }
    }

    /// <summary>
    /// Flags used to determine which offset is needed to locate the exception
    /// </summary>
    [Flags]
    internal enum ClrContext
    {
        None,
        Is32Bit = 1,
        Is64Bit = 2,
        IsNet2 = 4,
        IsNet4 = 8,
        IsNet4_6 = 16,
    }
}
