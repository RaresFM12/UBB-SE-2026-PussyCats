using PussyCatsApp.models;
using PussyCatsApp.services;
using System;

namespace PussyCatsApp.viewModels
{
    /// <summary>
    /// ViewModel for the upload panel.
    /// The View is responsible for opening the file picker and calling
    /// SetSelectedFilePath() with the result.
    /// </summary>
    public class UploadDocumentViewModel
    {
        private string documentName;
        private string selectedFilePath;
        private string errorMessage;
        private readonly DocumentService documentService;
        private readonly int userId;

        public UploadDocumentViewModel(DocumentService documentService, int userId)
        {
            this.documentService = documentService;
            this.userId = userId;
        }

        public void SetSelectedFilePath(string path) => selectedFilePath = path;

        public bool ValidateDocumentInput()
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                errorMessage = "Please enter a name for the document.";
                return false;
            }

            if (string.IsNullOrEmpty(selectedFilePath))
            {
                errorMessage = "Please select a file to upload.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public void UploadDocument()
        {
            if (!ValidateDocumentInput())
                return;

            var document = new Document
            {
                UserId = userId,
                DocumentName = documentName
            };

            documentService.UploadDocument(document, selectedFilePath);
        }

        public string GetDocumentName() => documentName;
        public void SetDocumentName(string name) => documentName = name;
        public string GetErrorMessage() => errorMessage;
        public string GetSelectedFilePath() => selectedFilePath;
    }
}