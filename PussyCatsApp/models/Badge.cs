using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class Badge
    {
        public BadgeTier Tier { get; private set; }
        public string IconPath { get; private set; }
        public int XpValue { get; private set; }

        private Badge(BadgeTier tier, string iconPath, int xpValue)
        {
            Tier = tier;
            IconPath = iconPath;
            XpValue = xpValue;
        }

        public static Badge AssignTier(float score)
        {
            if (score >= 90)
            {
                return new Badge(BadgeTier.GOLD, "ms-appx:///Assets/badges/gold.svg", 100);
            }

            if (score >= 70)
            {
                return new Badge(BadgeTier.SILVER, "ms-appx:///Assets/badges/silver.svg", 60);
            }

            if (score >= 50)
            {
                return new Badge(BadgeTier.BRONZE, "ms-appx:///Assets/badges/bronze.svg", 30);
            }

            return new Badge(BadgeTier.PARTICIPANT, "ms-appx:///Assets/badges/participant.svg", 10);
        }
    }
}
