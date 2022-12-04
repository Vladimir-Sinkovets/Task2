namespace Task2.Services.XLSXFileManagers
{
    public interface IXLSXFileManager
    {
        void SaveAndUploadToDataBase(IFormFile uploadedFile, string path);
    }
}
