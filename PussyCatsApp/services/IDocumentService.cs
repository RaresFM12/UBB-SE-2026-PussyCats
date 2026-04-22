using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.Models;

namespace PussyCatsApp.services
{
    public interface IDocumentService
    {
        List<Document> GetDocumentsByUserId(int userId);

        void UploadDocument(Document document, string filePath);

        void DeleteDocument(int documentId);

        string GetDocumentAbsolutePath(int documentId);
    }
}
