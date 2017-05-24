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
            TestNoDescription,
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

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.EnumUtilities))]
        public void ToDescription()
        {
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
            var singleFlag = (TestFlags.FlagsDescription).ToFlagsArray<TestFlags>();

            var multiFlag1 = (TestFlags.MultiFlagsDescription).ToFlagsArray<TestFlags>();

            var multiFlag2 = (TestFlags.MultiFlagsNoDescription).ToFlagsArray<TestFlags>();

            var multiFlag3 = (TestFlags.FlagsNoDescriptionWithValue | TestFlags.MultiFlagsDescription).ToFlagsArray<TestFlags>();
        }
    }
}
