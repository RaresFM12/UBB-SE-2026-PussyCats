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
    internal class SkillService
    {
        private SkillRepository skillRepository;

        public SkillService(SkillRepository skillRepository)
        {
            this.skillRepository = skillRepository;
        }

        public List<Skill> getTestsForUser(string userId)
        {
            return skillRepository.GetSkillsByUserId(int.Parse(userId));
        }

        public bool canRetakeTest(string skillId)
        {
            Skill skill = skillRepository.load(int.Parse(skillId));

            if (skill == null)
                throw new Exception($"No test found for ID {skillId}");

            return skill.isRetakeEligible();
        }

        public Badge submitRetake(string testId, float newScore)
        {
            if (!canRetakeTest(testId))
                throw new Exception("Test is not yet eligible for a retake. Action blocked at service layer.");

            int id = int.Parse(testId);

            skillRepository.UpdateSkillScore(id, newScore);
            skillRepository.UpdateAchievedDate(id, DateOnly.FromDateTime(DateTime.Now));

            return Badge.assignTier(newScore);
        }
    }
}
