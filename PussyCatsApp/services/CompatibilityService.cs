using PussyCatsApp.models;
using PussyCatsApp.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.services
{
    public class CompatibilityService
    {
        private UserSkillRepository userSkillRepository;
        private SkillGroupRepository skillGroupRepository;

        public CompatibilityService(UserSkillRepository userSkillRepository, SkillGroupRepository skillGroupRepository)
        {
            this.userSkillRepository = userSkillRepository;
            this.skillGroupRepository = skillGroupRepository;
        }

        private double ComputeGroupScore(SkillGroup group, List<UserSkill> userSkills)
        {
            double maxCi = 0;
            foreach (string skill in group.Skills)
            {
                double ci = 0;
                foreach (UserSkill userSkill in userSkills)
                {
                    if (userSkill.SkillName == skill)
                    {
                        if (userSkill.IsVerified)
                            ci = userSkill.Score / 100.0;
                        else
                            ci = 0.5;
                        break;
                    }
                }
                if (ci > maxCi)
                    maxCi = ci;
            }
            return maxCi;
        }

        private double ComputeMatchScore(List<SkillGroup> groups, List<double> groupScores)
        {
            int totalWeight = 0;
            foreach (SkillGroup group in groups)
                totalWeight += group.Weight;

            if (totalWeight == 0)
                return -1;

            double weightedSum = 0;
            for (int i = 0; i < groups.Count; i++)
                weightedSum += groups[i].Weight * groupScores[i];

            return weightedSum * 100 / totalWeight;
        }
    }
}
