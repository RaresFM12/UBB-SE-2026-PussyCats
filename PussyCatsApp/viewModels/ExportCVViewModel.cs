using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for exporting and previewing a user's CV as a PDF, handling loading state and export operations.
    /// </summary>
    public partial class ExportCVViewModel : ObservableObject
    {
        private readonly IPdfExportService pdfExportService;
        private readonly IUserProfileService userProfileService;
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

        public ExportCVViewModel(IPdfExportService pdfExportService, IUserProfileService userProfileService)
        {
            this.pdfExportService = pdfExportService;
            this.userProfileService = userProfileService;
        }

        public async Task LoadAndRenderCVAsync()
        {
            int minimumValidUserId = 1; // Assuming user IDs start from 1
            if (UserId < minimumValidUserId)
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
                StatusText = string.Empty; // User cancelled save picker
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