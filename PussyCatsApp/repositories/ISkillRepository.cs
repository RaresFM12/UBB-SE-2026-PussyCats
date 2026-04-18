using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.Repositories
{
    internal interface ISkillRepository : IRepository<Skill>
    {
        List<Skill> GetSkillsByUserId(int userId);
        void AddSkill(Skill skill);
        void RemoveSkill(int skillId);
        void UpdateSkillScore(int skillId, double score);
    }
}
