using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.repositories;
using PussyCatsApp.services;
using PussyCatsApp.storage;
using PussyCatsApp.viewModels;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Pickers;

namespace PussyCatsApp.views
{
    /// <summary>
    /// View — responsible only for:
    ///   • rendering data exposed by the ViewModels
    ///   • handling UI events (file picker, Process.Start, dialogs)
    ///   • forwarding user intent to ViewModel methods
    /// No business logic or data-access lives here.
    /// </summary>
    public sealed partial class DocumentsPage : Page
    {
        private readonly DocumentListViewModel listViewModel;
        private readonly UploadDocumentViewModel uploadViewModel;

        public DocumentsPage()
        {
            this.InitializeComponent();

            // Build the dependency chain here, matching the pattern in
            // UserProfileView — the ViewModels themselves stay UI-free.
            int currentUserId = 1; // replace with real session user id

            var repo = new DocumentRepository();
            var storage = new LocalFileStorageService();
            var service = new DocumentService(repo, storage);

            listViewModel = new DocumentListViewModel(service, currentUserId);
            uploadViewModel = new UploadDocumentViewModel(service, currentUserId);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadGrid();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void LoadGrid()
        {
            listViewModel.LoadDocuments();
            var docs = listViewModel.GetDocuments();

            lvDocuments.ItemsSource = null;
            lvDocuments.ItemsSource = docs;

            lblNoDocuments.Visibility = docs.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void ShowUploadError(string message)
        {
            lblUploadError.Text = message;
            lblUploadError.Visibility = Visibility.Visible;
        }

        private void ShowStatus(string message)
        {
            lblStatus.Text = message;
            lblStatus.Visibility = Visibility.Visible;
        }

        // ── Upload section — View handles file picker (UI concern) ───────────

        private void OnDocumentNameChanged(object sender, TextChangedEventArgs e)
        {
            // Forward plain text to ViewModel; no logic here
            uploadViewModel.SetDocumentName(txtDocumentName.Text);
        }

        private async void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            // File picker is a UI/OS concern — lives in the View
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Hand the result to the ViewModel as plain data
                uploadViewModel.SetSelectedFilePath(file.Path);
                lblSelectedFile.Text = file.Name;
            }
        }

        private void OnUploadClick(object sender, RoutedEventArgs e)
        {
            lblUploadError.Visibility = Visibility.Collapsed;

            try
            {
                uploadViewModel.Upload();

                string error = uploadViewModel.GetErrorMessage();
                if (!string.IsNullOrEmpty(error))
                {
                    ShowUploadError(error);
                    return;
                }

                // Reset form on success
                txtDocumentName.Text = string.Empty;
                lblSelectedFile.Text = "No file selected";
                uploadViewModel.SetDocumentName(string.Empty);
                uploadViewModel.SetSelectedFilePath(null);

                LoadGrid();
            }
            catch (Exception ex)
            {
                ShowUploadError(ex.Message);
                Debug.WriteLine($"[DocumentsView] Upload error: {ex}");
            }
        }

        // ── List section — View handles OS launch (UI concern) ───────────────

        private async void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Document doc)
                return;

            // Confirmation dialog is a UI concern — lives in the View
            var dialog = new ContentDialog
            {
                Title = "Delete Document",
                Content = $"Are you sure you want to delete \"{doc.DocumentName}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;

            try
            {
                listViewModel.DeleteDocument(doc.Id);
                LoadGrid();
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message);
            }
        }

        private void OnViewClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Document doc)
                return;

            lblStatus.Visibility = Visibility.Collapsed;

            // Ask ViewModel to resolve the path (data/business logic)
            string fullPath = listViewModel.GetResolvedFilePath(doc.Id);

            string status = listViewModel.GetStatusMessage();
            if (!string.IsNullOrEmpty(status))
            {
                ShowStatus(status);
                return;
            }

            // Opening the file with the OS is a UI/OS concern — lives in the View
            if (File.Exists(fullPath))
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
            }
            else
            {
                ShowStatus($"The file \"{doc.DocumentName}\" could not be found on disk.");
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}