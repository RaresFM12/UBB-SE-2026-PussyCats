using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp;
using PussyCatsApp.models;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

public class PdfExportService
{
    private readonly WebView2 _webView;

    public PdfExportService(WebView2 webView)
    {
        _webView = webView;
    }

    public async Task ExportAsync(UserProfile profile)
    {
        // 1. Navigate to the template and wait for it to fully load
        _webView.Source = new Uri("http://assets.local/CVHtmlTemplate.html");
        await WaitForNavigationAsync();

        // 2. Serialize the profile and call CVGenerator.generate()
        //    which injects data directly into the live DOM via element IDs.
        //    No document.write() — the DOM stays stable for PrintToPdfAsync.
        var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _webView.ExecuteScriptAsync($"CVGenerator.generate({json});");

        // 3. Wait for DOM updates to settle before printing
        await Task.Delay(500);

        // 4. A4 print settings — no margins, no header/footer
        //var printSettings = _webView.CoreWebView2.Environment.CreatePrintSettings();
        //printSettings.Orientation = CoreWebView2PrintOrientation.Portrait;
        //printSettings.PageWidth = 210;
        //printSettings.PageHeight = 297;
        //printSettings.MarginTop = 0;
        //printSettings.MarginBottom = 0;
        //printSettings.MarginLeft = 0;
        //printSettings.MarginRight = 0;
        //printSettings.ShouldPrintBackgrounds = true;
        //printSettings.ShouldPrintHeaderAndFooter = false;

        // 5. Save file picker
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.SuggestedFileName = $"{profile.FirstName} {profile.LastName} CV";
        savePicker.FileTypeChoices.Add("PDF Document", new[] { ".pdf" });

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle((App.Current as App).MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null) return;  // user cancelled

        // 6. Print via Chromium's native renderer
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
}