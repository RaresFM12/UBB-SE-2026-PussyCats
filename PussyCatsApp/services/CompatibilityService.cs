using PussyCatsApp.Models;
using PussyCatsApp.repositories;
using System;
using System.Collections.Generic;

namespace PussyCatsApp.services
{
    public class CompatibilityService
    {
        private UserSkillRepository userSkillRepository;
        private SkillGroupRepository skillGroupRepository;

        public CompatibilityService()
        {
            this.userSkillRepository = new UserSkillRepository();
            this.skillGroupRepository = new SkillGroupRepository();
        }

        private List<UserSkill> GetUserSkills(int userId)
        {
            List<UserSkill> verifiedSkills = userSkillRepository.GetVerifiedSkillsByUserId(userId);
            string parsedCv = userSkillRepository.GetParsedCvByUserId(userId);

            List<string> cvSkills = ExtractSkillsFromParsedCv(parsedCv);

            return MergeVerifiedAndUnverifiedSkills(verifiedSkills, cvSkills);
        }

        private List<string> ExtractSkillsFromParsedCv(string parsedCv)
        {
            List<string> extractedSkills = new List<string>();

            if (string.IsNullOrWhiteSpace(parsedCv))
                return extractedSkills;

            string[] lines = parsedCv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length < 3)
                return extractedSkills;

            string skillsLine = lines[2].Trim();

            if (string.IsNullOrWhiteSpace(skillsLine))
                return extractedSkills;

            string[] cvSkills = skillsLine.Split(',');

            foreach (string cvSkill in cvSkills)
            {
                string skillName = cvSkill.Trim();
                if (!string.IsNullOrWhiteSpace(skillName))
                    extractedSkills.Add(skillName);
            }

            return extractedSkills;
        }

        private List<UserSkill> MergeVerifiedAndUnverifiedSkills(List<UserSkill> verifiedSkills, List<string> cvSkills)
        {
            List<UserSkill> allSkills = new List<UserSkill>();

            foreach (UserSkill verifiedSkill in verifiedSkills)
                allSkills.Add(verifiedSkill);

            foreach (string cvSkill in cvSkills)
            {
                bool alreadyExists = false;

                foreach (UserSkill existingSkill in allSkills)
                {
                    if (string.Equals(existingSkill.SkillName, cvSkill, StringComparison.OrdinalIgnoreCase))
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    allSkills.Add(new UserSkill
                    {
                        SkillName = cvSkill,
                        IsVerified = false,
                        Score = 0
                    });
                }
            }

            return allSkills;
        }

        private double ComputeGroupScore(SkillGroup group, List<UserSkill> userSkills)
        {
            double maxCi = 0;

            foreach (string skill in group.Skills)
            {
                double ci = 0;

                foreach (UserSkill userSkill in userSkills)
                {
                    if (string.Equals(userSkill.SkillName, skill, StringComparison.OrdinalIgnoreCase))
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
                double groupScore = ComputeGroupScore(group, userSkills);
                if (groupScore > 0.5)
                    continue;

                Suggestion bestSuggestionForGroup = null;

                foreach (string skill in group.Skills)
                {
                    bool userHasVerified = false;
                    bool userHasUnverified = false;

                    foreach (UserSkill userSkill in userSkills)
                    {
                        if (string.Equals(userSkill.SkillName, skill, StringComparison.OrdinalIgnoreCase))
                        {
                            if (userSkill.IsVerified)
                                userHasVerified = true;
                            else
                                userHasUnverified = true;
                        }
                    }

                    if (userHasVerified)
                        continue;

                    double gain = ComputeGain(group, groupScore, totalWeight);

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
            List<UserSkill> userSkills = GetUserSkills(userId);
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