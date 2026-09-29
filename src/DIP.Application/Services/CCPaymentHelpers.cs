
using CCA.Util;
using DIP.CCPayment;
using DIP.Interface;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace DIP.Services
{
    public class CCPaymentHelpers : ICCPaymentHelpers
    {
        private readonly IDIPAppsettingService IdIPAppsettingService;
        private CCPaymentConfigValues configValues;
        public CCPaymentHelpers(IDIPAppsettingService idIPAppsettingService)
        {
            IdIPAppsettingService = idIPAppsettingService;
        }


        public async Task<CCPaymentConstant> BuildCcPaymentFormDataAsync(PaymentInitiationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            configValues = await IdIPAppsettingService.GetCardPaymentConfigurations();

            string EncRequest = await BuildEncryptedRequestAsync(request);

            return new CCPaymentConstant
            {
                PaymentUrl = configValues.PaymentUrl,
                AccessCode = configValues.AccessCode,
                EncRequest = EncRequest
            };
        }


        public async Task<CCPaymentResponse> ParseEncryptedResponseAsync(string encryptedResponse)
        {
            if (string.IsNullOrWhiteSpace(encryptedResponse))
                throw new ArgumentException("Encrypted response cannot be null or empty", nameof(encryptedResponse));

            configValues = await IdIPAppsettingService.GetCardPaymentConfigurations();

            var decryptedResponse = Decrypt(encryptedResponse, configValues.WorkingKey);

            //_logger.LogDebug("Decrypted CCAvenue response");

            return ParseResponse(decryptedResponse);
        }

        private async Task<string> BuildEncryptedRequestAsync(PaymentInitiationRequest request)
        {
            var requestString = await BuildRequestString(request);

            var encrypted = Encrypt(requestString, configValues.WorkingKey);

            if (string.IsNullOrEmpty(encrypted))
                throw new InvalidOperationException("CardPayment Encryption failed - returned null or empty string");

            return encrypted;
        }


        private Task<string> BuildRequestString(PaymentInitiationRequest request)
        {
            var builder = new StringBuilder();
            AppendParameter(builder, "merchant_id", configValues.MerchantId);
            AppendParameter(builder, "order_id", request.OrderId);
            AppendParameter(builder, "currency", request.Currency ?? configValues.Currency);
            AppendParameter(builder, "amount", request.Amount.ToString("F2"));
            AppendParameter(builder, "redirect_url", configValues.RedirectUrl);
            AppendParameter(builder, "language", configValues.Language);

            AppendParameterIfNotEmpty(builder, "billing_name", request.BillingName);
            AppendParameterIfNotEmpty(builder, "billing_email", request.BillingEmail);
            AppendParameterIfNotEmpty(builder, "billing_tel", request.BillingTel);
            AppendParameterIfNotEmpty(builder, "billing_notes", request.BillingNotes);

            return Task.FromResult(builder.ToString());
        }


        private static void AppendParameter(StringBuilder builder, string key, string value)
        {
            builder.Append(key).Append('=').Append(value).Append('&');
        }

        private static void AppendParameterIfNotEmpty(StringBuilder builder, string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                AppendParameter(builder, key, value);
            }
        }

        private CCPaymentResponse ParseResponse(string decryptedResponse)
        {
            var responseParams = HttpUtility.ParseQueryString(decryptedResponse);

            return new CCPaymentResponse
            {
                OrderId = GetParameter(responseParams, "order_id"),
                TrackingId = GetParameter(responseParams, "tracking_id"),
                BankRefNo = GetParameter(responseParams, "bank_ref_no"),
                OrderStatus = GetParameter(responseParams, "order_status"),
                FailureMessage = GetParameter(responseParams, "failure_message"),
                PaymentMode = GetParameter(responseParams, "payment_mode"),
                StatusCode = GetParameter(responseParams, "status_code"),
                StatusMessage = GetParameter(responseParams, "status_message"),
                Currency = GetParameter(responseParams, "currency"),
                Amount = GetDecimalParameter(responseParams, "amount"),
                CardName = GetParameter(responseParams, "card_name"),
                BillingName = GetParameter(responseParams, "billing_name"),
                BillingAddress = GetParameter(responseParams, "billing_address"),
                BillingCity = GetParameter(responseParams, "billing_city"),
                BillingState = GetParameter(responseParams, "billing_state"),
                BillingZip = GetParameter(responseParams, "billing_zip"),
                BillingCountry = GetParameter(responseParams, "billing_country"),
                BillingTel = GetParameter(responseParams, "billing_tel"),
                BillingEmail = GetParameter(responseParams, "billing_email"),
                DeliveryName = GetParameter(responseParams, "delivery_name"),
                DeliveryAddress = GetParameter(responseParams, "delivery_address"),
                DeliveryCity = GetParameter(responseParams, "delivery_city"),
                DeliveryState = GetParameter(responseParams, "delivery_state"),
                DeliveryZip = GetParameter(responseParams, "delivery_zip"),
                DeliveryCountry = GetParameter(responseParams, "delivery_country"),
                DeliveryTel = GetParameter(responseParams, "delivery_tel"),
                MerchantParam1 = GetParameter(responseParams, "merchant_param1"),
                MerchantParam2 = GetParameter(responseParams, "merchant_param2"),
                MerchantParam3 = GetParameter(responseParams, "merchant_param3"),
                MerchantParam4 = GetParameter(responseParams, "merchant_param4"),
                MerchantParam5 = GetParameter(responseParams, "merchant_param5"),
                MerchantParam6 = GetParameter(responseParams, "merchant_param6"),
                Vault = GetParameter(responseParams, "vault"),
                OfferType = GetParameter(responseParams, "offer_type"),
                OfferCode = GetParameter(responseParams, "offer_code"),
                DiscountValue = GetDecimalParameter(responseParams, "discount_value"),
                MerAmount = GetDecimalParameter(responseParams, "mer_amount"),
                EciValue = GetParameter(responseParams, "eci_value"),
                CardHolderName = GetParameter(responseParams, "card_holder_name"),
                BankQsiNo = GetParameter(responseParams, "bank_qsi_no"),
                SiCreated = GetParameter(responseParams, "si_created"),
                SiRefNo = GetParameter(responseParams, "si_ref_no"),
                SiStatus = GetParameter(responseParams, "si_status"),
                SiMerRefNo = GetParameter(responseParams, "si_mer_ref_no"),
                InvMerReferenceNo = GetParameter(responseParams, "inv_mer_reference_no"),
                BankReceiptNo = GetParameter(responseParams, "bank_receipt_no"),
                VisaPlanAcceptanceId = GetParameter(responseParams, "visaPlanAcceptanceId"),
                VisaPlanId = GetParameter(responseParams, "visaPlanId"),
                VisaEppAmt = GetParameter(responseParams, "visaEppAmt"),
                VisaEppTenure = GetParameter(responseParams, "visaEppTenure"),
                VisaEppFrequency = GetParameter(responseParams, "visaEppFrequency"),
                VisaEppRate = GetParameter(responseParams, "visaEppRate"),
                VisaEppFees = GetParameter(responseParams, "visaEppFees"),
                VisaEppTerms = GetParameter(responseParams, "visaEppTerms"),
                CustomerCardId = GetParameter(responseParams, "customer_card_id"),
                AcquirerMessage = GetParameter(responseParams, "acquirer_message"),
                Issuer = GetParameter(responseParams, "issuer"),
                IsMcpTxn = GetParameter(responseParams, "is_mcp_txn"),
                McpAmount = GetParameter(responseParams, "mcp_amount"),
                McpCurrency = GetParameter(responseParams, "mcp_currency"),
                McpConversionRate = GetParameter(responseParams, "mcp_conversion_rate"),
                BillingNotes = GetParameter(responseParams, "billing_notes")
            };
        }

        private static string GetParameter(System.Collections.Specialized.NameValueCollection collection, string key)
        {
            return collection[key] ?? string.Empty;
        }

        private static decimal GetDecimalParameter(System.Collections.Specialized.NameValueCollection collection, string key)
        {
            var value = collection[key];
            return decimal.TryParse(value, out var result) ? result : 0m;
        }

        #region CCAvenueCrypto
        private static string Encrypt(string plainText, string workingKey)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));

            if (string.IsNullOrWhiteSpace(workingKey))
                throw new ArgumentException("Working key cannot be null or empty", nameof(workingKey));

            var ccaCrypto = new CCACrypto();
            return ccaCrypto.Encrypt(plainText, workingKey);
        }

        private static string Decrypt(string encryptedText, string workingKey)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));

            if (string.IsNullOrWhiteSpace(workingKey))
                throw new ArgumentException("Working key cannot be null or empty", nameof(workingKey));

            var ccaCrypto = new CCACrypto();
            return ccaCrypto.Decrypt(encryptedText, workingKey);
        }
        #endregion
    }
}
