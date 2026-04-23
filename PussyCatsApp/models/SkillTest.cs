using System;

namespace PussyCatsApp.Models
{
    /// <summary>
    /// Represents a skill test taken by a user, including the score,
    /// date of completion, and eligibility rules for retaking the test.
    /// </summary>
    public class SkillTest
    {
        /// <summary>
        /// The minimum allowed score value.
        /// </summary>
        private const int MinimumScore = 0;

        /// <summary>
        /// The maximum allowed score value.
        /// </summary>
        private const int MaximumScore = 100;

        /// <summary>
        /// Number of months that must pass before a test can be retaken.
        /// </summary>
        private const int RetakeEligibilityMonths = 3;

        /// <summary>
        /// Score threshold at or above which the gold tier is awarded.
        /// </summary>
        private const int GoldScoreThreshold = 90;

        /// <summary>
        /// Score threshold at or above which the silver tier is awarded.
        /// </summary>
        private const int SilverScoreThreshold = 70;

        /// <summary>
        /// Score threshold at or above which the bronze tier is awarded.
        /// </summary>
        private const int BronzeScoreThreshold = 50;

        /// <summary>
        /// Experience points awarded for a gold-tier score.
        /// </summary>
        private const int GoldExperiencePoints = 100;

        /// <summary>
        /// Experience points awarded for a silver-tier score.
        /// </summary>
        private const int SilverExperiencePoints = 60;

        /// <summary>
        /// Experience points awarded for a bronze-tier score.
        /// </summary>
        private const int BronzeExperiencePoints = 30;

        /// <summary>
        /// Experience points awarded for a participant-tier score.
        /// </summary>
        private const int ParticipantExperiencePoints = 10;
        private string name = string.Empty;
        private int score;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with no score (defaults to zero).
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        public SkillTest(int skillTestId, int userId, string testName)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with a specified score.
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        /// <param name="testScore">The score achieved (0–100).</param>
        public SkillTest(int skillTestId, int userId, string testName, int testScore)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
            Score = testScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with a specified score and achieved date.
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        /// <param name="testScore">The score achieved (0–100).</param>
        /// <param name="achievedDate">The date on which the test was completed.</param>
        public SkillTest(int skillTestId, int userId, string testName, int testScore, DateOnly achievedDate)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
            Score = testScore;
            AchievedDate = achievedDate;
        }

        /// <summary>
        /// Gets the unique identifier for this skill test record.
        /// </summary>
        public int SkillTestId { get; }

        /// <summary>
        /// Gets the identifier of the user who took this test.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets or sets the date on which this test was completed.
        /// </summary>
        public DateOnly AchievedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets or sets the name of the skill test.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        public string Name
        {
            get => name;
            set => name = value ?? throw new ArgumentNullException(nameof(value), "Test name cannot be null.");
        }

        /// <summary>
        /// Gets or sets the score achieved on the test.
        /// Must be between <see cref="MinimumScore"/> and <see cref="MaximumScore"/> inclusive.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the value is less than <see cref="MinimumScore"/>
        /// or greater than <see cref="MaximumScore"/>.
        /// </exception>
        public int Score
        {
            get => score;
            set
            {
                if (value < MinimumScore || value > MaximumScore)
                {
                    throw new ArgumentException(
                        $"Score must be between {MinimumScore} and {MaximumScore} inclusive.");
                }

                score = value;
            }
        }

        /// <summary>
        /// Gets the achieved date formatted as dd.MM.yyyy for display purposes.
        /// </summary>
        public string AchievedDateFormatted => AchievedDate.ToString("dd.MM.yyyy");

        /// <summary>
        /// Determines whether this test is eligible for a retake.
        /// A test can be retaken if at least <see cref="RetakeEligibilityMonths"/>
        /// months have passed since the achieved date.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the test can be retaken; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRetakeEligible()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly eligibilityDate = currentDate.AddMonths(-RetakeEligibilityMonths);

            return eligibilityDate >= AchievedDate;
        }

        public int GetExperiencePoints()
        {
            if (Score >= GoldScoreThreshold)
            {
                return GoldExperiencePoints;
            }

            if (Score >= SilverScoreThreshold)
            {
                return SilverExperiencePoints;
            }

            if (Score >= BronzeScoreThreshold)
            {
                return BronzeExperiencePoints;
            }

            return ParticipantExperiencePoints;
        }
    }
}
