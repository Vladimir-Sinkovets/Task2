namespace Task2.Models
{
    public class Record
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? ClassName { get; set; }
        public double OpeningBalanceLiabilities { get; set; }
        public double OpeningBalanceAsset { get; set; }
        public double TurnoverCredit { get; set; }
        public double TurnoverDebit { get; set; }
        public int FileId { get; set; }
    }
}
