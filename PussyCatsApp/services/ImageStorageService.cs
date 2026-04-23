using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PussyCatsApp.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private string basePath = Path.Combine("uploads", "avatars");
        private const int BytesPerKilobyte = 1024;
        private const int BytesPerMegabyte = 1024 * BytesPerKilobyte;
        private const int MaxFileSizeInMb = 5;
        private const int MaxFileSize = MaxFileSizeInMb * BytesPerMegabyte;
        private readonly HashSet<string> allowedExtensions = new () { ".jpg", ".jpeg", ".png" };

        public ImageStorageService()
        {
            string fullDirectoryPath = Path.Combine(AppContext.BaseDirectory, basePath);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }
        }
        public ImageStorageService(string basePath)
        {
            this.basePath = basePath;

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public string SaveImage(Stream fileStream, string fileName)
        {
            string extension = GetImageExtension(fileName);
            CheckFileSize(fileStream);

            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string fullPath = Path.IsPathRooted(basePath)
                ? Path.Combine(basePath, uniqueFileName)
                : Path.Combine(AppContext.BaseDirectory, basePath, uniqueFileName);

            using (FileStream destinationStream = new FileStream(fullPath, FileMode.Create))
            {
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                fileStream.CopyTo(destinationStream);
            }

            return fullPath;
        }

        public void DeleteImage(string relativePath)
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

        private string GetImageExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Unsupported file type. Allowed formats are: {string.Join(", ", allowedExtensions.Order())}");
            }

            return extension;
        }
        public void CheckFileSize(Stream fileStream)
        {
            if (fileStream.Length > MaxFileSize)
            {
                throw new Exception("File size exceeds the maximum limit of " + MaxFileSize + "MB.");
            }
        }
    }
}