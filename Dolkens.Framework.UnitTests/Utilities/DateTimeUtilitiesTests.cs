using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class DateTimeUtilitiesTests
    {
        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.DateTimeUtilities))]
        public void StartOfWeek()
        {
            var day1 = new DateTime(2017, 5, 21);
            var day2 = new DateTime(2017, 5, 22);
            var day3 = new DateTime(2017, 5, 22);
            var day4 = new DateTime(2017, 5, 24);
            var day5 = new DateTime(2017, 5, 25);
            var day6 = new DateTime(2017, 5, 26);
            var day7 = new DateTime(2017, 5, 27);

            var day8 = new DateTime(2017, 5, 28);
            var day9 = new DateTime(2017, 5, 29);

            #region StartOfWeek()

            var startSun_Wed = day4.StartOfWeek();
            Assert.AreEqual(day1.Date, startSun_Wed.Date, "StartOfWeek() returned unexpected date.");

            var startSun_Sun = day1.StartOfWeek();
            Assert.AreEqual(day1.Date, startSun_Sun.Date, "StartOfWeek() returned unexpected date.");

            var startSun_Sat = day7.StartOfWeek();
            Assert.AreEqual(day1.Date, startSun_Sat.Date, "StartOfWeek() returned unexpected date.");

            #endregion

            #region StartOfWeek(DayOfWeek.Monday)

            var startMon_Wed = day4.StartOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day2.Date, startMon_Wed.Date, "StartOfWeek(DayOfWeek.Monday) returned unexpected date.");

            var startMon_Sun = day2.StartOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day2.Date, startMon_Sun.Date, "StartOfWeek(DayOfWeek.Monday) returned unexpected date.");

            var startMon_Sat = day8.StartOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day2.Date, startMon_Sat.Date, "StartOfWeek(DayOfWeek.Monday) returned unexpected date.");

            #endregion
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.DateTimeUtilities))]
        public void EndOfWeek()
        {
            var day1 = new DateTime(2017, 5, 21);
            var day2 = new DateTime(2017, 5, 22);
            var day3 = new DateTime(2017, 5, 22);
            var day4 = new DateTime(2017, 5, 24);
            var day5 = new DateTime(2017, 5, 25);
            var day6 = new DateTime(2017, 5, 26);
            var day7 = new DateTime(2017, 5, 27);

            var day8 = new DateTime(2017, 5, 28);
            var day9 = new DateTime(2017, 5, 29);

            #region EndOfWeek()

            var endSun_Wed = day4.EndOfWeek();
            Assert.AreEqual(day7.Date, endSun_Wed.Date, "EndOfWeek() returned unexpected date.");

            var endSun_Sun = day1.EndOfWeek();
            Assert.AreEqual(day7.Date, endSun_Sun.Date, "EndOfWeek() returned unexpected date.");

            var endSun_Sat = day7.EndOfWeek();
            Assert.AreEqual(day7.Date, endSun_Sat.Date, "EndOfWeek() returned unexpected date.");

            #endregion

            #region EndOfWeek(DayOfWeek.Monday)

            var endMon_Wed = day4.EndOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day8.Date, endMon_Wed.Date, "EndOfWeek(DayOfWeek.Monday) returned unexpected date.");

            var endMon_Sun = day2.EndOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day8.Date, endMon_Sun.Date, "EndOfWeek(DayOfWeek.Monday) returned unexpected date.");

            var endMon_Sat = day8.EndOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(day8.Date, endMon_Sat.Date, "EndOfWeek(DayOfWeek.Monday) returned unexpected date.");

            #endregion
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.DateTimeUtilities))]
        public void TrimTo()
        {
            throw new NotImplementedException();
        }
    }
}