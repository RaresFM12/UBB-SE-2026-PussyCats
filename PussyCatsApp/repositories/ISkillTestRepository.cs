using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public interface ISkillTestRepository : IRepository<SkillTest>
    {
        List<SkillTest> GetSkillTestsByUserId(int userId);
        void UpdateSkillTestScore(int skillId, int score);
        void UpdateAchievedDate(int skillId, DateOnly date);
    }
}
