using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Enum = System.ComponentModel;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class FileUtilitiesTests
    {
        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.FileUtilities))]
        public void GetRelativePath()
        {
            var dirShallow = new DirectoryInfo(@"C:\shallow");
            var fileShallow = new FileInfo(@"C:\shallow\file.txt");

            var dirDeep = new DirectoryInfo(@"C:\shallow\deep");
            var fileDeep = new FileInfo(@"C:\shallow\deep\file.txt");

            var dirOutside = new DirectoryInfo(@"C:\outside");
            var fileOutside = new FileInfo(@"C:\outside\file.txt");

            var deepDD = dirShallow.GetRelativePath(dirDeep);
            Assert.AreEqual(@".\deep", deepDD, true, "Unexpected child directory path");
            var deepDF = dirShallow.GetRelativePath(fileDeep);
            Assert.AreEqual(@".\deep\file.txt", deepDF, true, "Unexpected child file path");

            var deepFD = fileShallow.GetRelativePath(dirDeep);
            Assert.AreEqual(@".\deep", deepFD, true, "Unexpected child directory path");
            var deepFF = fileShallow.GetRelativePath(fileDeep);
            Assert.AreEqual(@".\deep\file.txt", deepFF, true, "Unexpected child file path");

            var outsiteDD = dirShallow.GetRelativePath(dirOutside);
            Assert.AreEqual(@"..\outside", outsiteDD, true, "Unexpected relative directory path");
            var outsideDF = dirShallow.GetRelativePath(fileOutside);
            Assert.AreEqual(@"..\outside\file.txt", outsideDF, true, "Unexpected relative file path");

            var outsideFD = fileShallow.GetRelativePath(dirOutside);
            Assert.AreEqual(@"..\outside", outsideFD, true, "Unexpected relative directory path");
            var outsideFF = fileShallow.GetRelativePath(fileOutside);
            Assert.AreEqual(@"..\outside\file.txt", outsideFF, true, "Unexpected relative file path");
        }
    }
}
