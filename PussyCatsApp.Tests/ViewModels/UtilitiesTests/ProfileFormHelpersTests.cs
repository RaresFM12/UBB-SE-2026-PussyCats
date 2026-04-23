using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class ProfileFormHelpersTests
    {
        [TestMethod]
        public void TestUniversityMatchesAllWords_ReturnsTrue_WhenAllWordsMatch()
        {
            string university = "Massachusetts Institute of Technology";
            string[] words = new string[] { "massachusetts", "institute", "technology" };
            bool result = ProfileFormHelpers.UniversityMatchesAllWords(university, words);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestUniversityMatchesAllWords_ReturnsFalse_WhenAtLeastOneWordDoesNotMatch()
        {
            string university = "Massachusetts Institute of Technology";
            string[] words = new string[] { "massachusetts", "institute", "harvard" };
            bool result = ProfileFormHelpers.UniversityMatchesAllWords(university, words);
            Assert.IsFalse(result);
        }
    }
}
