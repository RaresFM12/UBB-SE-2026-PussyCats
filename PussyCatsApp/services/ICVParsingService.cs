using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface ICVParsingService
    {
        UserProfile ParseCVFile(string content, string fileType);
    }
}
