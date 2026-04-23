using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Utilities;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void TestGenerateRandomScore()
        {
            int lowerBound = 1, upperBound = 10;
            int randomScore = Helpers.GenerateRandomScore(lowerBound, upperBound);
            Assert.IsTrue(lowerBound <= randomScore && randomScore <= upperBound);
        }

        [TestMethod]
        public void TestGetFormattedNameFromJobRoleUIUX()
        {
            Assert.AreEqual("UI/UX Designer", Helpers.GetFormattedNameFromJobRole(JobRole.UIUXDesigner));
        }

        [TestMethod]
        public void TestGetFormattedNameFromJobRoleAI()
        {
            Assert.AreEqual("AI/ML Engineer", Helpers.GetFormattedNameFromJobRole(JobRole.AIMLEngineer));
        }

        [TestMethod]
        public void TestGetFormattedNameFromJobRoleDataAnalyst()
        {
            Assert.AreEqual("Data Analyst", Helpers.GetFormattedNameFromJobRole(JobRole.DataAnalyst));
        }

        [TestMethod]
        public void TestGetFormattedNameFromJobRoleDevOps()
        {
            Assert.AreEqual("Dev Ops Engineer", Helpers.GetFormattedNameFromJobRole(JobRole.DevOpsEngineer));
        }
    }
}
