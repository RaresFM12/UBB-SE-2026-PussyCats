using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.repositories
{
    internal interface IDocumentRepository : IRepository<Document>
    {
        List<Document> GetDocumentsByUserId(int userId);
        void AddDocument(Document document);
        void RemoveDocument(int documentId);
        void RenameDocument(int documentId, string newName);
    }
}
