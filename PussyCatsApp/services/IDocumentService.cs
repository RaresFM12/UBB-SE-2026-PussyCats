using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface IDocumentService
    {
        List<Document> GetDocumentsByUserId(int userId);

        void UploadDocument(Document document, string filePath);

        void DeleteDocument(int documentId);

        string GetDocumentAbsolutePath(int documentId);
    }
}
