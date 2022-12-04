using OfficeOpenXml;
using Aspose.Cells;
using Task2.Models;
using Task2.Data;

namespace Task2.Services.XLSXFileManagers
{
    public class XLSXFileManager : IXLSXFileManager
    {
        private readonly ApplicationDbContext _context;

        public XLSXFileManager(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task SaveAndUploadToDataBaseAsync(IFormFile uploadedFile, string folderPath)
        {
            var fileName = SaveFile(uploadedFile, folderPath);

            await UploadToDatabaseAsync(fileName);
        }
        public IEnumerable<XLSXFile> GetAllFiles()
        {
            return _context.Files;
        }

        public IEnumerable<Record> GetRecordsByFile(int fileId)
        {
            return _context.Records.Where(r => r.FileId == fileId);
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

            var excelFile = new ExcelPackage(new FileInfo(file.Path));

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

            var newName = $"_{Guid.NewGuid()}_{uploadedFile.FileName}x";

            using var fileStream = new FileStream(folderPath + newName, FileMode.Create);

            workbook.Save(fileStream, SaveFormat.Xlsx);

            _context.Files.Add(new XLSXFile()
            {
                Path = fileStream.Name,
                Name = newName,
            });

            _context.SaveChanges();

            return newName;
        }
    }
}
