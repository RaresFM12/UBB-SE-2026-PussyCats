using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
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
        public UserLevel()
        {
            LevelNumber = 1;
            Title = "Newcomer";
            XpRequired = 0;
            NextLevelXP = 100;
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

        public int getProgressPercent(int totalXP)
        {
            if (NextLevelXP == 0)
                return 100;

            double progress = (double)(totalXP - XpRequired) / (NextLevelXP - XpRequired) * 100;
            return (int)progress;
        }

        public int getXPToNextLevel(int totalXP)
        {
            if (NextLevelXP == 0)
                return 0;

            return NextLevelXP - totalXP;
        }
    }
}
