using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework.Utilities
{
    public static class FileUtilities
    {
        public static String FilePathToUri(String path)
        {
            return path.Replace("\\", "/").Replace("./", "");
        }
        
        public static String GetRelativePath(FileSystemInfo sourcePath, FileSystemInfo destinationPath)
        {
            StringBuilder path = new StringBuilder(FILE_MAXLENGTH);
            
            if (FileUtilities.PathRelativePathTo(
                path,
                sourcePath.FullName,
                sourcePath.GetAttributes(),
                destinationPath.FullName,
                destinationPath.GetAttributes()) == 0)
                throw new ArgumentException("Paths must have a common prefix");

            return path.ToString();
        }

        private static FileAttributes GetAttributes(this FileSystemInfo fso)
        {
            if (fso.Attributes >= FileAttributes.ReadOnly) return fso.Attributes;
            if (fso is FileInfo) return FileAttributes.Normal;
            if (fso is DirectoryInfo) return FileAttributes.Directory;
            return fso.Attributes;
        }

        private const Int32 FILE_MAXLENGTH = 0x104;

        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern Int32 PathRelativePathTo(StringBuilder pszPath, String pszFrom, FileAttributes dwAttrFrom, String pszTo, FileAttributes dwAttrTo);
    }
}

namespace System.IO
{
    using DDRIT = Dolkens.Framework.Utilities.FileUtilities;

    public static partial class _Proxy
    {
        public static String FilePathToUri(this String path) { return DDRIT.FilePathToUri(path); }

        public static String GetRelativePath(this FileSystemInfo sourcePath, FileSystemInfo destinationPath) { return DDRIT.GetRelativePath(sourcePath, destinationPath); }
    }
}