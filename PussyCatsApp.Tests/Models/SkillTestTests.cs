// ============================================================================
// SkillTestTests.cs — Osherove-style unit tests for SkillTest
// ============================================================================
// Naming convention: MethodUnderTest_Scenario_ExpectedBehavior
// Each test has Arrange / Act / Assert sections.
// No mocking needed here because SkillTest is a pure model with no dependencies.
// ============================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Models;
using System;

namespace PussyCatsApp.Tests.Models
{
    [TestClass]
    public class SkillTestTests
    {
        // Constructor Tests

        [TestMethod]
        public void Constructor_WithNameOnly_SetsScoreToZero()
        {
            // Arrange & Act
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Assert
            Assert.AreEqual(0, skillTest.Score);
            Assert.AreEqual("React", skillTest.Name);
            Assert.AreEqual(1, skillTest.SkillTestId);
            Assert.AreEqual(10, skillTest.UserId);
        }

        [TestMethod]
        public void Constructor_WithScore_SetsAllProperties()
        {
            // Arrange & Act
            var skillTest = new SkillTest(skillTestId: 2, userId: 10, testName: "Docker", testScore: 85);

            // Assert
            Assert.AreEqual(85, skillTest.Score);
            Assert.AreEqual("Docker", skillTest.Name);
        }

        [TestMethod]
        public void Constructor_WithScoreAndDate_SetsAchievedDate()
        {
            // Arrange
            var date = new DateOnly(2025, 6, 15);

            // Act
            var skillTest = new SkillTest(skillTestId: 3, userId: 10, testName: "SQL", testScore: 72, achievedDate: date);

            // Assert
            Assert.AreEqual(date, skillTest.AchievedDate);
            Assert.AreEqual(72, skillTest.Score);
        }

        // Name Property Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Name_SetToNull_ThrowsArgumentNullException()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Name = null;

            // Assert — handled by ExpectedException
        }

        [TestMethod]
        public void Name_SetToValidString_UpdatesName()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Name = "Angular";

            // Assert
            Assert.AreEqual("Angular", skillTest.Name);
        }

        // Score Property Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Score_SetValueAboveMaximum_ThrowsArgumentException()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Score = 101;

            // Assert — handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Score_SetNegativeValue_ThrowsArgumentException()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Score = -1;

            // Assert — handled by ExpectedException
        }

        [TestMethod]
        public void Score_SetToZero_AcceptsValue()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Score = 0;

            // Assert
            Assert.AreEqual(0, skillTest.Score);
        }

        [TestMethod]
        public void Score_SetToMaximum_AcceptsValue()
        {
            // Arrange
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            // Act
            skillTest.Score = 100;

            // Assert
            Assert.AreEqual(100, skillTest.Score);
        }

        [TestMethod]
        public void Score_SetToMidRangeValue_AcceptsValue()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React");

            skillTest.Score = 55;

            Assert.AreEqual(55, skillTest.Score);
        }

        // AchievedDateFormatted Tests

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

        // IsRetakeEligible Tests

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
        public void IsRetakeEligible_AchievedDateLessThanThreeMonthsAgo_ReturnsFalse()
        {
            DateOnly oneMonthAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: oneMonthAgo);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsFalse(isEligible);
        }

        [TestMethod]
        public void IsRetakeEligible_AchievedDateIsToday_ReturnsFalse()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: today);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsFalse(isEligible);
        }

        [TestMethod]
        public void IsRetakeEligible_AchievedDateOneYearAgo_ReturnsTrue()
        {
            DateOnly oneYearAgo = DateOnly.FromDateTime(DateTime.Now.AddYears(-1));
            var skillTest = new SkillTest(
                skillTestId: 1,
                userId: 10,
                testName: "React",
                testScore: 60,
                achievedDate: oneYearAgo);

            bool isEligible = skillTest.IsRetakeEligible();

            Assert.IsTrue(isEligible);
        }

        // GetExperiencePoints Tests

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
        public void GetExperiencePoints_ScoreIs100_ReturnsGoldExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 100);

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

        [TestMethod]
        public void GetExperiencePoints_ScoreIs1_ReturnsParticipantExperiencePoints()
        {
            var skillTest = new SkillTest(skillTestId: 1, userId: 10, testName: "React", testScore: 1);

            int experiencePoints = skillTest.GetExperiencePoints();

            Assert.AreEqual(10, experiencePoints);
        }
    }
}
