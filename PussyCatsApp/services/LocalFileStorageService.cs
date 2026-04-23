using System;
using System.IO;

namespace PussyCatsApp.Services
{
    public class LocalFileStorageService : ILocalFileStorageService
    {
        private string basePath = Path.Combine("uploads", "documents");

        public LocalFileStorageService()
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, basePath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public LocalFileStorageService(string basePath)
        {
            this.basePath = basePath;

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public string SaveFile(Stream fileStream, string originalFileName)
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            string fullPath = Path.IsPathRooted(basePath)
                ? Path.Combine(basePath, uniqueFileName)
                : Path.Combine(AppContext.BaseDirectory, basePath, uniqueFileName);

            using var output = File.Create(fullPath);

            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }
            fileStream.CopyTo(output);

            return fullPath;
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }
            string fullPath = Path.Combine(AppContext.BaseDirectory, basePath, Path.GetFileName(relativePath));
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public string GetFilePath(string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }
            string returnedPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            if (!Path.Exists(returnedPath))
            {
                throw new FileNotFoundException($"File not found at path: {relativePath}");
            }
            return returnedPath;
        }
    }
}