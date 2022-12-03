using Task2.Services.XLSXFileManager;
using OfficeOpenXml;

namespace Task2.Services.XLSXFileManagers
{
    public class XLSXFileManager : IXLSXFileManager
    {
        public void UploadToDatabase(string fileName)
        {
            ExcelPackage excelFile = new ExcelPackage(new FileInfo(fileName));

            ExcelWorksheet worksheet = excelFile.Workbook.Worksheets[0];

            var totalRows = worksheet.Dimension.End.Row;
            var totalColumns = worksheet.Dimension.End.Column;

            List<Record> list = new List<Record>();

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
    }

    internal class Record
    {
        public string Account { get; set; }
        public string ClassName { get; set; }
        public double OpeningBalanceLiabilities { get; set; }
        public double OpeningBalanceAsset { get; set; }
        public double TurnoverCredit { get; set; }
        public double TurnoverDebit { get; set; }
    }
}
