using PussyCatsApp.models;
using PussyCatsApp.services;
using System.Collections.Generic;

namespace PussyCatsApp.viewModels
{
    /// <summary>
    /// ViewModel for the document list panel.
    /// Holds data and delegates to the service — no UI, no OS process calls.
    /// Opening the file with the default application is the View's responsibility:
    /// the ViewModel exposes GetResolvedFilePath() so the View can act on it.
    /// </summary>
    public class DocumentListViewModel
    {
        private List<Document> documents;
        private string statusMessage;
        private readonly int userId;
        private readonly DocumentService documentService;

        public DocumentListViewModel(DocumentService documentService, int userId)
        {
            this.documentService = documentService;
            this.userId = userId;
            documents = new List<Document>();
        }

        // ── Load / refresh the list from the service ─────────────────────────
        public void LoadDocuments()
        {
            documents = documentService.getAll(userId);
        }

        // ── Return the list so the View can bind it ──────────────────────────
        public List<Document> GetDocuments() => documents;

        // ── Delete a record and auto-refresh ────────────────────────────────
        public void DeleteDocument(int id)
        {
            documentService.delete(id);
            LoadDocuments();
        }

        /// <summary>
        /// Resolves the absolute path for a document.
        /// Returns null and sets a status message if the document is not found.
        /// The View is responsible for checking the path exists and launching
        /// the OS default application — that is UI/OS behaviour, not VM logic.
        /// </summary>
        public string GetResolvedFilePath(int documentId)
        {
            try
            {
                string fullPath = documentService.getFilePath(documentId);
                statusMessage = string.Empty;
                return fullPath;
            }
            catch
            {
                statusMessage = "The file could not be found on disk.";
                return null;
            }
        }

        public string GetStatusMessage() => statusMessage;
    }
}