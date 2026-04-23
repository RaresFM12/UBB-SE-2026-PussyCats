using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for exporting and previewing a user's CV as a PDF,
    /// handling navigation, loading, and integration with the export ViewModel.
    /// </summary>
    public sealed partial class ExportCVView : Page
    {
        public ExportCVViewModel ViewModel { get; private set; }
        private int userId;

        public ExportCVView()
        {
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int passedUserId)
            {
                userId = passedUserId;
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnPageLoaded;

            await CvWebView.EnsureCoreWebView2Async();
            CvWebView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

            var templateFolder = Path.Combine(AppContext.BaseDirectory, "resources");
            CvWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "assets.local",
                templateFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            IPdfExportService pdfExportService = new PdfExportService(CvWebView);
            ISkillTestRepository skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileRepository userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileService userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);

            ViewModel = new ExportCVViewModel(pdfExportService, userProfileService);
            ViewModel.UserId = userId;

            this.DataContext = ViewModel;

            await ViewModel.LoadAndRenderCVAsync();
        }

        private async void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("http") && !e.Uri.Contains("assets.local"))
            {
                e.Cancel = true;

                await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Uri));
            }
        }
    }
}