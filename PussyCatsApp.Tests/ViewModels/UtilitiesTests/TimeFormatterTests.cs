using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class TimeFormatterTests
    {
        [TestMethod]
        public void TestCalculateLabelCorrectly()
        {
            DateTime targetDateToday = DateTime.Now.Date;
            DateTime targetDateYesterday = DateTime.Now.Date.AddDays(-1);
            int numberOfDays = 5;
            DateTime targetDateMultipleDaysAgo = DateTime.Now.Date.AddDays(-numberOfDays);

            Assert.AreEqual("Profile last updated: Today", TimeFormatter.CalculateFreshnessLabel(targetDateToday));
            Assert.AreEqual("Profile last updated: Yesterday", TimeFormatter.CalculateFreshnessLabel(targetDateYesterday));
            Assert.AreEqual($"Profile last updated: {numberOfDays} days ago", TimeFormatter.CalculateFreshnessLabel(targetDateMultipleDaysAgo));
        }
    }
}
