using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    internal interface IDocumentRepository : IRepository<Document>
    {
        Document GetDocumentById(int documentId);
        List<Document> GetDocumentsByUserId(int userId);
        void AddDocument(Document document);
        void DeleteDocument(int documentId);
    }
}