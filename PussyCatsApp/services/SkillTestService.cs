using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml.Controls;

using PussyCatsApp.repositories;
using PussyCatsApp.Models;
using PussyCatsApp.models;



namespace PussyCatsApp.services
{
    public class SkillTestService : ISkillTestService
    {
        private ISkillTestRepository skillTestRepository;

        public SkillTestService(ISkillTestRepository skillTestRepository)
        {
            this.skillTestRepository = skillTestRepository;
        }

        public List<SkillTest> GetTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool CanRetakeTest(int skillId)
        {
            SkillTest skill = skillTestRepository.load(skillId);

            if (skill == null)
                throw new Exception($"No test found for ID {skillId}");

            return skill.IsRetakeEligible();
        }

        public Badge SubmitRetake(int skillId, int newScore)
        {
            if (!CanRetakeTest(skillId))
                throw new Exception("Test is not yet eligible for a retake. Action blocked at service layer.");


            skillTestRepository.UpdateSkillTestScore(skillId, newScore);
            skillTestRepository.UpdateAchievedDate(skillId, DateOnly.FromDateTime(DateTime.Now));

            return Badge.assignTier(newScore);
        }
    }
}
