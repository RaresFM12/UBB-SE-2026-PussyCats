using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class UserSkill
    {
        private string skillName;
        private bool isVerified;
        private int score;

        public string SkillName
        {
            get { return skillName; } set { skillName = value; }
        }
        public bool IsVerified
        {
            get { return isVerified; } set { isVerified = value; }
        }
        public int Score
        {
            get { return score; } set { score = value; }
        }
    }
}
