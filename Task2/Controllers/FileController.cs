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
        private readonly IXLSXFileManager _xlsFileManager;

        public FileController(IWebHostEnvironment appEnvironment, IXLSXFileManager xlsFileManager)
        {
            _appEnvironment = appEnvironment;
            _xlsFileManager = xlsFileManager;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                _xlsFileManager.SaveAndUploadToDataBase(uploadedFile, _appEnvironment.WebRootPath + path);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
    }
}
