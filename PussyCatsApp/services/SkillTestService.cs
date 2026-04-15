using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;
using PussyCatsApp.Models;
using Microsoft.UI.Xaml.Controls;

namespace PussyCatsApp.Services
{
    public class SkillTestService
    {
        private SkillTestRepository skillTestRepository;

        public SkillTestService(SkillTestRepository skillTestRepository)
        {
            this.skillTestRepository = skillTestRepository;
        }

        public List<SkillTest> GetTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool CanRetakeTest(int skillId)
        {
            SkillTest skill = skillTestRepository.Load(skillId);

            if (skill == null)
            {
                throw new Exception($"No test found for ID {skillId}");
            }

            return skill.IsRetakeEligible();
        }

        public Badge SubmitRetake(int skillId, int newScore)
        {
            if (!CanRetakeTest(skillId))
            {
                throw new Exception("Test is not yet eligible for a retake. Action blocked at service layer.");
            }

            skillTestRepository.UpdateSkillTestScore(skillId, newScore);
            skillTestRepository.UpdateAchievedDate(skillId, DateOnly.FromDateTime(DateTime.Now));

            return Badge.AssignTier(newScore);
        }
    }
}
