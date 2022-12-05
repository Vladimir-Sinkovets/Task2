using OfficeOpenXml;
using Aspose.Cells;
using Task2.Models;
using Task2.Data;
using Microsoft.Extensions.Options;
using Task2.Services.ExcelFileManagers.Options;

namespace Task2.Services.ExcelFileManagers
{
    public class ExcelFileManager : IExcelFileManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IOptions<FilesSaveSettings> _options;

        public ExcelFileManager(IWebHostEnvironment appEnvironment, ApplicationDbContext context, IOptions<FilesSaveSettings> options)
        {
            _appEnvironment = appEnvironment;
            _context = context;
            _options = options;
        }


        public async Task SaveAndUploadToDataBaseAsync(IFormFile uploadedFile)
        {
            string folderPath = _appEnvironment.WebRootPath + _options.Value.FolderPath;

            var fileName = SaveFile(uploadedFile, folderPath);

            await UploadToDatabaseAsync(fileName);
        }

        public IEnumerable<ExcelFile> GetAllFiles()
        {
            return _context.Files;
        }

        public IEnumerable<RecordViewItem> GetRecordViewItemsForFile(int fileId)
        {
            IEnumerable<Record> records = _context.Records
                .Where(r => r.FileId == fileId)
                .OrderBy(r => r.AccountNumber);

            var viewModel = MapRecords(records);

            return viewModel;
        }


        private static IEnumerable<RecordViewItem> MapRecords(IEnumerable<Record> records)
        {
            string currentClass = records.First().ClassName;

            var subClassSum = new RecordViewItem() { Col1 = "10", ClassName = currentClass };
            var classSum = new RecordViewItem() { Col1 = "ПО КЛАССУ", ClassName = currentClass };
            var sum = new RecordViewItem() { Col1 = "БАЛАНС" };

            foreach (var record in records)
            {
                if (record.AccountNumber[1] != subClassSum.Col1[1])
                {
                    SumRecordViewModelValues(classSum, subClassSum);

                    yield return subClassSum;

                    subClassSum = new RecordViewItem() { Col1 = record.AccountNumber[..2], ClassName = record.ClassName };
                }
                if (record.ClassName != classSum.ClassName)
                {
                    SumRecordViewModelValues(sum, classSum);

                    yield return classSum;

                    classSum = new RecordViewItem() { Col1 = "ПО КЛАССУ", ClassName = record.ClassName };
                }

                var recordViewModel = MapRecord(record);

                SumRecordViewModelValues(subClassSum, recordViewModel);

                yield return recordViewModel;
            }

            yield return subClassSum;
            yield return classSum;
            yield return sum;
        }

        private static void SumRecordViewModelValues(RecordViewItem target, RecordViewItem a)
        {
            target.OpeningBalanceAsset += a.OpeningBalanceAsset;
            target.OpeningBalanceLiabilities += a.OpeningBalanceLiabilities;
            target.TurnoverDebit += a.TurnoverDebit;
            target.TurnoverCredit += a.TurnoverCredit;
            target.FinalBalanceAsset += a.FinalBalanceAsset;
            target.FinalBalanceLiabilities += a.FinalBalanceLiabilities;
        }

        private static RecordViewItem MapRecord(Record record)
        {
            var result = new RecordViewItem()
            {
                Col1 = record.AccountNumber,
                ClassName = record.ClassName,
                TurnoverCredit = record.TurnoverCredit,
                TurnoverDebit = record.TurnoverDebit,
                OpeningBalanceAsset = record.OpeningBalanceAsset,
                OpeningBalanceLiabilities = record.OpeningBalanceLiabilities,
            };

            if (result.OpeningBalanceAsset == 0 && result.OpeningBalanceLiabilities == 0)
            {
                result.FinalBalanceLiabilities = 0;
                result.FinalBalanceAsset = 0;
            }
            else if (result.OpeningBalanceLiabilities == 0)
            {
                result.FinalBalanceAsset = result.OpeningBalanceAsset + result.TurnoverDebit - result.TurnoverCredit;
                result.FinalBalanceLiabilities = 0;
            }
            else if (result.OpeningBalanceAsset == 0)
            {
                result.FinalBalanceAsset = 0;
                result.FinalBalanceLiabilities = result.OpeningBalanceLiabilities - result.TurnoverDebit + result.TurnoverCredit;
            }

            return result;
        }

        private async Task UploadToDatabaseAsync(string fileName)
        {
            var records = GetAllRecordsFromFile(fileName);

            await _context.Records.AddRangeAsync(records);

            _context.SaveChanges();
        }

        private IEnumerable<Record> GetAllRecordsFromFile(string fileName)
        {
            var file = _context.Files.FirstOrDefault(f => f.Name == fileName);

            using var excelFile = new ExcelPackage(new FileInfo(file.Path));

            var worksheet = excelFile.Workbook.Worksheets[0];

            var totalRows = worksheet.Dimension.End.Row;

            string currentClass = (string)worksheet.Cells[9, 1].Value;

            for (int i = 10; i < totalRows; i++)
            {
                var A = worksheet.Cells[i, 1].Value;
                var B = worksheet.Cells[i, 2].Value;
                var C = worksheet.Cells[i, 3].Value;
                var D = worksheet.Cells[i, 4].Value;
                var E = worksheet.Cells[i, 5].Value;

                if (B == null)
                {
                    currentClass = (string)A;
                    continue;
                }

                if (A.ToString().Length == 2)
                    continue;

                if ((string)A == "ПО КЛАССУ" || (string)A == "БАЛАНС")
                    continue;

                yield return new Record()
                {
                    ClassName = currentClass,
                    AccountNumber = (string)A,
                    OpeningBalanceAsset = (double)B,
                    OpeningBalanceLiabilities = (double)C,
                    TurnoverDebit = (double)D,
                    TurnoverCredit = (double)E,
                    FileId = file.Id,
                };
            }
            
        }

        private string SaveFile(IFormFile uploadedFile, string folderPath)
        {
            var workbook = new Workbook(uploadedFile.OpenReadStream());

            var newFileName = $"_{Guid.NewGuid()}_{uploadedFile.FileName}x";

            using var fileStream = new FileStream(folderPath + newFileName, FileMode.Create);

            workbook.Save(fileStream, SaveFormat.Xlsx);

            _context.Files.Add(new ExcelFile()
            {
                Path = fileStream.Name,
                Name = newFileName,
            });

            _context.SaveChanges();

            return newFileName;
        }
    }
}
