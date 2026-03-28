using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    internal class DocumentRepository : IDocumentRepository
    {
        private readonly List<Document> _documents = new();

        public Document load(int id)
        {
            return _documents.FirstOrDefault(d => d.DocumentId == id);
        }

        public void save(int id, Document data)
        {
            var existing = _documents.FirstOrDefault(d => d.DocumentId == id);
            if (existing != null)
            {
                existing.StoredDocument = data.StoredDocument;
                existing.NameDocument = data.NameDocument;
                existing.UserId = data.UserId;
            }
            else
            {
                data.DocumentId = id;
                _documents.Add(data);
            }
        }

        public List<Document> GetDocumentsByUserId(int userId)
        {
            return _documents.Where(d => d.UserId == userId).ToList();
        }

        public void AddDocument(Document document)
        {
            if (document.DocumentId == 0)
            {
                document.DocumentId = _documents.Count > 0 ? _documents.Max(d => d.DocumentId) + 1 : 1;
            }
            _documents.Add(document);
        }

        public void RemoveDocument(int documentId)
        {
            var doc = _documents.FirstOrDefault(d => d.DocumentId == documentId);
            if (doc != null)
            {
                _documents.Remove(doc);
            }
        }

        public void RenameDocument(int documentId, string newName)
        {
            var doc = _documents.FirstOrDefault(d => d.DocumentId == documentId);
            if (doc != null)
            {
                doc.NameDocument = newName;
            }
        }
    }
}
