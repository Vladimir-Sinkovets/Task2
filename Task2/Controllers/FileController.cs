using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Task2.Services.XLSFileManager;

namespace Task2.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IXLSFileManager _xlsFileManager;

        public FileController(IWebHostEnvironment appEnvironment, IXLSFileManager xlsFileManager)
        {
            _appEnvironment = appEnvironment;
            _xlsFileManager = xlsFileManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                _xlsFileManager.UploadToDatabase(new StreamReader(uploadedFile.OpenReadStream()));
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
