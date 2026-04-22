using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class UserLevel
    {
        public enum UserTitle
        {
            Newcomer,
            Apprentice,
            Practitioner,
            Specialist,
            Expert
        }

        private const int LEVEL_1_XP = 0;
        private const int LEVEL_2_XP = 100;
        private const int LEVEL_3_XP = 250;
        private const int LEVEL_4_XP = 500;
        private const int LEVEL_5_XP = 800;

        public int LevelNumber { get; set; }
        public UserTitle Title { get; set; }
        public int XpRequired { get; set; }
        public int NextLevelXp { get; set; }

        private UserLevel(int levelNumber, UserTitle title, int xpRequired, int nextLevelXp)
        {
            LevelNumber = levelNumber;
            Title = title;
            XpRequired = xpRequired;
            NextLevelXp = nextLevelXp;
        }
        public UserLevel()
        {
            LevelNumber = 1;
            Title = UserTitle.Newcomer;
            XpRequired = LEVEL_1_XP;
            NextLevelXp = LEVEL_2_XP;
        }

        public static UserLevel CalculateLevel(int xp)
        {
            switch (xp)
            {
                case >= LEVEL_5_XP:
                    return new UserLevel(5, UserTitle.Expert, LEVEL_5_XP, LEVEL_1_XP);
                case >= LEVEL_4_XP:
                    return new UserLevel(4, UserTitle.Specialist, LEVEL_4_XP, LEVEL_5_XP);
                case >= LEVEL_3_XP:
                    return new UserLevel(3, UserTitle.Practitioner, LEVEL_3_XP, LEVEL_4_XP);
                case >= LEVEL_2_XP:
                    return new UserLevel(2, UserTitle.Apprentice, LEVEL_2_XP, LEVEL_3_XP);
                default:
                    return new UserLevel(1, UserTitle.Newcomer, LEVEL_1_XP, LEVEL_2_XP);
            }
        }

        public int GetLevelProgressPercent(int totalXp)
        {
            if (NextLevelXp == 0)
            {
                return 100;
            }

            double completedPercentageIntoCurrentLevel = this.GetLevelProgressFraction(totalXp);
            return (int)completedPercentageIntoCurrentLevel;
        }

        private double GetLevelProgressFraction(int totalXp)
        {
            double pointsIntoLevel = totalXp - XpRequired;
            double totalPointsForLevel = NextLevelXp - XpRequired;
            double completedPercentageIntoCurrentLevel = pointsIntoLevel / totalPointsForLevel * 100;
            return completedPercentageIntoCurrentLevel;
        }

        public int GetXpToNextLevel(int totalXp)
        {
            if (NextLevelXp == LEVEL_1_XP)
            {
                return 0;
            }
            return NextLevelXp - totalXp;
        }
    }
}
