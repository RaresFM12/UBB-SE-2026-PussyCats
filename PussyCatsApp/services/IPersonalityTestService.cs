using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.Models;

namespace PussyCatsApp.services
{
    public interface IPersonalityTestService
    {
        Dictionary<TraitType, double> CalculateTraitScores(Dictionary<Question, AnswerValue> answers);

        Dictionary<JobRole, double> CalculateRoleScores(Dictionary<TraitType, double> traitScores);

        Dictionary<JobRole, double> GetTopRoles(Dictionary<JobRole, double> roleScores, int length);

        void SaveResult(int userId, string personalityTestResult);
    }
}
