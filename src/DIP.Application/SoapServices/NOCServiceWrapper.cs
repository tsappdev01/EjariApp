using Microsoft.Extensions.Options;
using StgDipService;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DIP.SoapServices
{
    /// <summary>
    /// Wrapper implementation for NOC SOAP Service
    /// Provides configured, testable access to StgDipService.NOCServiceClient
    /// </summary>
    public class NOCServiceWrapper : INOCServiceWrapper, ITransientDependency, IDisposable
    {
        private readonly NOCServiceClient _client;
        private readonly ClsCredentials _credentials;
        private readonly NOCServiceConfig _config;

        public NOCServiceWrapper(IOptions<SoapServicesConfiguration> soapConfig)
        {
            _config = soapConfig.Value?.NOCService
                ?? throw new ArgumentNullException(nameof(soapConfig), "NOCService configuration is missing");

            // Validate configuration
            if (string.IsNullOrWhiteSpace(_config.Url))
                throw new ArgumentException("NOCService URL is not configured", nameof(soapConfig));

            if (string.IsNullOrWhiteSpace(_config.AppKey))
                throw new ArgumentException("NOCService AppKey is not configured", nameof(soapConfig));

            // Initialize credentials
            _credentials = new ClsCredentials { AppKey = _config.AppKey };

            // Create endpoint and binding
            var endpoint = new EndpointAddress(_config.Url);
            var binding = CreateBinding();

            // Initialize client with configured endpoint
            _client = new NOCServiceClient(binding, endpoint);
        }

        /// <summary>
        /// Creates and configures the SOAP binding
        /// </summary>
        private BasicHttpBinding CreateBinding()
        {
            var binding = new BasicHttpBinding
            {
                MaxBufferSize = _config.MaxBufferSize,
                MaxReceivedMessageSize = _config.MaxReceivedMessageSize,
                SendTimeout = TimeSpan.FromSeconds(_config.TimeoutSeconds),
                ReceiveTimeout = TimeSpan.FromSeconds(_config.TimeoutSeconds),
                AllowCookies = true,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
            };

            // Enable HTTPS if URL uses https
            if (_config.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            return binding;
        }

        /// <inheritdoc/>
        public async Task<bool> ForgotPassCodeAsync(string referenceNumber, string contact)
        {
            try
            {
                var response = await _client.ForgotPassCodeAsync(referenceNumber, contact, _credentials);
                return response.ForgotPassCodeResult;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                throw new Exception($"Failed to send forgot password code: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsTradeLicenseIssuer[]> GetTradeLicenseIssuersAsync()
        {
            try
            {
                var response = await _client.GetTradeLicenseIssuersAsync(_credentials);
                return response.GetTradeLicenseIssuersResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get trade license issuers: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string[]> GetDirectionsAsync()
        {
            try
            {
                var response = await _client.GetDirectionsAsync(_credentials);
                return response.GetDirectionsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get directions: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckRegisterTypeIsNOCAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.CheckRegisterTypeIsNOCAsync(referenceNumber, _credentials);
                return response.CheckRegisterTypeIsNOCResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check register type: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ProcessingPage> CheckProcessingPageAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.CheckProcessingPageAsync(referenceNumber, _credentials);
                return response.CheckProcessingPageResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check processing page: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<EOGETCurrentLandlordSignatureDetailsResult> EOGETCurrentLandlordSignatureDetailsAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.EOGETCurrentLandlordSignatureDetailsAsync(referenceNumber, _credentials);
                return response.EOGETCurrentLandlordSignatureDetailsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get current landlord signature details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EOCheckFeedbackForRefNoAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.EOCheckFeedbackForRefNoAsync(referenceNumber, _credentials);
                return response.EOCheckFeedbackForRefNoResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check feedback: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsRegistration> GetRegistrationInfoAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.GetRegistrationInfoAsync(referenceNumber, _credentials);
                return response.GetRegistrationInfoResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get registration info: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsRegistrationDetails> GetRegistrationDetailsAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.GetRegistrationDetailsAsync(referenceNumber, _credentials);
                return response.GetRegistrationDetailsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get registration details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentHistory[]> GetPaymentHistoryAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.GetPaymentHistoryAsync(_credentials, referenceNumber);
                return response.GetPaymentHistoryResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment history: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> EOInsertFeedbackForRefNoAsync(string referenceNumber, string answer, string comments,
            string companyName, string email, string serviceName, int serviceId)
        {
            try
            {
                var response = await _client.EOInsertFeedbackForRefNoAsync(referenceNumber, answer, comments,
                    companyName, email, serviceName, serviceId, _credentials);
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert feedback: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string[]> GetBuidlingNamesByPropertyAsync(string propertyCode)
        {
            try
            {
                var response = await _client.GetBuidlingNamesByPropertyAsync(propertyCode, _credentials);
                return response.GetBuidlingNamesByPropertyResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get building names: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> GetPropertyValidationAsync(string propertyCode, string propertyValue)
        {
            try
            {
                var response = await _client.GetPropertyValidationAsync(propertyCode, propertyValue, _credentials);
                return response.GetPropertyValidationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to validate property: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckProprtyCodeHaveTenantAsync(string propertyCode)
        {
            try
            {
                var response = await _client.CheckProprtyCodeHaveTenantAsync(propertyCode, _credentials);
                return response.CheckProprtyCodeHaveTenantResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check property tenant: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> RegistrationConfirmationAsync(ClsRegistration registration)
        {
            try
            {
                var response = await _client.RegistrationConfirmationAsync(registration, _credentials);
                return response.RegistrationConfirmationResult.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to confirm registration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RegistrationVerificationAsync(string referenceNumber, string passCode)
        {
            try
            {
                var response = await _client.RegistrationVerificationAsync(referenceNumber, passCode, _credentials);
                return response.RegistrationVerificationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify registration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> LandlordUAEPassEmailVerificationAsync(string referenceNumber, string Email, string Uuid, string emiratesId)
        {
            try
            {
                var response = await _client.LandlordUAEPassEmailVerificationAsync(Email, Uuid, emiratesId, referenceNumber, _credentials);
                return response.LandlordUAEPassEmailVerificationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify registration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EORegistrationLinkVerification(string referenceNumber)
        {
            try
            {
                var response = await _client.EORegistrationLinkVerificationAsync(referenceNumber, _credentials);
                return response.EORegistrationLinkVerificationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify registration using link: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<StgDipService.Categories[]> GetCategoryTypesAsync()
        {
            try
            {
                var response = await _client.GetCategoryTypesAsync(_credentials);
                return response.GetCategoryTypesResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get category types: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsEoGetEjariRefByContractNoResDTO[]> EoGetEjariRefNoAsync(string contractNo, string emailAddress)
        {
            try
            {
                var response = await _client.EoGetEjariRefNoAsync(contractNo, emailAddress, _credentials);
                return response.EoGetEjariRefNoResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get Ejari reference: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> UpdateNocApplicationRenewalAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.UpdateNocApplicationRenewalAsync(referenceNumber, _credentials);
                return response.UpdateNocApplicationRenewalResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update renewal: {ex.Message}", ex);
            }
        }
        /// <inheritdoc/>
        public async Task<string> VerifiedDeclarationLandlordAsync(string encryptText)
        {
            try
            {
                var response = await _client.VerifiedDeclarationLandlordAsync(encryptText, _credentials);
                return response.VerifiedDeclarationLandlordResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed VerifiedDeclarationLandlordAsync: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Order[]> GetOrderDetailsAsync(string referenceNumber, string isPaid)
        {
            try
            {
                var response = await _client.GetOrderDetailsAsync(_credentials, referenceNumber, isPaid);
                return response.GetOrderDetailsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get order details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task CreateTransactionRefForOrderIdAsync(TransactionRequestInfo reqInfo)
        {
            try
            {
                await _client.CreateTransactionRefForOrderIdAsync(_credentials, reqInfo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create transaction ref: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task InsertUaePgsRequestLogAsync(RequestLog requestLog)
        {
            try
            {
                await _client.InsertUaePgsRequestLogAsync(_credentials, requestLog);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert request log: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> InsertUaePgsRequestLogWithResultAsync(RequestLog requestLog)
        {
            try
            {
                var response = await _client.InsertUaePgsRequestLogAsync(_credentials, requestLog);
                return response.InsertUaePgsRequestLogResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert request log: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<EOCheckStatusOfDocumentSigningResult> EOCheckStatusOfDocumentSigningAsync(string referenceNumber, string LandlordEmail)
        {
            try
            {
                var response = await _client.EOCheckStatusOfDocumentSigningAsync(referenceNumber, LandlordEmail, _credentials);
                return response.EOCheckStatusOfDocumentSigningResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check document signing status: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsEODocument[]> GetDocumentsListAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.GetDocumentsListAsync(referenceNumber, _credentials);
                return response.GetDocumentsListResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get documents list: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsDocumentDetailDTO> GetDocumentForRefNoNEWAsync(string referenceNumber, int documentId)
        {
            try
            {
                var response = await _client.GetDocumentForRefNoNEWAsync(referenceNumber, documentId, _credentials);
                return response.GetDocumentForRefNoNEWResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get document: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> UpdateNocApplicationStatusByPropsAsync(string referenceNumber, string statusName)
        {
            try
            {
                var response = await _client.UpdateNocApplicationStatusByPropsAsync(referenceNumber, statusName, _credentials);
                return response.UpdateNocApplicationStatusByPropsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update application status: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ApproveNocApplicationByLandLordAsync(string designation, string referenceNumber)
        {
            try
            {
                var response = await _client.ApproveNocApplicationByLandLordAsync(designation, referenceNumber, _credentials);
                return response.ApproveNocApplicationByLandLordResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to approve application: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UploadSignedDeclarationDocumentForNOCAsync(ClsEODocument document, string LandlordEmail)
        {
            try
            {
                var response = await _client.UploadSignedDeclarationDocumentForNOCAsync(document, LandlordEmail, _credentials);
                return response.UploadSignedDeclarationDocumentForNOCResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload signed declaration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetUnitsListByPropertyCodeAsync(string propertyCode, List<string> buildingNames)
        {
            try
            {
                var response = await _client.GetUnitsListByPropertyCodeAsync(propertyCode, buildingNames, _credentials);
                return response.GetUnitsListByPropertyCodeResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get units list: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> InsertRegistrationDetailsAsync(ClsRegistrationDetails details)
        {
            try
            {
                var response = await _client.InsertRegistrationDetailsAsync(details, true, _credentials);
                return response.InsertRegistrationDetailsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert registration details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> InsertRegistrationDetailsAsync(ClsRegistrationDetails details, bool isSubmit)
        {
            try
            {
                var response = await _client.InsertRegistrationDetailsAsync(details, isSubmit, _credentials);
                return response.InsertRegistrationDetailsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert registration details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetMasterDatadetailAsync(string spname)
        {
            try
            {
                var response = await _client.getMasterDatadetail(spname, _credentials);
                return response.getMasterDatadetailResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get master data: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsEOGETTenantEmail> EOGETTenantEmailAndMobileAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.EOGETTenantEmailAndMobileAsync(referenceNumber, _credentials);
                return response.EOGETTenantEmailAndMobileResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get tenant email/mobile: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsEOMinMaxAmountValueDTO> EOGETMinAndMaxAmountValueByRefno(string referenceNumber)
        {
            try
            {
                var response = await _client.EoGetminandmaxAmountValueByRefnoAsync(referenceNumber, _credentials);
                return response.EoGetminandmaxAmountValueByRefnoResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get min and Max Amount Values: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CheckDocumentBackOfficeReviewCountAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.CheckDocumentBackOfficeReviewCountAsync(referenceNumber, _credentials);
                return response.CheckDocumentBackOfficeReviewCountResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check document review count: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> SendVerificationOTPToNocApplicationAsync(string referenceNumber, string contact, int type)
        {
            try
            {
                var response = await _client.SendVerificationOTPToNocApplicationAsync(referenceNumber, contact, type, _credentials);
                return response.SendVerificationOTPToNocApplicationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send OTP: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> VerificationSubTenatOTPVerificationAsync(string referenceNumber, int otp)
        {
            try
            {
                var response = await _client.VerificationSubTenatOTPVerificationAsync(referenceNumber, otp, _credentials);
                return response.VerificationSubTenatOTPVerificationResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify OTP: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> GenerateNocPdfApplicationAuthAsync(string referenceNumber, string qrBase64)
        {
            try
            {
                var response = await _client.GenerateNocPdfApplicationAuthAsync(referenceNumber, qrBase64, _credentials);
                return response.GenerateNocPdfApplicationAuthResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate NOC PDF: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateNocApplicationToPendingAuthAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.UpdateNocApplicationToPendingAuthAsync(referenceNumber, _credentials);
                return response.UpdateNocApplicationToPendingAuthResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update application to pending auth: {ex.Message}", ex);
            }
        }

        public async Task CreateOSPaymentsOrderAsync(string referenceNumber, string paymentFor, string amount, string comments, string month, string year)
        {
            try
            {
                var response = await _client.CreateOSPaymentsOrderAsync(_credentials, referenceNumber, paymentFor, amount, comments, month, year);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create OS payment order: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> CreateOSOrderIdAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.CreateOSOrderIdAsync(_credentials, referenceNumber);
                return response.CreateOSOrderIdResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create OS order ID: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Payments[]> GetPaymentsMatrixAsync(string type)
        {
            try
            {
                var response = await _client.GetPaymentsMatrixAsync(_credentials, type);
                return response.GetPaymentsMatrixResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payments matrix: {ex.Message}", ex);
            }
        }

        public async Task<ClsKYCDocumentConfigDetails[]> GetDIModelConfigurationValueAsync()
        {
            try
            {
                var response = await _client.GetDIModelConfigurationValueAsync(_credentials);
                return response.GetDIModelConfigurationValueResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get DI model configuration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> EOInsertSubTenantKYCDetailsAsync(string referenceNumber, string documentCode, int documentId, int attempt, bool verified, string detailKeys)
        {
            try
            {
                var response = await _client.EOInsertSubTenantKYCDetailsAsync(referenceNumber, documentCode, documentId, attempt, verified, detailKeys, _credentials);
                return response.EOInsertSubTenantKYCDetailsResult ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert sub-tenant KYC details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> EOInsertTenantEmailAndMobileAsync(string referenceNumber, string verificationType, string data)
        {
            try
            {
                var response = await _client.EOInsertTenantEmailAndMobileAsync(referenceNumber, verificationType, data, _credentials);
                return response.EOInsertTenantEmailAndMobileResult ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert tenant email/mobile: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UploadDocumentForRefNoNEWAsync(ClsEODocument document)
        {
            try
            {
                var response = await _client.UploadDocumentForRefNoNEWAsync(document, _credentials);
                return response.UploadDocumentForRefNoNEWResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload document: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<OrderConfirm> CreateResponseForOrderIdAsync(TransactionResponseInfo responseInfo, string methodName)
        {
            try
            {
                var response = await _client.CreateResponseForOrderIdAsync(_credentials, responseInfo, methodName);
                return response.CreateResponseForOrderIdResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create response for order ID: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUaePgsResponseLogAsync(ResponseLog responseLog)
        {
            try
            {
                await _client.UpdateUaePgsResponseLogAsync(_credentials, responseLog);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update UAE PGS response log: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EOSendForPaymentAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.EOSendForPaymentAsync(referenceNumber, _credentials);
                return response.EOSendForPaymentResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send for payment: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClsEODocument[]> GetPaymentSlipListAsync(string referenceNumber)
        {
            try
            {
                var response = await _client.GetPaymentSlipListAsync(referenceNumber, _credentials);
                return response.GetPaymentSlipListResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment slip list: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ArrayOfKeyValueOfstringstringKeyValueOfstringstring[]> GetPaymentSlipReasonsAsync()
        {
            try
            {
                var response = await _client.GetPaymentSlipReasonsAsync(_credentials);
                return response.GetPaymentSlipReasonsResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment slip reasons: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task InsertPaymentSlipReasonsAsync(string referenceNumber, string reason)
        {
            try
            {
                await _client.InsertPaymentSlipReasonsAsync(_credentials, referenceNumber, reason);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to insert payment slip reason: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<string> CreateOrderIdAsync(string referenceNumber, string isPaid, string amount, string paymentIds)
        {
            try
            {
                var response = await _client.CreateOrderIdAsync(_credentials, referenceNumber, isPaid, amount, paymentIds);
                return response.CreateOrderIdResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create order ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Disposes the WCF client properly
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
            {
                try
                {
                    if (_client.State == CommunicationState.Opened)
                    {
                        _client.Close();
                    }
                    else if (_client.State == CommunicationState.Faulted)
                    {
                        _client.Abort();
                    }
                }
                catch
                {
                    _client?.Abort();
                }
            }
        }
    }
}
