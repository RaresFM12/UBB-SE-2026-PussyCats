using System;
using System.IO;

namespace PussyCatsApp.Storage
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

        // A GUID prefix avoids name collisions between users.
        public string SaveFile(Stream fileStream, string originalFileName)
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            string fullPath = Path.Combine(basePath, uniqueFileName);

            using var output = File.Create(fullPath);
            fileStream.CopyTo(output);

            // Return relative path (stored in DB)
            return Path.Combine("uploads", "documents", uniqueFileName);
        }
        public void DeleteFile(string relativePath)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public string GetFilePath(string relativePath)
        {
            return Path.Combine(AppContext.BaseDirectory, relativePath);
        }
    }
}