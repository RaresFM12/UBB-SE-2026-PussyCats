using PussyCatsApp.models;
using PussyCatsApp.repositories;
using PussyCatsApp.storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace PussyCatsApp.services
{
    public class DocumentService
    {
        private readonly DocumentRepository repository;
        private readonly LocalFileStorageService fileStorage;

        public DocumentService(DocumentRepository repository, LocalFileStorageService fileStorage)
        {
            this.repository = repository;
            this.fileStorage = fileStorage;
        }

        private bool validateFileType(string extension)
        {
            // ".pdf" => "PDF"
            string normalised = extension.TrimStart('.').ToUpperInvariant();
            return Enum.TryParse<AllowedFileType>(normalised, out _);
        }

        public List<Document> getDocumentsByUserId(int userId)
        {
            return repository.getDocumentsByUserId(userId);
        }

        public void uploadDocument(Document document, string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (!validateFileType(extension))
                throw new InvalidOperationException(
                    "Invalid file type. Only PDF, JPG, and PNG files are accepted.");

            using var stream = File.OpenRead(filePath);
            string relativePath = fileStorage.saveFile(stream, Path.GetFileName(filePath));

            document.FilePath = relativePath;
            document.UploadDate = DateTime.Now;

            repository.addDocument(document);
        }

        public void deleteDocument(int id)
        {
            Document document = repository.getDocumentById(id);

            if (document == null)
                throw new InvalidOperationException("Document not found.");

            // delete the physical file before the database record
            if (!string.IsNullOrEmpty(document.FilePath))
                fileStorage.deleteFile(document.FilePath);

            repository.deleteDocument(id);
        }

        public string getDocumentAbsolutePath(int id)
        {
            Document document = repository.getDocumentById(id);

            if (document == null)
                throw new InvalidOperationException("Document not found.");

            return fileStorage.getFilePath(document.FilePath);
        }
    }
}