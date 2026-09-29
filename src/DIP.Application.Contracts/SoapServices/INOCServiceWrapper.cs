using StgDipService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIP.SoapServices
{
    /// <summary>
    /// Wrapper interface for NOC SOAP Service operations
    /// Provides abstraction over StgDipService.NOCServiceClient
    /// </summary>
    public interface INOCServiceWrapper
    {
        /// <summary>
        /// Sends forgot password code to user's registered contact (mobile or email)
        /// </summary>
        /// <param name="referenceNumber">User's reference number</param>
        /// <param name="contact">Mobile number (with +971 prefix) or email address</param>
        /// <returns>True if password code was sent successfully, false otherwise</returns>
        Task<bool> ForgotPassCodeAsync(string referenceNumber, string contact);
        /// <summary>
        /// Gets trade license issuers list
        /// </summary>
        /// <returns>Array of trade license issuers</returns>
        Task<ClsTradeLicenseIssuer[]> GetTradeLicenseIssuersAsync();

        /// <summary>
        /// Gets available directions
        /// </summary>
        /// <returns>Array of direction strings</returns>
        Task<string[]> GetDirectionsAsync();

        /// <summary>
        /// Checks if registration type is NOC
        /// </summary>
        Task<bool> CheckRegisterTypeIsNOCAsync(string referenceNumber);

        /// <summary>
        /// Checks processing page status
        /// </summary>
        Task<ProcessingPage> CheckProcessingPageAsync(string referenceNumber);
        /// <summary>
        /// Checks processing page status
        /// </summary>
        Task<EOGETCurrentLandlordSignatureDetailsResult> EOGETCurrentLandlordSignatureDetailsAsync(string referenceNumber);

        /// <summary>
        /// Checks if feedback is required for reference number
        /// </summary>
        Task<bool> EOCheckFeedbackForRefNoAsync(string referenceNumber);

        /// <summary>
        /// Gets registration information
        /// </summary>
        Task<ClsRegistration> GetRegistrationInfoAsync(string referenceNumber);

        /// <summary>
        /// Gets registration details
        /// </summary>
        Task<ClsRegistrationDetails> GetRegistrationDetailsAsync(string referenceNumber);

        /// <summary>
        /// Gets payment history
        /// </summary>
        Task<PaymentHistory[]> GetPaymentHistoryAsync(string referenceNumber);

        /// <summary>
        /// Inserts feedback for reference number
        /// </summary>
        Task<string> EOInsertFeedbackForRefNoAsync(string referenceNumber, string answer, string comments,
            string companyName, string email, string serviceName, int serviceId);

        /// <summary>
        /// Gets building names by property
        /// </summary>
        Task<string[]> GetBuidlingNamesByPropertyAsync(string propertyCode);

        /// <summary>
        /// Validates property code and value
        /// </summary>
        Task<bool> GetPropertyValidationAsync(string propertyCode, string propertyValue);

        /// <summary>
        /// Checks if property code has tenant
        /// </summary>
        Task<bool> CheckProprtyCodeHaveTenantAsync(string propertyCode);

        /// <summary>
        /// Confirms registration
        /// </summary>
        Task<string> RegistrationConfirmationAsync(ClsRegistration registration);

        /// <summary>
        /// Verifies registration
        /// </summary>
        Task<bool> RegistrationVerificationAsync(string referenceNumber, string passCode);

        /// <summary>
        /// Verifies registration using link 
        /// </summary>
        Task<bool> EORegistrationLinkVerification(string referenceNumber);

        /// <summary>
        /// Gets category types
        /// </summary>
        Task<StgDipService.Categories[]> GetCategoryTypesAsync();

        /// <summary>
        /// Gets Ejari reference number by contract and email
        /// </summary>
        Task<ClsEoGetEjariRefByContractNoResDTO[]> EoGetEjariRefNoAsync(string contractNo, string emailAddress);

        /// <summary>
        /// Updates NOC application renewal
        /// </summary>
        Task<string> UpdateNocApplicationRenewalAsync(string referenceNumber);

        /// <summary>
        /// LandlordUAEPassEmailVerificationAsync
        /// </summary>
        Task<bool> LandlordUAEPassEmailVerificationAsync(string referenceNumber, string Email, string Uuid, string emiratesId);

        /// <summary>
        /// VerifiedDeclarationLandlordAsync
        /// </summary>
        Task<string> VerifiedDeclarationLandlordAsync(string encryptText);

        /// <summary>
        /// Gets order details
        /// </summary>
        Task<Order[]> GetOrderDetailsAsync(string referenceNumber, string isPaid);

        /// <summary>
        /// Creates transaction reference for order ID
        /// </summary>
        Task CreateTransactionRefForOrderIdAsync(TransactionRequestInfo reqInfo);

        /// <summary>
        /// Inserts UAE PGS request log
        /// </summary>
        Task InsertUaePgsRequestLogAsync(RequestLog requestLog);

        /// <summary>
        /// Inserts UAE PGS request log and returns the log ID
        /// </summary>
        Task<int> InsertUaePgsRequestLogWithResultAsync(RequestLog requestLog);

        /// <summary>
        /// Checks the status of document signing
        /// </summary>
        Task<EOCheckStatusOfDocumentSigningResult> EOCheckStatusOfDocumentSigningAsync(string referenceNumber, string LandlordEmail);

        /// <summary>
        /// Gets list of documents for reference number
        /// </summary>
        Task<ClsEODocument[]> GetDocumentsListAsync(string referenceNumber);

        /// <summary>
        /// Gets document content for reference number
        /// </summary>
        Task<ClsDocumentDetailDTO> GetDocumentForRefNoNEWAsync(string referenceNumber, int documentId);

        /// <summary>
        /// Updates NOC application status
        /// </summary>
        Task<int> UpdateNocApplicationStatusByPropsAsync(string referenceNumber, string statusName);

        /// <summary>
        /// Approves NOC application by landlord
        /// </summary>
        Task<bool> ApproveNocApplicationByLandLordAsync(string designation, string referenceNumber);

        /// <summary>
        /// Uploads signed declaration document for NOC
        /// </summary>
        Task<bool> UploadSignedDeclarationDocumentForNOCAsync(ClsEODocument document, string LandlordEmail);

        // ── NocBasicForm ──────────────────────────────────────────────
        /// <summary>Gets unit list by property code (JSON string)</summary>
        Task<string> GetUnitsListByPropertyCodeAsync(string propertyCode, List<string> buildingNames);

        /// <summary>Inserts registration details</summary>
        Task<bool> InsertRegistrationDetailsAsync(ClsRegistrationDetails details);

        /// <summary>Inserts registration details with explicit isSubmit flag</summary>
        Task<bool> InsertRegistrationDetailsAsync(ClsRegistrationDetails details, bool isSubmit);

        /// <summary>Gets master data details by stored-procedure name</summary>
        Task<string> GetMasterDatadetailAsync(string spname);

        // ── NocAcknowledgement ────────────────────────────────────────
        /// <summary>Gets tenant email and mobile</summary>
        Task<ClsEOGETTenantEmail> EOGETTenantEmailAndMobileAsync(string referenceNumber);

        /// <summary>Checks document back office review count</summary>
        Task<bool> CheckDocumentBackOfficeReviewCountAsync(string referenceNumber);

        /// <summary>Sends OTP to NOC application for verification (type: 1=Mobile, 2=Email)</summary>
        Task<int> SendVerificationOTPToNocApplicationAsync(string referenceNumber, string contact, int type);

        /// <summary>Verifies sub-tenant OTP</summary>
        Task<int> VerificationSubTenatOTPVerificationAsync(string referenceNumber, int otp);

        /// <summary>Generates the NOC PDF application with auth</summary>
        Task<bool> GenerateNocPdfApplicationAuthAsync(string referenceNumber, string qrBase64);

        /// <summary>Updates NOC application to PendingAuth status</summary>
        Task<bool> UpdateNocApplicationToPendingAuthAsync(string referenceNumber);

        // ── PaymentServices ───────────────────────────────────────────
        /// <summary>Creates OS payment order</summary>
        Task CreateOSPaymentsOrderAsync(string referenceNumber, string paymentFor, string amount, string comments, string month, string year);

        /// <summary>Creates OS order ID</summary>
        Task<string> CreateOSOrderIdAsync(string referenceNumber);

        /// <summary>Gets payments matrix</summary>
        Task<Payments[]> GetPaymentsMatrixAsync(string type);

        // ── NocDocuments ──────────────────────────────────────────────
        /// <summary>Gets DI model configuration values</summary>
        Task<ClsKYCDocumentConfigDetails[]> GetDIModelConfigurationValueAsync();

        /// <summary>Inserts sub-tenant KYC details</summary>
        Task<int> EOInsertSubTenantKYCDetailsAsync(string referenceNumber, string documentCode, int documentId, int attempt, bool verified, string detailKeys);

        /// <summary>Inserts tenant email and mobile</summary>
        Task<int> EOInsertTenantEmailAndMobileAsync(string referenceNumber, string verificationType, string data);

        /// <summary>Uploads document for reference number</summary>
        Task<bool> UploadDocumentForRefNoNEWAsync(ClsEODocument document);

        /// <summary>Gets minimum and maximum amount values by reference number</summary>
        Task<ClsEOMinMaxAmountValueDTO> EOGETMinAndMaxAmountValueByRefno(string referenceNumber);

        // ── PGResponse / PaymentResult ────────────────────────────────
        /// <summary>Creates response for order ID and returns confirmation</summary>
        Task<OrderConfirm> CreateResponseForOrderIdAsync(TransactionResponseInfo responseInfo, string methodName);

        /// <summary>Updates UAE PGS response log</summary>
        Task UpdateUaePgsResponseLogAsync(ResponseLog responseLog);

        // ── EjariUploads / EjariUploadPayslip ─────────────────────────
        /// <summary>Sends the application for payment</summary>
        Task<bool> EOSendForPaymentAsync(string referenceNumber);

        /// <summary>Gets payment slip document list for a reference number</summary>
        Task<ClsEODocument[]> GetPaymentSlipListAsync(string referenceNumber);

        // ── PaySlipReason ─────────────────────────────────────────────
        /// <summary>Gets payment slip upload reasons</summary>
        Task<ArrayOfKeyValueOfstringstringKeyValueOfstringstring[]> GetPaymentSlipReasonsAsync();

        /// <summary>Inserts the chosen payment slip upload reason</summary>
        Task InsertPaymentSlipReasonsAsync(string referenceNumber, string reason);

        // ── EjariPayment ──────────────────────────────────────────────
        /// <summary>Creates an order ID for the given reference number and payment details</summary>
        Task<string> CreateOrderIdAsync(string referenceNumber, string isPaid, string amount, string paymentIds);
    }
}

