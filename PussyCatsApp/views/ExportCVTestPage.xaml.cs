using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using PussyCatsApp.viewModels;
using PussyCatsApp.repositories;

namespace PussyCatsApp.views
{
    public sealed partial class ExportCVTestPage : Page
    {
        // Exposed as a property so the XAML {x:Bind ViewModel.xxx} works
        public ExportCVViewModel ViewModel { get; private set; }

        public ExportCVTestPage()
        {
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            await HiddenWebView.EnsureCoreWebView2Async();

            var templateFolder = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "resources");

            HiddenWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "assets.local",
                templateFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            var userRepository = new UserProfileRepository();
            var pdfExportService = new PdfExportService(HiddenWebView, userRepository);
            ViewModel = new ExportCVViewModel(pdfExportService);

            this.DataContext = ViewModel;
        }
    }
}