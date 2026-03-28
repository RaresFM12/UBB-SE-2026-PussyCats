using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.UI.Xaml.Media;

namespace PussyCatsApp.services
{
    internal class ImageStorageService
    {
        private string basePath = Path.Combine("uploads", "avatars");

        public ImageStorageService()
        {
            string fullDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), basePath);

            if(!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }


        }

        public string SaveImage(Stream fileStream, string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if(extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new ArgumentException("Unsupported file type. Only .jpg, .jpeg, and .png are allowed.");
            }

            if(fileStream.Length > 5 * 1024 * 1024)
            {
                throw new Exception("File size exceeds the maximum limit of 5MB.");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + extension;

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(),basePath, uniqueFileName);

            using( FileStream destinationStream = new FileStream(fullPath, FileMode.Create))
            {
                if(fileStream.CanSeek)
                    fileStream.Position = 0;

                fileStream.CopyTo(destinationStream);
            }

            return Path.Combine(basePath, uniqueFileName);
        }

        public void DeleteImage(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

        }
    }
}
