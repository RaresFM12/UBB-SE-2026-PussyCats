using System;
using PussyCatsApp.Models;
using PussyCatsApp.Services;
using PussyCatsApp.utilities;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for the upload panel.
    /// The View is responsible for opening the file picker and calling
    /// SetSelectedFilePath() with the result.
    /// </summary>
    public class UploadDocumentViewModel
    {
        private string documentName = string.Empty;
        private string selectedFilePath = string.Empty;
        private string errorMessage = string.Empty;
        private readonly IDocumentService documentService;
        private readonly int userId;

        public UploadDocumentViewModel(IDocumentService documentService, int userId)
        {
            this.documentService = documentService;
            this.userId = userId;
        }

        public void SetSelectedFilePath(string path)
        {
            selectedFilePath = path;
        }

        public bool ValidateDocumentInput()
        {
            try
            {
                DocumentValidator.ValidateDocumentInput(documentName, selectedFilePath);
                errorMessage = string.Empty;
                return true;
            }
            catch (ArgumentException exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        public void UploadDocument()
        {
            if (!ValidateDocumentInput())
            {
                return;
            }

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