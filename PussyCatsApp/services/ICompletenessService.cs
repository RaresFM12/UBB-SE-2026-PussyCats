using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface ICompletenessService
    {
        int CalculateCompleteness(UserProfile profile);
        string GetNextEmptyFieldPrompt(UserProfile profile);
    }
}
