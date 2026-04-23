using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class PhoneNumberHelperTests
    {
        [TestMethod]
        public void TestExtractPhonePrefixAndNumber_ReturnsBothEmptyIfNullOrEmpty()
        {
            Assert.AreEqual(("", ""), PhoneNumberHelper.ExtractPhonePrefixAndNumber(null));
            Assert.AreEqual(("", ""), PhoneNumberHelper.ExtractPhonePrefixAndNumber(""));
        }

        [TestMethod]
        public void TestExtractPhonePrefixAndNumber_ReturnsPrefixAndNumberWhenPrefixMatches()
        {
            var result = PhoneNumberHelper.ExtractPhonePrefixAndNumber("+40123456789");
            Assert.AreEqual("+40", result.prefix);
            Assert.AreEqual("123456789", result.number);
        }

        [TestMethod]
        public void TestExtractPhonePrefixAndNumber_ReturnsEmptyPrefixWhenNoMatch()
        {
            var result = PhoneNumberHelper.ExtractPhonePrefixAndNumber("123456789");
            Assert.AreEqual("", result.prefix);
            Assert.AreEqual("123456789", result.number);
        }
    }
}
