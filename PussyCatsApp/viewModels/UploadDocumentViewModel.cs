using PussyCatsApp.models;
using PussyCatsApp.services;
using System;

namespace PussyCatsApp.viewModels
{
    /// <summary>
    /// ViewModel for the upload panel.
    /// Holds only plain data and business logic — zero UI / OS references.
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

        // ── Called by the View after the file picker resolves ────────────────
        public void SetSelectedFilePath(string path) => selectedFilePath = path;

        // ── Validate both required inputs before uploading ───────────────────
        public bool ValidateInput()
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

        // ── Business logic: build Document and delegate to service ───────────
        public void Upload()
        {
            if (!ValidateInput())
                return;

            var doc = new Document
            {
                UserId = userId,
                DocumentName = documentName
            };

            documentService.upload(doc, selectedFilePath);
        }

        // ── Plain accessors the View reads ───────────────────────────────────
        public string GetDocumentName() => documentName;
        public void SetDocumentName(string name) => documentName = name;
        public string GetErrorMessage() => errorMessage;
        public string GetSelectedFilePath() => selectedFilePath;
    }
}