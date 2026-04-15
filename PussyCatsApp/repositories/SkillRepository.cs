using System;
using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    internal class SkillRepository : ISkillRepository
    {
        private readonly List<Skill> skills = new ();

        public Skill Load(int skillId)
        {
            return skills.FirstOrDefault(s => s.SkillId == skillId);
        }

        public void Save(int skillId, Skill data)
        {
            var existing = skills.FirstOrDefault(s => s.SkillId == skillId);
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
            return skills.Where(s => s.UserId == userId).ToList();
        }

        public void AddSkill(Skill skill)
        {
            if (skill.SkillId == 0)
            {
                skill.SkillId = skills.Count > 0 ? skills.Max(s => s.SkillId) + 1 : 1;
            }
            skills.Add(skill);
        }

        public void RemoveSkill(int skillId)
        {
            var skill = skills.FirstOrDefault(s => s.SkillId == skillId);
            if (skill != null)
            {
                skills.Remove(skill);
            }
        }

        public void UpdateSkillScore(int skillId, double score)
        {
            var skill = skills.FirstOrDefault(s => s.SkillId == skillId);
            if (skill != null)
            {
                skill.Score = score;
            }
        }
    }
}
