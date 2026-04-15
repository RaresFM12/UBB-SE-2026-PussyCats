using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for exporting a user's CV to PDF format, managing preview rendering and file download.
    /// </summary>
    public partial class ExportCVViewModel : ObservableObject
    {
        private readonly PdfExportService pdfExportService;
        private readonly UserProfileService userProfileService;
        public int UserId { get; set; }

        private string statusText = string.Empty;
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public ExportCVViewModel(PdfExportService pdfExportService)
        {
            this.pdfExportService = pdfExportService;
            userProfileService = new UserProfileService();
        }

        public async Task LoadAndRenderCVAsync()
        {
            if (UserId <= 0)
            {
                return;
            }

            IsLoading = true;
            StatusText = "Loading CV preview...";

            try
            {
                var profile = userProfileService.GetProfile(UserId);

                if (profile == null)
                {
                    throw new Exception("User profile not found in database.");
                }

                await pdfExportService.RenderProfileAsync(profile);

                StatusText = string.Empty;
            }
            catch (Exception ex)
            {
                StatusText = $"Preview failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ExportCVAsync()
        {
            IsLoading = true;
            StatusText = "Saving PDF...";

            try
            {
                await pdfExportService.DownloadPdfAsync();
                StatusText = "Downloaded successfully!";
            }
            catch (OperationCanceledException)
            {
                StatusText = string.Empty; // User cancelled Save picker
            }
            catch (Exception ex)
            {
                StatusText = $"Export failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}