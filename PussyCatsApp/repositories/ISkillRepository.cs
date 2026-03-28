using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories
{
    internal interface ISkillRepository:IRepository<Skill>
    {
        List<Skill> GetSkillsByUserId(int userId);
        void UpdateSkillScore(int skillId, int score);
        void UpdateAchievedDate(int skillId, DateOnly date);
    }
}
