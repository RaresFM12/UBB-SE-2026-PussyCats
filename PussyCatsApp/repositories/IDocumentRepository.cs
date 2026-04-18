using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.Repositories
{
    public interface IDocumentRepository // : IRepository<Document>
    {
        List<Document> GetDocumentsByUserId(int userId);
        void AddDocument(Document document);
        void DeleteDocument(int documentId);
        // void RenameDocument(int documentId, string newName);
    }
}
