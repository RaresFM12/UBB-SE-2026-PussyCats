using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.repositories;
using PussyCatsApp.models;

namespace PussyCatsApp.services
{
    internal class SkillTestService
    {
        private SkillTestRepository skillTestRepository;

        public SkillTestService(SkillTestRepository skillTestRepository)
        {
            this.skillTestRepository = skillTestRepository;
        }

        public List<SkillTest> getTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool canRetakeTest(int skillId)
        {
            SkillTest skill = skillTestRepository.load(skillId);

            if (skill == null)
                throw new Exception($"No test found for ID {skillId}");

            return skill.isRetakeEligible();
        }

        public Badge submitRetake(int skillId, int newScore)
        {
            if (!canRetakeTest(skillId))
                throw new Exception("Test is not yet eligible for a retake. Action blocked at service layer.");

            skillTestRepository.UpdateSkillTestScore(skillId, newScore);
            skillTestRepository.UpdateAchievedDate(skillId, DateOnly.FromDateTime(DateTime.Now));

            return Badge.assignTier(newScore);
        }
    }
}
