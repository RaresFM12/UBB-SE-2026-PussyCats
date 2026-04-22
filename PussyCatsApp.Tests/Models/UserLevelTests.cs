using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.Models
{
    [TestClass]
    public class UserLevelTests
    {
        [TestMethod]
        [DataRow(900, 5, UserLevel.UserTitle.Expert, 800, 0)]
        [DataRow(800, 5, UserLevel.UserTitle.Expert, 800, 0)]
        [DataRow(799, 4, UserLevel.UserTitle.Specialist, 500, 800)]
        [DataRow(500, 4, UserLevel.UserTitle.Specialist, 500, 800)]
        [DataRow(499, 3, UserLevel.UserTitle.Practitioner, 250, 500)]
        [DataRow(250, 3, UserLevel.UserTitle.Practitioner, 250, 500)]
        [DataRow(249, 2, UserLevel.UserTitle.Apprentice, 100, 250)]
        [DataRow(100, 2, UserLevel.UserTitle.Apprentice, 100, 250)]
        [DataRow(99, 1, UserLevel.UserTitle.Newcomer, 0, 100)]
        [DataRow(0, 1, UserLevel.UserTitle.Newcomer, 0, 100)]
        [DataRow(-50, 1, UserLevel.UserTitle.Newcomer, 0, 100)]
        public void CalculateLevel_GivenXp_ReturnsCorrectLevel(
            int xp,
            int expectedLevel,
            UserLevel.UserTitle expectedTitle,
            int expectedMinXp,
            int expectedMaxXp)
        {
            var level = UserLevel.CalculateLevel(xp);

            Assert.AreEqual(expectedLevel, level.LevelNumber, $"Level failed for xp: {xp}");
            Assert.AreEqual(expectedTitle, level.Title, $"Title failed for xp: {xp}");
            Assert.AreEqual(expectedMinXp, level.XpRequired, $"XpRequired failed for xp: {xp}");
            Assert.AreEqual(expectedMaxXp, level.NextLevelXp, $"NextLevelXp failed for xp: {xp}");
        }
    }
}
