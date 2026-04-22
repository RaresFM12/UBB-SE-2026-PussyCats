using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using Windows.Storage.Pickers;

public class PdfExportService : IPdfExportService
{
    private readonly WebView2 webView;
    private readonly IUserProfileRepository profileRepository;
    private UserProfile currentProfile;

    public PdfExportService(WebView2 webView)
    {
        this.webView = webView;
    }
    public async Task RenderProfileAsync(UserProfile profile)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
        }

        currentProfile = profile;

        webView.Source = new Uri("http://assets.local/CVHtmlTemplate.html");
        await WaitForNavigationAsync();

        var json = JsonSerializer.Serialize(currentProfile, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await webView.ExecuteScriptAsync($"CVGenerator.generate({json});");

        // Wait for DOM updates to settle
        await Task.Delay(500);
    }

    public async Task DownloadPdfAsync()
    {
        if (currentProfile == null)
        {
            throw new InvalidOperationException("Profile has not been rendered yet.");
        }

        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.SuggestedFileName = BuildFileName(currentProfile);
        savePicker.FileTypeChoices.Add("PDF Document", new[] { ".pdf" });

        var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, windowHandle);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
        {
            return;  // user cancelled
        }

        // Print via Chromium's native renderer
        var success = await webView.CoreWebView2.PrintToPdfAsync(file.Path, null);
        if (!success)
        {
            throw new InvalidOperationException("PDF generation failed. Please try again.");
        }
    }

    private Task WaitForNavigationAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        void Handler(WebView2 s, CoreWebView2NavigationCompletedEventArgs e)
        {
            webView.NavigationCompleted -= Handler;
            tcs.SetResult(true);
        }
        webView.NavigationCompleted += Handler;
        return tcs.Task;
    }

    private string BuildFileName(UserProfile profile)
    {
        var firstName = string.IsNullOrWhiteSpace(profile.FirstName) ? "FirstName" : profile.FirstName;
        var lastName = string.IsNullOrWhiteSpace(profile.LastName) ? "LastName" : profile.LastName;
        return $"{firstName}_{lastName}_CV.pdf";
    }
}