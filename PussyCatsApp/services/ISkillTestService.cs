using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.Models;

namespace PussyCatsApp.services
{
    public interface ISkillTestService
    {
        List<SkillTest> GetTestsForUser(int userId);

        bool CanRetakeTest(int skillId);

        Badge SubmitRetake(int skillId, int newScore);
    }
}
