using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.models
{
    public class UserLevel
    {
        public int LevelNumber { get; set; }
        public string Title { get; set; }
        public int XpRequired { get; set; }
        public int NextLevelXP { get; set; }
        public int TotalXP { get; set; }

        private UserLevel(int levelNumber, string title, int xpRequired, int nextLevelXP)
        {
            LevelNumber = levelNumber;
            Title = title;
            XpRequired = xpRequired;
            NextLevelXP = nextLevelXP;
        }

        public static UserLevel calculateLevel(int xp)
        {
            if (xp >= 800)
                return new UserLevel(5, "Expert", 800, 0);
            else if (xp >= 500)
                return new UserLevel(4, "Specialist", 500, 800);
            else if (xp >= 250)
                return new UserLevel(3, "Practitioner", 250, 500);
            else if (xp >= 100)
                return new UserLevel(2, "Apprentice", 100, 250);
            else
                return new UserLevel(1, "Newcomer", 0, 100);
        }

        public float getProgressPercent(int totalXP)
        {
            if (NextLevelXP == 0)
                return 100f;

            return (float)(totalXP - XpRequired) / (NextLevelXP - XpRequired) * 100f;
        }

        public int getXPToNextLevel()
        {
            if (NextLevelXP == 0)
                return 0;

            return NextLevelXP - XpRequired;
        }
    }
}
