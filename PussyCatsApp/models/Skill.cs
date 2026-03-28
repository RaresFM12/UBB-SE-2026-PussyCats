using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PussyCatsApp.models
{
    public class Skill
    {
        public int SkillId { get; }
        public string _name = string.Empty;
        public double _score;
        public int UserId { get; }
        public DateOnly AchievedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException("Test name cannot be null");
        }
        public double Score
        {
            get => _score;
            set 
            {  
                if (value < 0 || value > 100)
                    throw new ArgumentException("Score cannot be greater that 100 and less that 0");
                _score = value;
            }
           
        }
        public string AchievedDateFormatted => AchievedDate.ToString("dd.MM.yyyy");

        public Skill(int skillId, int userId, string name)
        {
            SkillId = skillId;
            UserId = userId;
            Name = name;
        }

        public Skill(int skillId, int userId, string name, double score)
        {
            SkillId = skillId;
            UserId = userId;
            Name = name;
            Score = score;
        }

        public Skill (int skillId, int userId, string name, float score, DateOnly achievedDate)
        {
            SkillId = skillId;
            UserId = userId;
            Name = name;
            Score = score;
            AchievedDate = achievedDate;
        }

        public bool isRetakeEligible()
        {
            var dateNow = DateOnly.FromDateTime(DateTime.Now);
            if (dateNow.AddMonths(-3) <= AchievedDate)
                return true;
            return false;     
        }

    }
}
