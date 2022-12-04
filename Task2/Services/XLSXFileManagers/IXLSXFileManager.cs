using Task2.Models;

namespace Task2.Services.XLSXFileManagers
{
    public interface IXLSXFileManager
    {
        Task SaveAndUploadToDataBaseAsync(IFormFile uploadedFile, string folderPath);
        IEnumerable<XLSXFile> GetAllFiles();
        IEnumerable<Record> GetRecordsByFile(int fileId);
    }
}
