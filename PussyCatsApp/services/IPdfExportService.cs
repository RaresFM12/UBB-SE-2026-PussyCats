using System.Threading.Tasks;

using PussyCatsApp.Models;
namespace PussyCatsApp.Services
{
    public interface IPdfExportService
    {
        Task RenderProfileAsync(UserProfile profile);
        Task DownloadPdfAsync();
    }
}
