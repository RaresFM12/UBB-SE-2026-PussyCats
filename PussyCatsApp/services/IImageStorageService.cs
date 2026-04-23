using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace PussyCatsApp.Services
{
    public interface IImageStorageService
    {
        string SaveImage(Stream fileStream, string fileName);
        void DeleteImage(string relativePath);
    }
}
