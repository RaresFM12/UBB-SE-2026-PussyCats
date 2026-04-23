using System;
using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class CompatibilityService : ICompatibilityService
    {
        private const int SkillsLineIndex = 2;
        private const char SkillDelimiter = ',';
        private const double UnverifiedSkillScore = 0.5;
        private const double ScoreNormalizationFactor = 100.0;
        private const double GapThreshold = 0.5;
        private const double TargetGroupScore = 0.8;
        private const int MaxSuggestions = 3;
        private const int InvalidScore = -1;

        private IUserSkillRepository userSkillRepository;
        private ISkillGroupRepository skillGroupRepository;

        public CompatibilityService(IUserSkillRepository userSkillRepository, ISkillGroupRepository skillGroupRepository)
        {
            this.userSkillRepository = userSkillRepository;
            this.skillGroupRepository = skillGroupRepository;
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
            {
                return extractedSkills;
            }

            string[] lines = parsedCv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length <= SkillsLineIndex)
            {
                return extractedSkills;
            }

            string skillsLine = lines[SkillsLineIndex].Trim();

            if (string.IsNullOrWhiteSpace(skillsLine))
            {
                return extractedSkills;
            }

            string[] cvSkills = skillsLine.Split(SkillDelimiter);

            foreach (string cvSkill in cvSkills)
            {
                string skillName = cvSkill.Trim();
                if (!string.IsNullOrWhiteSpace(skillName))
                {
                    extractedSkills.Add(skillName);
                }
            }

            return extractedSkills;
        }

        private List<UserSkill> MergeVerifiedAndUnverifiedSkills(List<UserSkill> verifiedSkills, List<string> cvSkills)
        {
            List<UserSkill> allSkills = new List<UserSkill>();

            foreach (UserSkill verifiedSkill in verifiedSkills)
            {
                allSkills.Add(verifiedSkill);
            }

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
                        {
                            ci = userSkill.Score / ScoreNormalizationFactor;
                        }
                        else
                        {
                            ci = UnverifiedSkillScore;
                        }

                        break;
                    }
                }

                if (ci > maxCi)
                {
                    maxCi = ci;
                }
            }

            return maxCi;
        }

        private double ComputeMatchScore(List<SkillGroup> groups, List<double> groupScores)
        {
            int totalWeight = 0;
            foreach (SkillGroup group in groups)
            {
                totalWeight += group.Weight;
            }

            if (totalWeight == 0)
            {
                return InvalidScore;
            }

            double weightedSum = 0;
            for (int i = 0; i < groups.Count; i++)
            {
                weightedSum += groups[i].Weight * groupScores[i];
            }

            return weightedSum * ScoreNormalizationFactor / totalWeight;
        }

        private double ComputeGain(SkillGroup group, double groupScore, int totalWeight)
        {
            return ScoreNormalizationFactor * group.Weight * (TargetGroupScore - groupScore) / totalWeight;
        }

        private List<Suggestion> IdentifyGaps(List<SkillGroup> groups, List<UserSkill> userSkills, int totalWeight)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            foreach (SkillGroup group in groups)
            {
                double groupScore = ComputeGroupScore(group, userSkills);
                if (groupScore > GapThreshold)
                {
                    continue;
                }

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
                            {
                                userHasVerified = true;
                            }
                            else
                            {
                                userHasUnverified = true;
                            }
                        }
                    }

                    if (userHasVerified)
                    {
                        continue;
                    }

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
                {
                    suggestions.Add(bestSuggestionForGroup);
                }
            }

            suggestions.Sort((a, b) => b.GainScore.CompareTo(a.GainScore));

            if (suggestions.Count > MaxSuggestions)
            {
                suggestions = suggestions.GetRange(0, MaxSuggestions);
            }

            return suggestions;
        }

        public RoleResult CalculateForRole(int userId, JobRole role)
        {
            List<UserSkill> userSkills = GetUserSkills(userId);
            List<SkillGroup> groups = skillGroupRepository.GetSkillsGroupByRole(role);

            int totalWeight = 0;
            foreach (SkillGroup group in groups)
            {
                totalWeight += group.Weight;
            }

            List<double> groupScores = new List<double>();
            foreach (SkillGroup group in groups)
            {
                groupScores.Add(ComputeGroupScore(group, userSkills));
            }

            double matchScore = ComputeMatchScore(groups, groupScores);

            RoleResult result = new RoleResult();
            result.JobRole = role;

            if (matchScore == InvalidScore)
            {
                result.MatchScore = InvalidScore;
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
            {
                results.Add(CalculateForRole(userId, role));
            }

            return results;
        }

        public List<Suggestion> GetSuggestions(RoleResult result)
        {
            return result.Suggestions;
        }
    }
}