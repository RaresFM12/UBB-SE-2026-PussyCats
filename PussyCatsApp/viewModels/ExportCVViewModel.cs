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
                var userProfile = userProfileService.GetProfile(UserId);

                if (userProfile == null)
                {
                    throw new Exception("User userProfile not found in database.");
                }

                await pdfExportService.RenderProfileAsync(userProfile);

                StatusText = string.Empty;
            }
            catch (Exception exception)
            {
                StatusText = $"Preview failed: {exception.Message}";
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
            catch (Exception exception)
            {
                StatusText = $"Export failed: {exception.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}