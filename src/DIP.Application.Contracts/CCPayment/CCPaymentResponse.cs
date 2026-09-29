using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.CCPayment
{
    /// <summary>
    /// Payment response from CCAvenue containing essential transaction details
    /// </summary>
    public class CCPaymentResponse
    {
        private const string STATUS_SUCCESS = "Success";
        private const string STATUS_FAILURE = "Failure";
        private const string STATUS_ABORTED = "Aborted";

        public string OrderId { get; set; } = string.Empty;
        public string TrackingId { get; set; } = string.Empty;
        public string BankRefNo { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;

        public string FailureMessage { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public string CardName { get; set; } = string.Empty;

        public string BillingName { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingState { get; set; } = string.Empty;
        public string BillingZip { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = string.Empty;
        public string BillingTel { get; set; } = string.Empty;
        public string BillingEmail { get; set; } = string.Empty;

        public string DeliveryName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryState { get; set; } = string.Empty;
        public string DeliveryZip { get; set; } = string.Empty;
        public string DeliveryCountry { get; set; } = string.Empty;
        public string DeliveryTel { get; set; } = string.Empty;

        public string MerchantParam1 { get; set; } = string.Empty;
        public string MerchantParam2 { get; set; } = string.Empty;
        public string MerchantParam3 { get; set; } = string.Empty;
        public string MerchantParam4 { get; set; } = string.Empty;
        public string MerchantParam5 { get; set; } = string.Empty;
        public string MerchantParam6 { get; set; } = string.Empty;

        public string Vault { get; set; } = string.Empty;
        public string OfferType { get; set; } = string.Empty;
        public string OfferCode { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }  
        public decimal MerAmount { get; set; }

        public string EciValue { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string BankQsiNo { get; set; } = string.Empty;

        public string SiCreated { get; set; } = string.Empty;
        public string SiRefNo { get; set; } = string.Empty;
        public string SiStatus { get; set; } = string.Empty;
        public string SiMerRefNo { get; set; } = string.Empty;

        public string InvMerReferenceNo { get; set; } = string.Empty;
        public string BankReceiptNo { get; set; } = string.Empty;

        public string VisaPlanAcceptanceId { get; set; } = string.Empty;
        public string VisaPlanId { get; set; } = string.Empty;
        public string VisaEppAmt { get; set; } = string.Empty;
        public string VisaEppTenure { get; set; } = string.Empty;
        public string VisaEppFrequency { get; set; } = string.Empty;
        public string VisaEppRate { get; set; } = string.Empty;
        public string VisaEppFees { get; set; } = string.Empty;
        public string VisaEppTerms { get; set; } = string.Empty;

        public string CustomerCardId { get; set; } = string.Empty;
        public string AcquirerMessage { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;

        public string IsMcpTxn { get; set; } = string.Empty;
        public string McpAmount { get; set; } = string.Empty;
        public string McpCurrency { get; set; } = string.Empty;
        public string McpConversionRate { get; set; } = string.Empty;

        public string BillingNotes { get; set; } = string.Empty;

        #region cache Data's
        public string BillReference { get; set; } = string.Empty;
        public string TxnRefNo { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// Check if payment was successful
        /// </summary>
        public bool IsSuccess => OrderStatus?.Equals(STATUS_SUCCESS, StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Check if payment failed
        /// </summary>
        public bool IsFailure => OrderStatus?.Equals(STATUS_FAILURE, StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Check if payment was aborted by user
        /// </summary>
        public bool IsAborted => OrderStatus?.Equals(STATUS_ABORTED, StringComparison.OrdinalIgnoreCase) == true;
    }
}
