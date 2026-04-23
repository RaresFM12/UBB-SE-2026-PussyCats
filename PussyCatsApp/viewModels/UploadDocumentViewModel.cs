using System;
using PussyCatsApp.Models;
using PussyCatsApp.Services;
using PussyCatsApp.Utilities;

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

        public void SetSelectedFilePath(string filePath)
        {
            selectedFilePath = filePath;
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

        public string GetDocumentName()
        {
            return documentName;
        }
        public void SetDocumentName(string documentName)
        {
            this.documentName = documentName;
        }
        public string GetErrorMessage()
        {
            return errorMessage;
        }
        public string GetSelectedFilePath()
        {
            return selectedFilePath;
        }
    }
}