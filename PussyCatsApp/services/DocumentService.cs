using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace PussyCatsApp.services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly LocalFileStorageService fileStorage;

        public DocumentService(IDocumentRepository documentRepository, LocalFileStorageService fileStorage)
        {
            this.documentRepository = documentRepository;
            this.fileStorage = fileStorage;
        }

        private bool ValidateFileType(string extension)
        {
            // ".pdf" => "PDF"
            string normalisedExtension = extension.TrimStart('.').ToUpperInvariant();
            return Enum.TryParse<AllowedFileType>(normalisedExtension, out _);
        }

        public List<Document> GetDocumentsByUserId(int userId)
        {
            return documentRepository.GetDocumentsByUserId(userId);

        }

        public void UploadDocument(Document document, string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (!ValidateFileType(extension))
                throw new InvalidOperationException(
                    "Invalid file type. Only PDF, JPG, and PNG files are accepted.");

            using var stream = File.OpenRead(filePath);
            string relativePath = fileStorage.SaveFile(stream, Path.GetFileName(filePath));

            document.FilePath = relativePath;
            document.UploadDate = DateTime.Now;

            documentRepository.AddDocument(document);

        }

        public void DeleteDocument(int documentId)
        {
            Document document = documentRepository.GetDocumentById(documentId);


            if (document == null)
                throw new InvalidOperationException("Document not found.");

            // delete the physical file before the database record
            if (!string.IsNullOrEmpty(document.FilePath))
                fileStorage.DeleteFile(document.FilePath);

            documentRepository.DeleteDocument(documentId);

        }

        public string GetDocumentAbsolutePath(int documentId)
        {
            Document document = documentRepository.GetDocumentById(documentId);


            if (document == null)
                throw new InvalidOperationException("Document not found.");

            return fileStorage.GetFilePath(document.FilePath);
        }
    }
}