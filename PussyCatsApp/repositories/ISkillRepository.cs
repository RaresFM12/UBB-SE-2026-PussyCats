using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.repositories
{
    internal interface ISkillRepository : IRepository<Skill>
    {
        List<Skill> GetSkillsByUserId(int userId);
        void AddSkill(Skill skill);
        void RemoveSkill(int skillId);
        void UpdateSkillScore(int skillId, double score);
    }
}
