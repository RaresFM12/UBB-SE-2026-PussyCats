using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace PussyCatsApp.models
{
    public class SkillTest
    {
        public int SkillTestId { get; }
        public string _name = string.Empty;
        public int _score;
        public int UserId { get; }
        public DateOnly AchievedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException("Test name cannot be null");
        }
        public int Score
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

        public SkillTest(int skillId, int userId, string name)
        {
            SkillTestId = skillId;
            UserId = userId;
            Name = name;
        }

        public SkillTest(int skillId, int userId, string name, int score)
        {
            SkillTestId = skillId;
            UserId = userId;
            Name = name;
            Score = score;
        }

        public SkillTest (int skillId, int userId, string name, int score, DateOnly achievedDate)
        {
            SkillTestId = skillId;
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

        public int getXP()
        {

            if (Score >= 90)
                return 100;

            if (Score >= 70)
                return 60;

            if (Score >= 50)
                return 30;

            return 10;
        }

    }
}
