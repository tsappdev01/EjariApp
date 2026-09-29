namespace DIP.CCPayment
{
    public class CCPaymentConstant
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string EncRequest { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
    }

    public class CCPaymentCacheDTO
    {
        public string BillReference { get; set; } = string.Empty;
        public string TxnRefNo { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
