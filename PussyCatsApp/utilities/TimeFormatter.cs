using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Utilities
{
    internal class TimeFormatter
    {
        public static string CalculateFreshnessLabel(DateTime targetDate)
        {
            TimeSpan difference = DateTime.Now.Date - targetDate.Date;
            int totalDays = (int)difference.TotalDays;

            if (totalDays == 0)
            {
                return "Profile last updated: Today";
            }
            else if (totalDays == 1)
            {
                return "Profile last updated: Yesterday";
            }
            else
            {
                return $"Profile last updated: {totalDays} days ago";
            }
        }
    }
}
