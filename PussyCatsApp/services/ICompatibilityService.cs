using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.services
{
    public interface ICompatibilityService
    {
        RoleResult CalculateForRole(int userId, JobRole role);

        List<RoleResult> CalculateAll(int userId);

        List<Suggestion> GetSuggestions(RoleResult result);
    }
}