using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories
{
    public interface ISkillTestRepository:IRepository<SkillTest>
    {
        List<SkillTest> GetSkillTestsByUserId(int userId);
        void UpdateSkillTestScore(int skillId, int score);
        void UpdateAchievedDate(int skillId, DateOnly date);
    }
}
