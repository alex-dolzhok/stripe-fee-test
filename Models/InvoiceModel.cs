namespace CardIssuerCountry
{
    public class InvoiceModel
    {
        public decimal Amount { get; init; }
        public decimal TransactionFee { get; init; }
        public string Currency { get; init; } = null!;
        public string CardCountry { get; set; } = null!;
    }
}