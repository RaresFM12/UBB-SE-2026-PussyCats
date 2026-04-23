using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

public interface IPdfExportService
{
    Task RenderProfileAsync(UserProfile profile);
    Task DownloadPdfAsync();
}
