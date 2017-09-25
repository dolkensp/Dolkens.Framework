using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enum = System.ComponentModel;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class EnumUtilitiesTests
    {
        private enum TestEnum
        {
            TestNoDescription, // = 0
            TestNoDescriptionWithValue = 4,

            [Enum.Description("Test Description")]
            TestDescription, // = 5
            [Enum.Description("Test Description With Value")]
            TestDescriptionWithValue = 8,
        }

        [Flags]
        private enum TestFlags
        {
            FlagsNoDescription, // = 0
            FlagsNoDescriptionWithValue = 4,

            [Enum.Description("Flags Description")]
            FlagsDescription, // = 5
            [Enum.Description("Flags Description With Value")]
            FlagsDescriptionWithValue = 8,

            OtherValue = 16,

            MultiFlagsNoDescription = FlagsDescriptionWithValue | FlagsNoDescriptionWithValue,
            [Enum.Description("Multi Flags Description")]
            MultiFlagsDescription = FlagsDescriptionWithValue | OtherValue,
        }

        [Flags]
        private enum TestFlagsNoZero : Int64
        {
            Flag1 = 1,
            Flag2 = 2,
            Flag4 = 4,
        }

        [Flags]
        public enum TriggerEnum : Int64
        {
            Retrieve = 1,
            Insert = 2,
            Update = 4,
            Delete = 8,

            Touch = 16,

            // Patch = 32,
            // Options = 64,

            StartSession = 128,
            EndSession = 256,
            CancelSession = 512,

            CheckIn = 1024,
            Completed = 2048,
            Charge = 4096,

            HardwareSync = 8192,
            ChargingSync = 16384,

            Error = Int64.MaxValue,
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.EnumUtilities))]
        public void ToDescription()
        {
            var val1 = (Int32)TestEnum.TestNoDescription;

            var testNoDescription = TestEnum.TestNoDescription.ToDescription();
            Assert.AreEqual("TestNoDescription", testNoDescription, "TestEnum with no description, and no value, returned unexpected result.");
            var testNoDescriptionWithValue = TestEnum.TestNoDescriptionWithValue.ToDescription();
            Assert.AreEqual("TestNoDescriptionWithValue", testNoDescriptionWithValue, "TestEnum with no description, and a value, returned an unexpected result.");

            var testDescription = TestEnum.TestDescription.ToDescription();
            Assert.AreEqual("Test Description", testDescription, "TestEnum with a description, and no value, returned an unexpected result.");
            var testDescriptionWithValue = TestEnum.TestDescriptionWithValue.ToDescription();
            Assert.AreEqual("Test Description With Value", testDescriptionWithValue, "TestEnum with a description, and a value, returned an unexpected result.");

            var flagsNoDescription = TestFlags.FlagsNoDescription.ToDescription();
            Assert.AreEqual("FlagsNoDescription", flagsNoDescription);
            var flagsNoDescriptionWithValue = TestFlags.FlagsNoDescriptionWithValue.ToDescription();
            Assert.AreEqual("FlagsNoDescriptionWithValue", flagsNoDescriptionWithValue);

            var flagsDescription = TestFlags.FlagsDescription.ToDescription();
            Assert.AreEqual("Flags Description, FlagsNoDescriptionWithValue", flagsDescription);
            var flagsDescriptionWithValue = TestFlags.FlagsDescriptionWithValue.ToDescription();
            Assert.AreEqual("Flags Description With Value", flagsDescriptionWithValue);

            var multiFlagsDescription = TestFlags.MultiFlagsDescription.ToDescription(false);
            Assert.AreEqual("Multi Flags Description", multiFlagsDescription);
            var multiFlagsNoDescription = TestFlags.MultiFlagsNoDescription.ToDescription(false);
            Assert.AreEqual("MultiFlagsNoDescription", multiFlagsNoDescription);

            var otherFlagsDescription = (TestFlags.FlagsNoDescriptionWithValue | TestFlags.OtherValue).ToDescription();
            Assert.AreEqual("OtherValue, FlagsNoDescriptionWithValue", otherFlagsDescription);

            TestEnum badTest = (TestEnum)10;
            var badTestDescription = badTest.ToDescription();
            Assert.AreEqual("10", badTestDescription);

            TestFlags badTestFlags = (TestFlags)6;
            var badTestFlagsDescription = badTestFlags.ToDescription();
            Assert.AreEqual("FlagsNoDescriptionWithValue, 6", badTestFlagsDescription);
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.EnumUtilities))]
        public void ToEnum()
        {
            var emptyFlags = "flagsnodescriptionwithvalue,flagsdescriptionwithvalue".ToEnum<TestFlags>(false);
            Assert.IsNull(emptyFlags, "ToEnum(false) was not case sensitive.");

            var multiFlagsNoDescription = "flagsnodescriptionwithvalue,flagsdescriptionwithvalue".ToEnum<TestFlags>(true);
            Assert.IsNotNull(multiFlagsNoDescription, "ToEnum(true) was case sensitive.");
            Assert.IsTrue(multiFlagsNoDescription.Value.HasFlag(TestFlags.FlagsNoDescriptionWithValue), "ToEnum(true) is missing the first value.");
            Assert.IsTrue(multiFlagsNoDescription.Value.HasFlag(TestFlags.FlagsDescriptionWithValue), "ToEnum(true) is missing the second value.");
            Assert.AreEqual(multiFlagsNoDescription, TestFlags.MultiFlagsNoDescription, "ToEnum(true) didn't return the expected flags.");
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.EnumUtilities))]
        public void ToFlags()
        {
            // var singleFlag = (TestFlags.FlagsDescription).ToFlagsArray<TestFlags>().FromFlagsArray<TestFlags>();
            // 
            // var multiFlag1 = (TestFlags.MultiFlagsDescription).ToFlagsArray<TestFlags>().FromFlagsArray<TestFlags>();
            // 
            // var multiFlag2 = (TestFlags.MultiFlagsNoDescription).ToFlagsArray<TestFlags>().FromFlagsArray<TestFlags>();
            // 
            // var multiFlag3 = (TestFlags.FlagsNoDescriptionWithValue | TestFlags.MultiFlagsDescription).ToFlagsArray<TestFlags>().FromFlagsArray<TestFlags>();

            var multiFlag4 = (TestFlagsNoZero.Flag2 | TestFlagsNoZero.Flag4).ToFlagsArray<TestFlagsNoZero>().FromFlagsArray<TestFlagsNoZero>();
            var multiFlag5 = (TriggerEnum.Insert | TriggerEnum.Update).ToFlagsArray<TriggerEnum>().FromFlagsArray<TriggerEnum>();
        }
    }
}
