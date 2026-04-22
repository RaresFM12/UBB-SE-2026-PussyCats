using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class SkillGroup
    {
        private string groupName;
        private List<string> skills;
        private int weight;
        private JobRole jobRole;

        public string GroupName
        {
            get { return groupName; } set { groupName = value; }
        }
        public List<string> Skills
        {
            get { return skills; } set { skills = value; }
        }
        public int Weight
        {
            get { return weight; } set { weight = value; }
        }
        public JobRole JobRole
        {
            get { return jobRole; } set { jobRole = value; }
        }
    }
}
