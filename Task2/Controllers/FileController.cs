using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Task2.Services.XLSXFileManager;

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

                var workbook = new Workbook(uploadedFile.OpenReadStream());

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path + "x", FileMode.Create))
                {
                    workbook.Save(fileStream, SaveFormat.Xlsx);
                    //await uploadedFile.CopyToAsync(fileStream);
                }

                _xlsFileManager.UploadToDatabase(_appEnvironment.WebRootPath + path + "x");
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
