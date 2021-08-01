using CardIssuerCountry.Utils;

namespace CardIssuerCountry
{
    public class InvoiceModel
    {
        public decimal Amount { get; init; }
        public string AmountFormatted => Formatter.Amount(Amount);

        public decimal TransactionFee { get; init; }
        public string TransactionFeeFormatted => Formatter.Amount(TransactionFee);

        public decimal TotalAmount => Amount + TransactionFee;
        public string TotalAmountFormatted => Formatter.Amount(TotalAmount);

        public string Currency { get; init; } = null!;
        public string CardCountry { get; set; } = null!;
    }
}