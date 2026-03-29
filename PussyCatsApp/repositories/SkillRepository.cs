using System;
using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    internal class SkillRepository : ISkillRepository
    {
        private readonly List<Skill> _skills = new();

        public Skill load(int id)
        {
            return _skills.FirstOrDefault(s => s.SkillId == id);
        }

        public void save(int id, Skill data)
        {
            var existing = _skills.FirstOrDefault(s => s.SkillId == id);
            if (existing != null)
            {
                existing.Name = data.Name;
                existing.Score = data.Score;
                existing.UserId = data.UserId;
                existing.AchievedDate = data.AchievedDate;
            }
            else
            {
                data.SkillId = id;
                _skills.Add(data);
            }
        }

        public List<Skill> GetSkillsByUserId(int userId)
        {
            return _skills.Where(s => s.UserId == userId).ToList();
        }

        public void AddSkill(Skill skill)
        {
            if (skill.SkillId == 0)
            {
                skill.SkillId = _skills.Count > 0 ? _skills.Max(s => s.SkillId) + 1 : 1;
            }
            _skills.Add(skill);
        }

        public void RemoveSkill(int skillId)
        {
            var skill = _skills.FirstOrDefault(s => s.SkillId == skillId);
            if (skill != null)
            {
                _skills.Remove(skill);
            }
        }

        public void UpdateSkillScore(int skillId, double score)
        {
            var skill = _skills.FirstOrDefault(s => s.SkillId == skillId);
            if (skill != null)
            {
                skill.Score = score;
            }
        }
    }
}
