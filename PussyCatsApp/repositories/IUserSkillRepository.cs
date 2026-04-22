using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public interface IUserSkillRepository
    {
        List<UserSkill> GetVerifiedSkillsByUserId(int userId);
        string GetParsedCvByUserId(int userId);
    }
}