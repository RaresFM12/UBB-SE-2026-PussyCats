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

        // ── Validate that the file extension is in the allowed list ──────────
        private bool validateFileType(string extension)
        {
            // Strip leading dot and uppercase so ".pdf" == "PDF"
            string normalised = extension.TrimStart('.').ToUpperInvariant();
            return Enum.TryParse<AllowedFileType>(normalised, out _);
        }

        // ── Return all documents belonging to a user ─────────────────────────
        public List<Document> getAll(int userId)
        {
            return repository.getByUserId(userId);
        }

        // ── Upload: validate → save file → persist DB record ────────────────
        public void upload(Document doc, string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (!validateFileType(extension))
                throw new InvalidOperationException(
                    "Invalid file type. Only PDF, JPG, and PNG files are accepted.");

            using var stream = File.OpenRead(filePath);
            string relativePath = fileStorage.saveFile(stream, Path.GetFileName(filePath));

            doc.FilePath = relativePath;
            doc.UploadDate = DateTime.Now;

            repository.add(doc);
        }

        // ── Delete: remove file from disk first, then remove DB record ───────
        public void delete(int id)
        {
            Document doc = repository.getById(id);

            if (doc == null)
                throw new InvalidOperationException("Document not found.");

            // Always delete the physical file before the database record
            if (!string.IsNullOrEmpty(doc.FilePath))
                fileStorage.deleteFile(doc.FilePath);

            repository.delete(id);
        }

        // ── Resolve full absolute path for a stored document ─────────────────
        public string getFilePath(int id)
        {
            Document doc = repository.getById(id);

            if (doc == null)
                throw new InvalidOperationException("Document not found.");

            return fileStorage.getFilePath(doc.FilePath);
        }
    }
}