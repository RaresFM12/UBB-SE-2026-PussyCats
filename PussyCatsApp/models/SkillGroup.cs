using System.Collections.Generic;

namespace PussyCatsApp.Models
{
    public class SkillGroup
    {
        public string GroupName { get; set; }
        public List<string> Skills { get; set; }
        public int Weight { get; set; }
        public JobRole JobRole { get; set; }
    }
}
