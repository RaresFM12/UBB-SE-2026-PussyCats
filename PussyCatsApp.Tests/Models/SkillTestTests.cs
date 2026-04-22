using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Models;
using System;

namespace PussyCatsApp.Tests.Models
{
    [TestClass]
    public class SkillTestTests
    {
        [TestMethod]
        public void AchievedDateFormatted_SpecificDate_ReturnsCorrectFormat()
        {
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 80,
                achievedDate: new DateOnly(2025, 3, 15));

            string formatted = skillTest.AchievedDateFormatted;

            Assert.AreEqual("15.03.2025", formatted);
        }

        [TestMethod]
        public void IsRetakeEligible_AchievedDateMoreThanThreeMonthsAgo_ReturnsTrue()
        {
            DateOnly fourMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: fourMonthsAgo);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsTrue(isEligible);
        }

        [TestMethod]
        public void IsRetakeEligible_AchievedDateLessThanThreeMonthsAgo_ReturnsFalse()
        {
            DateOnly oneMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: oneMonthsAgo);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsFalse(isEligible);
        }

        [TestMethod]
        public void IsRetakeEligible_AchievedDateExactlyThreeMonthsAgo_ReturnsTrue()
        {
            DateOnly exactlyThreeMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-3));
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: exactlyThreeMonthsAgo);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsTrue(isEligible);
        }


        [TestMethod]
        public void GetExperiencePoints_ScoreIs95_ReturnsGoldExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 95);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(100, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIsExactly90_ReturnsGoldExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 90);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(100, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIs89_ReturnsSilverExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 89);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(60, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIsExactly70_ReturnsSilverExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 70);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(60, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIs69_ReturnsBronzeExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 69);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(30, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIsExactly50_ReturnsBronzeExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 50);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(30, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIs49_ReturnsParticipantExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 49);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(10, experiencePoints);
        }

        [TestMethod]
        public void GetExperiencePoints_ScoreIsZero_ReturnsParticipantExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 0);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(10, experiencePoints);
        }

    }
}
