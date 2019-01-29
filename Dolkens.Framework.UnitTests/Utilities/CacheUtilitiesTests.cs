using Dolkens.Framework.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class CacheUtilitiesTests
    {
        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Caching.CacheUtils))]
        public void GetCachedData()
        {
            var firstCallCount = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, this.TestMethod);
            var secondCallCount = 0;
            var thirdCallCount = 0;

            var complete = 0;

            var thread1 = new Thread(() =>
            {
                secondCallCount = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, this.TestMethod);

                complete++;
            });

            var thread2 = new Thread(() =>
            {
                thirdCallCount = CacheUtils.GetCachedData(CacheUtils.DefaultSettings, this.TestMethod);

                complete++;
            });

            thread1.Start();
            thread2.Start();

            while (complete < 2) Thread.SpinWait(10);

            Assert.AreEqual(1, firstCallCount, "Cached method has run an unexpected number of times");
            Assert.AreEqual(1, secondCallCount, "Cached method has run an unexpected number of times");
            Assert.AreEqual(1, thirdCallCount, "Cached method has run an unexpected number of times");
        }

        private static Int32 _hasRun = 0;

        private Int32 TestMethod()
        {
            return ++_hasRun;
        }
    }
}