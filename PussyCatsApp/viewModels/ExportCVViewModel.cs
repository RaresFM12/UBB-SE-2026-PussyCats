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
                // Replace with a real repository call when moving to the profile page:
                // var profile = await _profileRepository.GetCurrentUserAsync();
                var profile = await LoadProfileFromJsonAsync("SampleCVs/sample_cv.json");

                if (profile == null)
                {
                    StatusText = "sample_cv.json not found or invalid.";
                    return;
                }

                await _pdfExportService.ExportProfileToPdfAsync(profile);
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