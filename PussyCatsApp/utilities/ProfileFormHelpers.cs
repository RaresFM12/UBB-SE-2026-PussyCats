using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Utilities
{
    public class ProfileFormHelpers
    {
        public static bool UniversityMatchesAllWords(string university, string[] words)
        {
            foreach (string word in words)
            {
                if (!university.ToLower().Contains(word))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
