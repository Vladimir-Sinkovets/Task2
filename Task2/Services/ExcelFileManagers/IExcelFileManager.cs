using Task2.Models;

namespace Task2.Services.ExcelFileManagers
{
    public interface IExcelFileManager
    {
        Task SaveAndUploadToDataBaseAsync(IFormFile uploadedFile);
        IEnumerable<ExcelFile> GetAllFiles();
        IEnumerable<RecordViewItem> GetRecordViewItemsForFile(int fileId);
    }
}
