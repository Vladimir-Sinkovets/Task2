using Microsoft.AspNetCore.Mvc;
using Task2.Models;
using Task2.Services.ExcelFileManagers;

namespace Task2.Controllers
{
    public class FileController : Controller
    {
        private readonly IExcelFileManager _excelFileManager;

        public FileController(IExcelFileManager excelFileManager)
        {
            _excelFileManager = excelFileManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                await _excelFileManager.SaveAndUploadToDataBaseAsync(uploadedFile);
            }

            return View();
        }

        [HttpGet]
        public IActionResult AllFiles()
        {
            var files = _excelFileManager.GetAllFiles();

            return View(files);
        }

        [HttpGet]
        public IActionResult File(int fileId)
        {
            var viewModel = _excelFileManager.GetRecordViewItemsForFile(fileId);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
    }
}
