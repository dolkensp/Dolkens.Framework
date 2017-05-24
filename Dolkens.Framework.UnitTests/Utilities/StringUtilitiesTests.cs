using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dolkens.Framework.UnitTests.Utilities
{
    [TestClass]
    public class StringUtilitiesTests
    {
        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.StringUtilities))]
        public void StripTags()
        {
            var cleanStringTest = "This is a clean string".StripTags();
            Assert.AreEqual("This is a clean string", cleanStringTest, "Clean string test failed.");

            var htmlStringTest = "<p>This is a HTML string<p>".StripTags();
            Assert.AreEqual("This is a HTML string", htmlStringTest, "HTML string test failed.");

            var entityStringTest = "This is an entity&nbsp;string<p>".StripTags();
            Assert.AreEqual("This is an entity string", entityStringTest, "HTML entity string test failed.");
        }

        [TestMethod]
        [TestCategory(nameof(Dolkens.Framework.Utilities.StringUtilities))]
        public void TrimTo()
        {
            var longSentenceTest = "This is a test of the trim functionality".TrimTo(15);
            Assert.AreEqual("This is a test...", longSentenceTest, "Long string was not trimmed correctly.");

            var shortSentenceTest = "Short Sentence".TrimTo(25);
            Assert.AreEqual("Short Sentence", shortSentenceTest, "Short string was not trimmed correctly.");

            var longWordTest1 = "FirstTestOfReallyLongWords".TrimTo(5);
            Assert.AreEqual("FirstTestOfReallyLongWords", longWordTest1, "Single leading long word was not trimmed correctly.");

            var longWordTest2 = "SecondTest Of Really Long Words".TrimTo(5);
            Assert.AreEqual("SecondTest...", longWordTest2, "Leading long word was not trimmed correctly.");
        }
    }
}
