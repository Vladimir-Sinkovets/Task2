namespace Task2.Models
{
    public class RecordViewItem
    {
        public string? Col1 { get; set; }
        public string? ClassName { get; set; }
        
        public double OpeningBalanceLiabilities { get; set; }
        public double OpeningBalanceAsset { get; set; }

        public double TurnoverCredit { get; set; }
        public double TurnoverDebit { get; set; }
        
        public double FinalBalanceLiabilities { get; set; }
        public double FinalBalanceAsset { get; set; }

    }
}
