using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.services
{
    public interface ICompatibilityService
    {
        RoleResult CalculateForRole(int userId, JobRole role);

        List<RoleResult> CalculateAll(int userId);

        List<Suggestion> GetSuggestions(RoleResult result);
    }
}