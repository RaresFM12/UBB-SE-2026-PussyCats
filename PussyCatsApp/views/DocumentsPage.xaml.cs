using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PussyCatsApp.models;
using PussyCatsApp.repositories;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PussyCatsApp.views
{
    public sealed partial class DocumentsPage : Page
    {
        private readonly DocumentRepository _documentRepository = new();
        private readonly ObservableCollection<Document> _documents = new();
        private const int CurrentUserId = 1;

        public DocumentsPage()
        {
            this.InitializeComponent();
            DocumentsListView.ItemsSource = _documents;
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            _documents.Clear();
            foreach (var doc in _documentRepository.GetDocumentsByUserId(CurrentUserId))
            {
                _documents.Add(doc);
            }
            EmptyText.Visibility = _documents.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void UploadDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".doc");
            picker.FileTypeFilter.Add(".docx");
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".xml");

            var window = (Application.Current as App)?.MainWindow;
            if (window == null) return;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var content = await FileIO.ReadTextAsync(file);
                    var document = new Document { UserId = CurrentUserId, NameDocument = file.Name, StoredDocument = content };
                    _documentRepository.AddDocument(document);
                    _documents.Add(document);
                    EmptyText.Visibility = Visibility.Collapsed;
                    ShowInfo("Document uploaded successfully.", InfoBarSeverity.Success);
                }
                catch (Exception ex)
                {
                    ShowInfo("Error: " + ex.Message, InfoBarSeverity.Error);
                }
            }
        }

        private async void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Document doc)
            {
                var inputBox = new TextBox { Text = doc.NameDocument, PlaceholderText = "Enter new name" };
                var dialog = new ContentDialog { Title = "Rename Document", Content = inputBox, PrimaryButtonText = "Rename", CloseButtonText = "Cancel", XamlRoot = this.XamlRoot };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    _documentRepository.RenameDocument(doc.DocumentId, inputBox.Text.Trim());
                    doc.NameDocument = inputBox.Text.Trim();
                    var idx = _documents.IndexOf(doc);
                    if (idx >= 0) { _documents.RemoveAt(idx); _documents.Insert(idx, doc); }
                    ShowInfo("Document renamed.", InfoBarSeverity.Success);
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Document doc)
            {
                _documentRepository.RemoveDocument(doc.DocumentId);
                _documents.Remove(doc);
                EmptyText.Visibility = _documents.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                ShowInfo("Document removed.", InfoBarSeverity.Success);
            }
        }

        private void ShowInfo(string message, InfoBarSeverity severity)
        {
            DocumentInfoBar.Message = message;
            DocumentInfoBar.Severity = severity;
            DocumentInfoBar.IsOpen = true;
        }
    }
}