using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PussyCatsApp.viewModels
{
    public partial class ExportCVViewModel : ObservableObject
    {
        private readonly PdfExportService _pdfExportService;

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

        public async Task LoadAndRenderCVAsync()
        {
            if (UserId <= 0) return;

            IsLoading = true;
            StatusText = "Loading CV preview...";

            try
            {
                await _pdfExportService.RenderProfileAsync(UserId);
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
                await _pdfExportService.DownloadPdfAsync();
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