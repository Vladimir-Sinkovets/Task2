using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Task2.Services.XLSXFileManagers;

namespace Task2.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IXLSXFileManager _xlsxFileManager;

        public FileController(IWebHostEnvironment appEnvironment, IXLSXFileManager xlsFileManager)
        {
            _appEnvironment = appEnvironment;
            _xlsxFileManager = xlsFileManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string folderPath = $"{_appEnvironment.WebRootPath}/Files/";

                await _xlsxFileManager.SaveAndUploadToDataBaseAsync(uploadedFile, folderPath);
            }
            return View();
        }

        [HttpGet]
        public IActionResult AllFiles()
        {
            var files = _xlsxFileManager.GetAllFiles();

            return View(files);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
    }
}
