using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Task2.Models;
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
        public IActionResult File(int fileId)
        {
            IList<Record> records = _xlsxFileManager.GetRecordsByFile(fileId)
                .OrderBy(r => r.AccountNumber)
                .ToList();

            var viewModel = CreateViewModel(records);

            return View(viewModel);
        }

        private static IEnumerable<RecordViewModel> CreateViewModel(IList<Record> records)
        {
            string currentClass = records.First().ClassName;

            var subClassesSum = new RecordViewModel() { Col1 = "10", ClassName = currentClass};
            var classesSum = new RecordViewModel() { Col1 = "ПО КЛАССУ", ClassName = currentClass };
            var sum = new RecordViewModel() { Col1 = "БАЛАНС" };

            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].AccountNumber[1] != subClassesSum.Col1[1])
                {
                    SumRecordViewModelValues(classesSum, subClassesSum);

                    yield return subClassesSum;

                    subClassesSum = new RecordViewModel() { Col1 = records[i].AccountNumber[..2], ClassName = records[i].ClassName };
                }
                if (records[i].ClassName != classesSum.ClassName)
                {
                    SumRecordViewModelValues(sum, classesSum);

                    yield return classesSum;

                    classesSum = new RecordViewModel() { Col1 = "ПО КЛАССУ", ClassName = records[i].ClassName };
                }

                var recordViewModel = MapRecord(records[i]);

                SumRecordViewModelValues(subClassesSum, recordViewModel);

                yield return recordViewModel;
            }
            yield return subClassesSum;
            yield return classesSum;
            yield return sum;
        } // remove

        private static void SumRecordViewModelValues(RecordViewModel target, RecordViewModel a)
        {
            target.OpeningBalanceAsset += a.OpeningBalanceAsset;
            target.OpeningBalanceLiabilities += a.OpeningBalanceLiabilities;
            target.TurnoverDebit += a.TurnoverDebit;
            target.TurnoverCredit += a.TurnoverCredit;
            target.FinalBalanceAsset += a.FinalBalanceAsset;
            target.FinalBalanceLiabilities += a.FinalBalanceLiabilities;
        }

        private static RecordViewModel MapRecord(Record record)
        {
            var result = new RecordViewModel()
            {
                Col1 = record.AccountNumber,
                ClassName = record.ClassName,
                TurnoverCredit = record.TurnoverCredit,
                TurnoverDebit = record.TurnoverDebit,
                OpeningBalanceAsset = record.OpeningBalanceAsset,
                OpeningBalanceLiabilities = record.OpeningBalanceLiabilities,
            };

            if(result.OpeningBalanceAsset == 0 && result.OpeningBalanceLiabilities == 0)
            {
                result.FinalBalanceLiabilities = 0;
                result.FinalBalanceAsset = 0;
            }
            else if (result.OpeningBalanceLiabilities == 0)
            {
                result.FinalBalanceAsset =  result.OpeningBalanceAsset + result.TurnoverDebit - result.TurnoverCredit;
                result.FinalBalanceLiabilities = 0;
            } 
            else if (result.OpeningBalanceAsset == 0)
            {
                result.FinalBalanceAsset = 0;
                result.FinalBalanceLiabilities = result.OpeningBalanceLiabilities - result.TurnoverDebit + result.TurnoverCredit;
            }

            return result;
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
    }
}
