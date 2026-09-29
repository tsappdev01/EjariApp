namespace DIP.CCPayment
{
    public class CCPaymentConfigValues
    {
        public string MerchantId { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public string WorkingKey { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string ClientReturnUrl { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

    }
}
