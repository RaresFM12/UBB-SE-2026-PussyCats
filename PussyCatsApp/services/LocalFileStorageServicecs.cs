using System;
using System.IO;

namespace PussyCatsApp.storage
{
    public class LocalFileStorageService
    {
        private readonly string basePath;

        public LocalFileStorageService()
        {
            // Store files in <app startup dir>/uploads/documents/
            basePath = Path.Combine(
                AppContext.BaseDirectory,
                "uploads",
                "documents");

            Directory.CreateDirectory(basePath);
        }

        // ── Save a stream to disk, return the relative path ─────────────────
        // A GUID prefix avoids name collisions between users.
        public string saveFile(Stream fileStream, string originalFileName)
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            string fullPath = Path.Combine(basePath, uniqueFileName);

            using var output = File.Create(fullPath);
            fileStream.CopyTo(output);

            // Return relative path (stored in DB)
            return Path.Combine("uploads", "documents", uniqueFileName);
        }

        // ── Delete a file from disk given its relative path ─────────────────
        public void deleteFile(string relativePath)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        // ── Resolve a relative path to its full absolute path ────────────────
        public string getFilePath(string relativePath)
        {
            return Path.Combine(AppContext.BaseDirectory, relativePath);
        }
    }
}