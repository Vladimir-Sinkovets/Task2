using Task2.Services.XLSXFileManagers.Models;
using OfficeOpenXml;
using Aspose.Cells;

namespace Task2.Services.XLSXFileManagers
{
    public class XLSXFileManager : IXLSXFileManager
    {
        public void SaveAndUploadToDataBase(IFormFile uploadedFile, string path)
        {
            var xlsxPath = SaveFile(uploadedFile, path);

            UploadToDatabase(xlsxPath);
        }

        private void UploadToDatabase(string fileName)
        {
            var excelFile = new ExcelPackage(new FileInfo(fileName));

            var worksheet = excelFile.Workbook.Worksheets[0];

            var totalRows = worksheet.Dimension.End.Row;

            var list = new List<Record>();

            string currentClass = (string) worksheet.Cells[9, 1].Value;

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


                list.Add(new Record() 
                {
                    ClassName = currentClass,
                    Account = (string) A,
                    OpeningBalanceAsset = (double) B,
                    OpeningBalanceLiabilities = (double) C,
                    TurnoverDebit = (double) D,
                    TurnoverCredit = (double) E,
                });
            }
        }

        private string SaveFile(IFormFile uploadedFile, string path)
        {
            var workbook = new Workbook(uploadedFile.OpenReadStream());

            using var fileStream = new FileStream(path + "x", FileMode.Create);

            workbook.Save(fileStream, SaveFormat.Xlsx);

            return path + "x";
        }
    }
}
