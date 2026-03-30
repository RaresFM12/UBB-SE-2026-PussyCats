using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp;
using PussyCatsApp.models;
using PussyCatsApp.repositories;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

public class PdfExportService
{
    private readonly WebView2 _webView;
    private readonly IUserProileRepository _profileRepository;

    // Inject the repository into the service
    public PdfExportService(WebView2 webView, IUserProileRepository profileRepository)
    {
        _webView = webView;
        _profileRepository = profileRepository;
    }

    public async Task ExportProfileToPdfAsync(int userId)
    {
        // Fetch the data
        var profile = _profileRepository.getProfileById(userId);

        if (profile == null)
            throw new InvalidOperationException("User profile not found in database.");

        _webView.Source = new Uri("http://assets.local/CVHtmlTemplate.html");
        await WaitForNavigationAsync();

        var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _webView.ExecuteScriptAsync($"CVGenerator.generate({json});");

        // Wait for DOM updates to settle before printing
        await Task.Delay(500);

        // Save file picker
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.SuggestedFileName = BuildFileName(profile);
        savePicker.FileTypeChoices.Add("PDF Document", new[] { ".pdf" });

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null) return;  // user cancelled

        // Print via Chromium's native renderer
        var success = await _webView.CoreWebView2.PrintToPdfAsync(file.Path, null);
        if (!success)
            throw new InvalidOperationException("PDF generation failed. Please try again.");
    }

    private Task WaitForNavigationAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        void Handler(WebView2 s, CoreWebView2NavigationCompletedEventArgs e)
        {
            _webView.NavigationCompleted -= Handler;
            tcs.SetResult(true);
        }
        _webView.NavigationCompleted += Handler;
        return tcs.Task;
    }

    private string BuildFileName(UserProfile profile)
    {
        var firstName = string.IsNullOrWhiteSpace(profile.FirstName) ? "FirstName" : profile.FirstName;
        var lastName = string.IsNullOrWhiteSpace(profile.LastName) ? "LastName" : profile.LastName;
        return $"{firstName}_{lastName}_CV.pdf";
    }
}
