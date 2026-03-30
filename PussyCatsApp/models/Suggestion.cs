using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.models
{
    public class Suggestion
    {
        private string skillName;
        private string groupName;
        private double gainScore;

        public string SkillName { get { return skillName; } set { skillName = value; } }
        public string GroupName { get { return groupName; } set { groupName = value; } }
        public double GainScore { get { return gainScore; } set { gainScore = value; } }
    }
}
