using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
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
        // 1. Inject the user data into the template
        var json = JsonSerializer.Serialize(BuildJsProfile(profile), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _webView.Source = new Uri("http://assets.local/CVHtmlTemplate.html");
        await WaitForNavigationAsync();
        await _webView.ExecuteScriptAsync($"CVGenerator.generate({json});");

        // Small delay to let the JS finish injecting data into the DOM
        await Task.Delay(300);

        // 2. Build print settings that match A4
        var printSettings = _webView.CoreWebView2.Environment.CreatePrintSettings();
        printSettings.Orientation         = CoreWebView2PrintOrientation.Portrait;
        printSettings.PageWidth           = 210;   // mm
        printSettings.PageHeight          = 297;   // mm
        printSettings.MarginTop           = 0;
        printSettings.MarginBottom        = 0;
        printSettings.MarginLeft          = 0;
        printSettings.MarginRight         = 0;
        printSettings.PrintBackgrounds    = true;
        printSettings.ShouldPrintHeaderAndFooter = false;

        // 3. Pick a save location
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation  = PickerLocationId.DocumentsLibrary;
        savePicker.SuggestedFileName       = $"{profile.FirstName} {profile.LastName} CV";
        savePicker.FileTypeChoices.Add("PDF Document", new[] { ".pdf" });

        // WinUI 3 requires the picker to be associated with the window
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null) return;   // user cancelled

        // 4. Print directly to the chosen path — uses the native print pipeline,
        //    so the output is pixel-perfect identical to the print preview
        var success = await _webView.CoreWebView2.PrintToPdfAsync(
            file.Path, printSettings);

        if (!success)
            throw new InvalidOperationException(
                "PDF generation failed. Please try again.");
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

    private static object BuildJsProfile(UserProfile p) => new
    {
        firstName   = p.FirstName,
        lastName    = p.LastName,
        email       = p.Email,
        phoneNumber = p.PhoneNumber,
        city        = p.Address?.City,
        country     = p.Address?.Country,
        github      = p.GithubAccount,
        linkedin    = p.LinkedinAccount,

        education = p.Education?.Select(e => new
        {
            university  = e.University,
            programName = e.ProgramName,
            startYear   = e.StartYear?.ToString(),
            endYear     = e.EndYear?.ToString(),
            courses     = e.RelevantCourses,
            description = e.Description,
        }),

        workExperience = p.WorkExperiences?.Select(w => new
        {
            jobTitle    = w.JobTitle,
            company     = w.Company,
            startDate   = w.StartDate.ToString("MMM yyyy"),
            endDate     = w.EndDate.HasValue
                            ? w.EndDate.Value.ToString("MMM yyyy")
                            : null,
            description = w.Description,
        }),

        projects = p.Projects?.Select(pr => new
        {
            name         = pr.Name,
            description  = pr.Description,
            technologies = pr.Technologies,
            startYear    = pr.StartYear?.ToString(),
            endYear      = pr.EndYear?.ToString(),
            url          = pr.Url,
        }),

        extraCurricular = p.ExtraCurricularActivities?.Select(a => new
        {
            activityName = a.ActivityName,
            organisation = a.Organisation,
            role         = a.Role,
            period       = a.Period,
            description  = a.Description,
        }),

        certificates = p.Certificates?.Select(c => new
        {
            name       = c.Name,
            uploadDate = c.UploadDate.ToString("dd.MM.yyyy"),
        }),

        skills = p.Skills,
    };
}