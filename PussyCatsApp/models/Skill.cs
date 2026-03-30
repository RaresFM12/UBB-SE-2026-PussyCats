using System;

namespace PussyCatsApp.models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
        public int UserId { get; set; }
        public DateTime AchievedDate { get; set; } = DateTime.Now;
    }
}
