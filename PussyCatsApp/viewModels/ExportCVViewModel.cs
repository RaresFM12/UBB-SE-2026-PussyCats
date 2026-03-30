using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PussyCatsApp.viewModels
{
    public partial class ExportCVViewModel : ObservableObject
    {
        private readonly PdfExportService _pdfExportService;

        // ADD THIS: A simple property to hold the ID
        public int UserId { get; set; }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ExportCVViewModel(PdfExportService pdfExportService)
        {
            _pdfExportService = pdfExportService;
        }

        // CHANGE THIS: Remove the (int userId) parameter
        [RelayCommand]
        private async Task ExportCVAsync()
        {
            // Check the property instead
            if (UserId <= 0)
            {
                StatusText = "No valid user profile loaded.";
                return;
            }

            IsLoading = true;
            StatusText = "Generating PDF…";

            try
            {
                await _pdfExportService.ExportProfileToPdfAsync(UserId);
                StatusText = string.Empty;
            }
            catch (OperationCanceledException)
            {
                StatusText = string.Empty;
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