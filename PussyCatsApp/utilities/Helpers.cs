using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.utilities
{
    public class Helpers
    {
        /// <summary>
        /// Generate a random score between the specified lower and upper bounds (inclusive).
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static int GenerateRandomScore(int lowerBound, int upperBound)
        {
            Random random = new Random();
            return random.Next(lowerBound, upperBound + 1);
        }
    }
}
