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

        private double ComputeGain(SkillGroup group, double gj, int totalWeight)
        {
            return 100.0 * group.Weight * (0.8 - gj) / totalWeight;
        }

        private List<Suggestion> IdentifyGaps(List<SkillGroup> groups, List<UserSkill> userSkills, int totalWeight)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            foreach (SkillGroup group in groups)
            {
                double gj = ComputeGroupScore(group, userSkills);
                if (gj > 0.5)
                    continue;

                Suggestion bestSuggestionForGroup = null;

                foreach (string skill in group.Skills)
                {
                    bool userHasVerified = false;
                    bool userHasUnverified = false;

                    foreach (UserSkill userSkill in userSkills)
                    {
                        if (userSkill.SkillName == skill)
                        {
                            if (userSkill.IsVerified)
                                userHasVerified = true;
                            else
                                userHasUnverified = true;
                        }
                    }

                    if (userHasVerified)
                        continue;

                    double gain = ComputeGain(group, gj, totalWeight);

                    Suggestion candidate = new Suggestion
                    {
                        SkillName = skill,
                        GroupName = group.GroupName,
                        GainScore = gain
                    };

                    if (bestSuggestionForGroup == null)
                    {
                        bestSuggestionForGroup = candidate;
                    }
                    else if (userHasUnverified && bestSuggestionForGroup.SkillName != null)
                    {
                        continue;
                    }
                }

                if (bestSuggestionForGroup != null)
                    suggestions.Add(bestSuggestionForGroup);
            }

            suggestions.Sort((a, b) => b.GainScore.CompareTo(a.GainScore));

            if (suggestions.Count > 3)
                suggestions = suggestions.GetRange(0, 3);

            return suggestions;
        }

        public RoleResult CalculateForRole(int userId, JobRole role)
        {
            List<UserSkill> userSkills = userSkillRepository.GetByUserId(userId);
            List<SkillGroup> groups = skillGroupRepository.GetByRole(role);

            int totalWeight = 0;
            foreach (SkillGroup group in groups)
                totalWeight += group.Weight;

            List<double> groupScores = new List<double>();
            foreach (SkillGroup group in groups)
                groupScores.Add(ComputeGroupScore(group, userSkills));

            double matchScore = ComputeMatchScore(groups, groupScores);

            RoleResult result = new RoleResult();
            result.JobRole = role;

            if (matchScore == -1)
            {
                result.MatchScore = -1;
                result.Suggestions = new List<Suggestion>();
                return result;
            }

            result.MatchScore = matchScore;
            result.Suggestions = IdentifyGaps(groups, userSkills, totalWeight);
            return result;
        }

        public List<RoleResult> CalculateAll(int userId)
        {
            List<RoleResult> results = new List<RoleResult>();
            foreach (JobRole role in Enum.GetValues(typeof(JobRole)))
                results.Add(CalculateForRole(userId, role));
            return results;
        }

        public List<Suggestion> GetSuggestions(RoleResult result)
        {
            return result.Suggestions;
        }
    }
}
