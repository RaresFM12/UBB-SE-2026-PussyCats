using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    public interface IUserSkillRepository
    {
        List<UserSkill> GetVerifiedSkillsByUserId(int userId);

        string GetParsedCvByUserId(int userId);
    }
}