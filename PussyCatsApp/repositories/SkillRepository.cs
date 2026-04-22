using System;
using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly List<Skill> skills = new List<Skill>();

        public Skill Load(int skillId)
        {
            foreach (Skill skill in skills)
            {
                if (skill.SkillId == skillId)
                {
                    return skill;
                }
            }
            return null;
        }

        public void Save(int skillId, Skill data)
        {
            Skill existing = null;
            foreach (Skill skill in skills)
            {
                if (skill.SkillId == skillId)
                {
                    existing = skill;
                    break;
                }
            }

            if (existing != null)
            {
                existing.Name = data.Name;
                existing.Score = data.Score;
                existing.UserId = data.UserId;
                existing.AchievedDate = data.AchievedDate;
            }
            else
            {
                data.SkillId = skillId;
                skills.Add(data);
            }
        }

        public List<Skill> GetSkillsByUserId(int userId)
        {
            List<Skill> userSkills = new List<Skill>();
            foreach (Skill skill in skills)
            {
                if (skill.UserId == userId)
                {
                    userSkills.Add(skill);
                }
            }
            return userSkills;
        }

        public void AddSkill(Skill skill)
        {
            if (skill.SkillId == 0)
            {
                if (skills.Count == 0)
                {
                    skill.SkillId = 1;
                }
                else
                {
                    int maxId = 0;
                    foreach (Skill s in skills)
                    {
                        if (s.SkillId > maxId)
                        {
                            maxId = s.SkillId;
                        }
                    }
                    skill.SkillId = maxId + 1;
                }
            }
            skills.Add(skill);
        }

        public void RemoveSkill(int skillId)
        {
            Skill skillToRemove = null;
            foreach (Skill s in skills)
            {
                if (s.SkillId == skillId)
                {
                    skillToRemove = s;
                    break;
                }
            }

            if (skillToRemove != null)
            {
                skills.Remove(skillToRemove);
            }
        }

        public void UpdateSkillScore(int skillId, double score)
        {
            Skill skillToUpdate = null;
            foreach (Skill skill in skills)
            {
                if (skill.SkillId == skillId)
                {
                    skillToUpdate = skill;
                    break;
                }
            }

            if (skillToUpdate != null)
            {
                skillToUpdate.Score = score;
            }
        }
    }
}