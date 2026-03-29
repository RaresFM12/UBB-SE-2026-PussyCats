using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using PussyCatsApp.viewModels;

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
            // WebView2 must be initialised imperatively — this is the only
            // reason code-behind exists. Everything else lives in the ViewModel.
            await HiddenWebView.EnsureCoreWebView2Async();

            var templateFolder = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "resources");

            HiddenWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "assets.local",
                templateFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            // Build the service with the now-ready WebView2, then
            // construct the ViewModel and set it as the DataContext.
            var pdfExportService = new PdfExportService(HiddenWebView);
            ViewModel = new ExportCVViewModel(pdfExportService);

            // Notify the XAML bindings that ViewModel is now set
            this.DataContext = ViewModel;
        }
    }
}