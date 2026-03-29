using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp.models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PussyCatsApp.views
{
    public sealed partial class ExportCVTestPage : Page
    {
        private PdfExportService? _pdfExportService;

        public ExportCVTestPage()
        {
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // Initialise WebView2 core before anything else
            await HiddenWebView.EnsureCoreWebView2Async();

            // Map Assets/CvTemplate/ folder to http://assets.local/
            // so the WebView can load HTML, CSS and JS as if from a web server.
            var templateFolder = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "resources");

            System.Diagnostics.Debug.WriteLine($"[ExportCVTestPage] templateFolder resolved to: {templateFolder}");


            HiddenWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "assets.local",
                templateFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            _pdfExportService = new PdfExportService(HiddenWebView);
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pdfExportService == null)
            {
                StatusText.Text = "WebView2 not ready yet, please try again.";
                return;
            }

            ExportButton.IsEnabled = false;
            LoadingRing.IsActive = true;
            StatusText.Text = "Generating PDF…";

            try
            {
                // ── Swap this for a real repository call on the actual profile page ──
                // e.g.:  var profile = await _profileRepository.GetCurrentUserAsync();
                var profile = await LoadProfileFromJsonAsync("../SampleCVs/sample_cv.json");

                Debug.WriteLine($"[ExportCVTestPage] Loaded profile: {JsonSerializer.Serialize(profile)}");

                if (profile == null)
                {
                    StatusText.Text = "sample_cv.json not found or invalid.";
                    return;
                }

                await _pdfExportService.ExportAsync(profile);

                StatusText.Text = "Downloaded successfully!";
            }
            catch (OperationCanceledException)
            {
                StatusText.Text = "Export cancelled.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Export failed: {ex.Message}";
            }
            finally
            {
                ExportButton.IsEnabled = true;
                LoadingRing.IsActive = false;
            }
        }

        private static async Task<UserProfile?> LoadProfileFromJsonAsync(string fileName)
        {
            var installedPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            var filePath = Path.Combine(installedPath, fileName);

            if (!File.Exists(filePath))
                return null;

            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<UserProfile>(stream, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }); 
        }

        // ── Hardcoded test data matching the real UserProfile model ──────


        private static UserProfile BuildTestProfile() => new UserProfile
        {
            FirstName = "Maria",
            LastName = "Ionescu",
            Email = "maria.ionescu@example.com",
            PhoneNumber = "+40 721 123 456",
            City = "Bucharest",
            Country = "Romania",
            GitHub = "https://github.com/mariaionescu",
            LinkedIn = "https://linkedin.com/in/mariaionescu",
            University = "Politehnica University of Bucharest",
            Degree = "BSc Computer Science",
            UniversityStartYear = 2022,
            ExpectedGraduationYear = 2025,
            Motivation = "Passionate about building scalable systems.",
            ActiveAccount = true,

            WorkExperiences = new()
            {
                new WorkExperience
                {
                    JobTitle         = "Software Engineer Intern",
                    Company          = "Endava Bucharest",
                    StartDate        = new DateTime(2024, 6, 1),
                    EndDate          = new DateTime(2024, 9, 1),
                    CurrentlyWorking = false,
                    Description      = "Developed REST APIs in Spring Boot; reduced query latency by 22%.",
                },
                new WorkExperience
                {
                    JobTitle         = "Full-Stack Developer",
                    Company          = "Freelance",
                    StartDate        = new DateTime(2023, 1, 1),
                    EndDate          = null,
                    CurrentlyWorking = true,
                    Description      = "Shipped three SaaS products with 400+ active users.",
                },
            },

            Projects = new()
            {
                new Project
                {
                    Name         = "JobMatch Platform",
                    Description  = "Candidate–recruiter matching with personality assessment.",
                    Technologies = new() { "React", "Spring Boot", "PostgreSQL", "Docker" },
                    Url          = "https://github.com/mariaionescu/jobmatch",
                },
                new Project
                {
                    Name         = "Budget Tracker PWA",
                    Description  = "Offline-capable personal finance tracker.",
                    Technologies = new() { "Vue.js", "FastAPI", "MongoDB" },
                    Url          = "",
                },
            },

            ExtraCurricularActivities = new()
            {
                new ExtraCurricularActivity
                {
                    ActivityName = "Google Developer Student Club",
                    Organization = "Politehnica University",
                    Role         = "Technical Lead",
                    Period       = "2023 – present",
                    Description  = "Organised cloud & containerisation workshops for 120+ members.",
                },
            },

            RelevantCertificates = new()
            {
                "AWS Certified Developer – Associate",
                "Oracle Certified Professional: Java SE 17",
            },

            Skills = new()
            {
                "Java", "Python", "TypeScript", "Spring Boot", "Django",
                "React", "PostgreSQL", "MongoDB", "Docker", "Kubernetes",
                "AWS", "Git", "REST API", "Agile/Scrum", "Figma",
            },
        };
    }
}
