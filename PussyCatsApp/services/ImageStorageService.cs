using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.UI.Xaml.Media;

namespace PussyCatsApp.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private string basePath = Path.Combine("uploads", "avatars");
        private const int BytesPerKilobyte = 1024;
        private const int BytesPerMegabyte = 1024 * BytesPerKilobyte;
        private const int MaxFileSizeInMb = 5;
        private const int MaxFileSize = MaxFileSizeInMb * BytesPerMegabyte;

        public ImageStorageService()
        {
            string fullDirectoryPath = Path.Combine(AppContext.BaseDirectory, basePath);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }
        }

        public string SaveImage(Stream fileStream, string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new ArgumentException("Unsupported file type. Only .jpg, .jpeg, and .png are allowed.");
            }

            if (fileStream.Length > MaxFileSize)
            {
                throw new Exception("File size exceeds the maximum limit of 5MB.");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + extension;

            string fullPath = Path.Combine(AppContext.BaseDirectory, basePath, uniqueFileName);

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

            string fullPath = relativePath;
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}