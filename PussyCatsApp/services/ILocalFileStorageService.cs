using System.IO;

namespace PussyCatsApp.storage
{
    public interface ILocalFileStorageService
    {
        string SaveFile(Stream fileStream, string originalFileName);
        void DeleteFile(string relativePath);
        string GetFilePath(string relativePath);
    }
}
