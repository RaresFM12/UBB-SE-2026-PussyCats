using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PussyCatsApp.models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;


namespace PussyCatsApp.viewModels
{
    public partial class ExportCVViewModel : ObservableObject
    {
        private readonly PdfExportService _pdfExportService;

        public ExportCVViewModel(PdfExportService pdfExportService)
        {
            _pdfExportService = pdfExportService;
        }

        // ── Bindable state ────────────────────────────────────────────

        public string StatusText { get; set; } = string.Empty;

        public bool IsLoading { get; set; } = false;



        // ── Commands ──────────────────────────────────────────────────

        [RelayCommand]
        private async Task ExportCVAsync()
        {
            IsLoading = true;
            StatusText = "Generating PDF…";

            try
            {
                // The ViewModel delegates the entire operation to the service.
                // Assuming ID 1 for testing purposes.
                await _pdfExportService.ExportProfileToPdfAsync(1);

                StatusText = "Downloaded successfully!";
            }
            catch (OperationCanceledException)
            {
                StatusText = "Export cancelled.";
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

        // ── Data loading ──────────────────────────────────────────────
        // in case we still need to process json files for mocks

        private static async Task<UserProfile?> LoadProfileFromJsonAsync(string relativePath)
        {
            var installedPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            var filePath = Path.Combine(installedPath, relativePath);

            if (!File.Exists(filePath))
                return null;

            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<UserProfile>(stream,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}