using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class ParserUtilitiesTests
    {
        [TestMethod]
        public void Parse()
        {
            // Test String
            // Test Nullable
            // Test DateTime
            // Test Generic Parser "Parse"
            // Test Defaults
            // Test Exception Bubbling
            throw new NotImplementedException();
        }

        [TestMethod]
        public void To()
        {
            // Test Cast version of Parse
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToBoolean()
        {
            // ToBoolean(Boolean @default)
            // Test bool values
            // Test non-bool values return @default

            // ToBoolean()
            // Test bool values
            // Test non-bool values
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToInt16()
        {
            // ToInt16
            // Test enum values
            // Test string values
            // Test non-int16 values
            // Test int32 values

            // ToInt16(Int16 @default)
            // Test enum values
            // Test string values
            // Test non-int16 values return @default
            // Test int32 values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToInt32()
        {
            // ToInt32
            // Test enum values
            // Test string values
            // Test non-int32 values
            // Test int64 values

            // ToInt32(Int32 @default)
            // Test enum values
            // Test string values
            // Test non-int32 values return @default
            // Test int64 values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToInt64()
        {
            // ToInt64
            // Test enum values
            // Test string values
            // Test non-int64 values
            // Test massive values

            // ToInt64(Int64 @default)
            // Test enum values
            // Test string values
            // Test non-int64 values return @default
            // Test massive values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToSingle()
        {
            // ToSingle
            // Test string values
            // Test non-single values
            // Test double values

            // ToSingle(Single @default)
            // Test string values
            // Test non-single values return @default
            // Test double values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToDouble()
        {
            // ToDouble
            // Test string values
            // Test non-double values
            // Test massive values

            // ToSingle(Single @default)
            // Test string values
            // Test non-double values return @default
            // Test massive values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToDecimal()
        {
            // ToDecimal
            // Test string values
            // Test non-decimal values
            // Test massive values

            // ToDecimal(Decimal @default)
            // Test string values
            // Test non-decimal values return @default
            // Test massive values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToDateTime()
        {
            // ToDateTime
            // Test string values
            // Test timezone support
            // Test non-datetime values
            // Test invalid date values

            // ToDateTime(DateTime @default)
            // Test string values
            // Test timezone support
            // Test non-datetime values return @default
            // Test invalid date values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToDateTimeOffset()
        {
            // ToDateTimeOffset
            // Test string values
            // Test timezone support
            // Test non-datetimeoffset values
            // Test invalid date values

            // ToDateTimeOffset(DateTimeOffset @default)
            // Test string values
            // Test timezone support
            // Test non-datetimeoffset values return @default
            // Test invalid date values return @default
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ToTimeSpan()
        {
            // ToTimeSpan
            // Test string values
            // Test timespan support
            // Test non-timespan values return @default

            // ToTimeSpan(TimeSpan @default)
            // Test string values
            // Test timezone support
            // Test non-timespan values return @default
            throw new NotImplementedException();
        }
    }
}