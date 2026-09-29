using Blazorise;
using DIP.Connected_Services.DocumentIntelligent;
using DIP.DIModels;
using DIP.EServices;
using DIP.PageInfos;
using DIP.QrService;
using DIP.SoapServices;
using DIP.UploadEjariFiles;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using StgDipService;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;


namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocDocuments
    {

        [Parameter]
        public string Lang { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }


        private string encryptedReferenceNumber;

        private bool _initialized;

        #region StepProgress
        private int currentStep = 2;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion

        [Inject]
        public IQrServiceClient IQrServiceClient { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IDocumentIntelligentClient _documentIntelligentClient { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }
        public ClsKYCDocumentConfigDetails[] DiConfigValues { get; set; } = [];
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        [Inject]
        public IBlobContainer<UploadEjariFilesContainer> EFormServiceSubCategoryContainer { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; } = null!;
        public string StatusName { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public List<ClsEODocument> DocumentsGrid { get; private set; }
        public string ErrorMessageUpload { get; set; }
        private int indexError;
        public string ErrorMessageMissingDocuments { get; set; }
        public bool ButtonDisable { get; set; } = false;
        public List<InvalidDocumentDTO> InvalidDocumentDTO { get; set; } = [];
        public TenancyContractValidation tenancyContractValidation { get; set; } = new();
        public bool documentIntelligentVerification { get; set; } = false;
        public int totalCountdocumentVerification { get; set; }
        public int progresseddocumentVerification { get; set; }
        public int InvalidAttempt { get; set; } = 0;
        public int ProgressValue { get; set; } = 0;
        public bool modelclose { get; set; } = true;

        public List<string> prebuildDocumentCode { get; set; } = [];

        public Modal modalRef;

        public NocDocuments()
        {
        }

        protected override async Task OnInitializedAsync()
        {

            if (_initialized) return;
            _initialized = true;
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);
            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            GetEServicesInput getEServicesInput = new()
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }
            await PrepareData();
        }

        private async Task PrepareData()
        {
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            StatusName = status.StatusName;
            DiConfigValues = await NOCService.GetDIModelConfigurationValueAsync();
            prebuildDocumentCode = DiConfigValues.Select(x => new { x.DocumentCode, x.ModelId })
                                              .ToList()
                                              .Where(x => x.ModelId.Contains("prebuilt-idDocument"))
                                              .Select(x => x.DocumentCode)
                                              .Distinct()
                                              .ToList();
            await RedirectionToCorrespondingPage();
            await PopulateDocumentsList();
        }

        private async Task RedirectionToCorrespondingPage()
        {
            if (!StatusName.Equals("Upload"))
            {
                switch (StatusName)
                {
                    case "Registered":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "Verified":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocBasicForm/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "Upload":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "ApplicantAcknowledgement":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocAcknowledgement/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "PendingAuth":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        break;
                    case "Payment":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                        break;

                    case "PaymentProcess":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                        break;

                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        break;
                }
            }
        }
        private async Task PopulateDocumentsList()
        {
            var documentsGridResult = await NOCService.GetDocumentsListAsync(ReferenceNumber);
            DocumentsGrid = [.. documentsGridResult.Where(d => d.DocumentCode != "DMSEJAPP").OrderByDescending(x => x.IsMandatory == true).DistinctBy(x => x.Id)];
            InvalidAttempt = DocumentsGrid.MaxBy(x => x.ZAttemptCount)?.ZAttemptCount ?? 0;
            int maxnumber = DocumentsGrid.MaxBy(x => x.ZAttemptCount)?.ZAttemptCount ?? 0;
            if (maxnumber > 0)
                DocumentsGrid = [.. DocumentsGrid.OrderByDescending(x => x.ZSystemVerified).ThenByDescending(x => x.ZAttemptCount == InvalidAttempt)];
            if (DocumentsGrid.Where(x => x.DocumentCode == "DMSTNCONT" && x.ZSystemVerified).Any())
            {
                tenancyContractValidation.TenantEmailStatus = 2;
                tenancyContractValidation.TenantPhoneStatus = 2;
            }
            if (InvalidDocumentDTO.Count > 0)
            {
                foreach (InvalidDocumentDTO docError in InvalidDocumentDTO)
                {
                    int index = DocumentsGrid.FindIndex(x => x.DocumentCode == docError.DocumnetCode);
                    if (index >= 0)
                    {
                        DocumentsGrid[index].ErrorMessageUpload = docError.DocumentError;
                    }
                }
            }

            foreach (ClsEODocument document in DocumentsGrid)
            {
                DocumentsListDocumentStatusType(document);
            }
        }

        private void DocumentsListDocumentStatusType(ClsEODocument document)
        {
            int index = DocumentsGrid.FindIndex(x => x.DocumentCode == document.DocumentCode);
            if (!(index >= 0))
            {
                return;
            }
            #region always first 
            if (document.Uploading)
            {
                DocumentsGrid[index].DocumentStatusType = 8;
                return;
            }
            #endregion

            #region condition
            //dont chnage the order of condition please understand code first,
            //if you change condition without understanding the code it will be crash the system. 
            int[] tenancyContractValidationArr = new[] { 0, 1 };
            if (InvalidAttempt > 2 &&
               (tenancyContractValidationArr.Contains(tenancyContractValidation.TenantEmailStatus) || tenancyContractValidationArr.Contains(tenancyContractValidation.TenantPhoneStatus))
               && document.attempt == false && !document.ZSystemVerified)
            {
                if (document.DocumentCode == "DMSTNCONT" && !document.ZSystemVerified)
                {
                    DocumentsGrid[index].DocumentStatusType = 5; // Attempt failed Tc
                    return;
                }
                else if (DiConfigValues.Select(x => x.DocumentCode).Distinct().ToList().Contains(document.DocumentCode) && !document.ZSystemVerified
                    && (document.IsUploaded.HasValue && document.IsUploaded.Value))
                {
                    DocumentsGrid[index].DocumentStatusType = 4; // Attempt Info
                    return;
                }
            }

            if (InvalidAttempt > 2 && document.DocumentCode != "DMSTNCONT" && !document.ZSystemVerified
                && DiConfigValues.Select(x => x.DocumentCode).Distinct().ToList().Contains(document.DocumentCode) && !document.ZSystemVerified
                && (document.IsUploaded.HasValue && document.IsUploaded.Value))
            {
                DocumentsGrid[index].DocumentStatusType = 4; // Attempt Info
                return;
            }

            if (!string.IsNullOrEmpty(document.ErrorMessageUpload))
            {
                DocumentsGrid[index].DocumentStatusType = 3; //ErrorMessageUpload
                return;
            }

            if (document.ZAttemptCount > 0 && !document.attempt && !document.ZSystemVerified)
            {
                DocumentsGrid[index].DocumentStatusType = 2; // diverficationfailed
                return;
            }

            if (document.ZSystemVerified)
            {
                DocumentsGrid[index].DocumentStatusType = 7;
                return;
            }

            if (document.IsUploaded ?? false)
            {
                DocumentsGrid[index].DocumentStatusType = 1; //Uploaded
                return;
            }
            #endregion 
        }

        public object ValidateMandatoryDocuments()
        {
            if (DocumentsGrid == null || DocumentsGrid.Count == 0)
                return "No documents found";
            var notUploaded = DocumentsGrid
                .Where(d => (d.IsMandatory ?? true) &&
                (InvalidAttempt > 0 ? true : !(d.IsUploaded ?? false))
                && (InvalidAttempt > 0 ?
                        DiConfigValues.Select(x => x.DocumentCode).Distinct().ToList().Contains(d.DocumentCode)
                        && d.ZSystemVerified == false && d.attempt == false
                    : true
                )
                &&
               (((tenancyContractValidation.TenantEmailStatus != 2 || tenancyContractValidation.TenantPhoneStatus != 2) && d.ZAttemptCount > 2) ?
                d.attempt == false && d.DocumentCode == "DMSTNCONT" && d.ZSystemVerified == false : d.ZAttemptCount < 3))
                //.Select(d => d.DocName)
                .ToList();
            foreach (ClsEODocument doc in notUploaded)
            {
                int index = DocumentsGrid.FindIndex(x => x.DocumentCode == doc.DocumentCode);
                if (index >= 0)
                {
                    if (doc.ZAttemptCount > 0)
                    {

                    }
                    DocumentsGrid[index].ErrorMessageUpload = $"{DocumentsGrid[index].DocName} {(doc.ZAttemptCount > 0 ? "failed verification and must be replaced before you can continue." : "is a required document.Please upload.")}";
                    DocumentsGrid[index].attempt = false;
                    DocumentsListDocumentStatusType(doc);
                }
            }

            if (notUploaded.Count == 0)
                return true;

            //return string.Join(", ", notUploaded);
            return "hhhhh";
        }

        #region Submit Methods 

        public async Task handleBackAction()
        {
            int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "Verified");
            if (updateApplicationStatus == 1)
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocBasicForm/{encryptedReferenceNumber}");
            }
            return;
        }

        public async Task OnProcced()
        {
            ButtonDisable = true;
            ErrorMessageMissingDocuments = string.Empty;
            ProgressValue = 0;
            await InvokeAsync(StateHasChanged);
            object submit = ValidateMandatoryDocuments();
            bool _diValidationResponse = false;

            if (submit is bool V && V == true)
            {
                _diValidationResponse = await DocumentIntelligentValidationAsync();
            }
            else if (submit is string S && !string.IsNullOrWhiteSpace(S))
            {
                ButtonDisable = false;
                //ErrorMessageMissingDocuments = (string)submit;
                return;
            }

            if ((_diValidationResponse || InvalidAttempt >= 2) && (tenancyContractValidation.TenantEmailStatus == 2 || tenancyContractValidation.TenantPhoneStatus == 2))
            {
                int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "ApplicantAcknowledgement");
                if (updateApplicationStatus == 1)
                {
                    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocAcknowledgement/{encryptedReferenceNumber}");
                }
            }
            else
            {
                await PopulateDocumentsList();
                DocumentsGrid = [.. DocumentsGrid.OrderByDescending(x => x.ZSystemVerified && x.ZAttemptCount > 0)];
                modelclose = false;
                await modalRef.Hide();
                ButtonDisable = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        #endregion


        private async Task<bool> DocumentIntelligentValidationAsync()
        {
            InvalidDocumentDTO = [];
            await modalRef.Show();
            var documentExpiryOffsetDays = Configuration.GetValue("App:DocumentExpiryValidationOffsetDays", 0);
            bool _dateExtractionFromDiForTC = false;
            var documentExpiryCompareBaseline = DateTime.Now;
            List<string> _validationDocumnetCode = [.. DiConfigValues.DistinctBy(x => x.DocumentCode).Select(x => x.DocumentCode)];
            if (InvalidAttempt > 2)
            {
                _validationDocumnetCode = [.. _validationDocumnetCode.Where(x => x == "DMSTNCONT")];
            }
            List<ClsEODocument> _validationDocuments = [.. DocumentsGrid.Where(x => _validationDocumnetCode.Contains(x.DocumentCode) && x.ZSystemVerified == false
                                                                                && x.ZReviewRequired == false
                                                                                && x.IsMandatory == true
                                                                                )];
            totalCountdocumentVerification = _validationDocuments.Count();
            progresseddocumentVerification = 0;
            await InvokeAsync(StateHasChanged);
            foreach (ClsEODocument documentItem in _validationDocuments)
            {
                if (documentItem.DocumentCode == "DMSTNCONT")
                {
                    documentExpiryCompareBaseline = DateTime.Now.AddDays(documentExpiryOffsetDays);
                }
                else
                {
                    documentExpiryCompareBaseline = DateTime.Now;
                }
                progresseddocumentVerification++;
                await InvokeAsync(StateHasChanged);
                if (!string.IsNullOrEmpty(documentItem.DMSDocument))
                {
                    List<KYCObjectDTO> kycitems = [];
                    List<InvalidDocumentFieldDTO> fieldsError = [];
                    bool documentvalid = false;
                    byte[] documentBytes = [];
                    if (documentItem.Id > 0)
                    {
                        var documenResponse = await NOCService.GetDocumentForRefNoNEWAsync(ReferenceNumber, documentItem.Id);
                        documentBytes = Convert.FromBase64String(documenResponse?.DMSDocument ?? string.Empty);
                    }
                    else
                    {
                        documentBytes = Convert.FromBase64String(documentItem.DMSDocument);
                    }
                    string _documentModel = DiConfigValues.Where(x => x.DocumentCode == documentItem.DocumentCode).Select(x => x.ModelId).First();
                    string _documentModelId = string.Empty;
                    string _documentModeldocType = string.Empty;
                    bool __documentModeldocTypeCheck = false;
                    if (_documentModel.Contains('|'))
                    {
                        try
                        {
                            __documentModeldocTypeCheck = true;
                            List<string> _documentSplit = [.. _documentModel.Split('|')];
                            _documentModelId = _documentSplit[0];
                            _documentModeldocType = _documentSplit[1];
                        }
                        catch (Exception)
                        {
                            InvalidDocumentDTO.Add(new InvalidDocumentDTO
                            {
                                DocumnetCode = documentItem.DocumentCode,
                                DocuemntId = documentItem.Id,
                                Attempt = 1,
                                fieldsError = fieldsError,
                                DocumentError = "Try Again.",
                            });
                            continue;
                        }
                    }
                    decimal _confidenceScore = DiConfigValues.Where(x => x.DocumentCode == documentItem.DocumentCode).Select(x => x.EligibleScore).First();
                    try
                    {


                        var _response = await _documentIntelligentClient.UploadFile(documentBytes, __documentModeldocTypeCheck ? _documentModelId : _documentModel);
                        if (_response is DocumentModelDTO and not null)
                        {
                            DocumentModelDTO _documentdata = (DocumentModelDTO)_response;
                            if (_documentdata.analyzeResult != null)
                            {
                                if ((_documentdata.analyzeResult.documents[0].docType != _documentModeldocType) && __documentModeldocTypeCheck)
                                {
                                    InvalidDocumentDTO.Add(new InvalidDocumentDTO
                                    {
                                        DocumnetCode = documentItem.DocumentCode,
                                        DocuemntId = documentItem.Id,
                                        Attempt = 1,
                                        fieldsError = fieldsError,
                                        DocumentError = "The uploaded document does not match the required document type. Please upload the correct document",
                                    });

                                    bool resultss = await InsertSubTenantKYCDetailsAsync(ReferenceNumber, documentItem.DocumentCode,
                                                            documentItem.Id, InvalidAttempt + 1, false, JsonSerializer.Serialize(kycitems));

                                    ProgressValue = (progresseddocumentVerification * 100) / totalCountdocumentVerification;
                                    await InvokeAsync(StateHasChanged);
                                    continue;
                                }

                                int documentIndex = 1;
                                if (_documentdata.analyzeResult.documents.Count > 1)
                                {
                                    documentIndex = 2;
                                }
                                for (int i = 0; i < documentIndex; i++)
                                {
                                    DIdocumentsDTO _document = _documentdata.analyzeResult.documents[i];

                                    Dictionary<string, DIField> _documentFields = _document.fields;
                                    List<ClsKYCDocumentConfigDetails> requiredFields = [.. DiConfigValues.Where(x => x.DocumentCode == documentItem.DocumentCode && x.IsMandatory == true).OrderByDescending(x => x.FieldName == "Contract To")];
                                    foreach (var requiredfield in requiredFields)
                                    {
                                        List<string> fieldkeys = [.. requiredfield.FieldName.Split(',')];

                                        string finalValue = string.Empty;
                                        int index = 0;
                                        foreach (var _fieldKey in fieldkeys)
                                        {
                                            string _value = string.Empty;
                                            index++;
                                            if (kycitems.Select(x => x.FieldName).Distinct().Contains(_fieldKey))
                                            {
                                                continue;
                                            }
                                            if (!_documentFields.ContainsKey(_fieldKey))
                                            {

                                                fieldsError.Add(new InvalidDocumentFieldDTO
                                                {
                                                    ModelId = _documentModel,
                                                    FieldName = requiredfield.FieldName,
                                                    FieldType = "Nil",
                                                    FieldValue = "Nil",
                                                    ConfidenceScore = 0,
                                                    FieldError = _fieldKey == "Certificate Name" ?
                                                    "The uploaded document does not match the required document type. Please upload the correct document"
                                                    : _fieldKey + " - Required information could not be extracted. Please upload a clearer copy of your document.",
                                                });
                                                continue;
                                            }
                                            decimal confidence = (_documentFields[_fieldKey].confidence * 100);

                                            switch (_documentFields[_fieldKey].type)
                                            {
                                                case "string":
                                                    _value = _documentFields[_fieldKey].valueString;
                                                    break;
                                                case "date":
                                                    _value = _documentFields[_fieldKey].valueDate;
                                                    break;
                                                case "number":
                                                    _value = _documentFields[_fieldKey].valueNumber.ToString();
                                                    break;
                                                case "array":
                                                    _value = JsonSerializer.Serialize(_documentFields[_fieldKey].valueArray);
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (_value == null || string.IsNullOrEmpty(_value))
                                            {
                                                fieldsError.Add(new InvalidDocumentFieldDTO
                                                {
                                                    ModelId = _documentModel,
                                                    FieldName = requiredfield.FieldName,
                                                    FieldType = _documentFields[_fieldKey].type,
                                                    FieldValue = _value ?? string.Empty,
                                                    ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                    FieldError = _fieldKey == "Certificate Name" ?
                                                    "The uploaded document does not match the required document type. Please upload the correct document" :
                                                    _fieldKey + " - Required information could not be extracted. Please upload a clearer copy of your document.",
                                                });
                                                if (InvalidAttempt != 2) continue;
                                            }
                                            else if (confidence < _confidenceScore)
                                            {
                                                fieldsError.Add(new InvalidDocumentFieldDTO
                                                {
                                                    ModelId = _documentModel,
                                                    FieldName = requiredfield.FieldName,
                                                    FieldType = _documentFields[_fieldKey].type,
                                                    FieldValue = _value ?? string.Empty,
                                                    ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                    FieldError = _fieldKey == "Certificate Name" ?
                                                    "The uploaded document does not match the required document type. Please upload the correct document"
                                                    : _fieldKey + " - Required information could not be extracted. Please upload a clearer copy of your document.",
                                                });
                                                if (InvalidAttempt != 2) continue;
                                            }
                                            else if (!string.IsNullOrEmpty(requiredfield.ZValidationType) && (fieldkeys.Count == index))
                                            {
                                                List<string> DMSEIACCertificateNames = ["Initial Approval Certificate".ToLower(), "Approval Certificate".ToLower()];
                                                List<string> DMSEBNECCertificateNames = ["Business Name Reservation Certificate".ToLower(), "Initial Business Name Reservation Certificate".ToLower()];
                                                switch (requiredfield.ZValidationType)
                                                {
                                                    case "DocumentTypeValid":
                                                        switch (documentItem.DocumentCode)
                                                        {
                                                            case "DMSEIAC":
                                                                if (!DMSEIACCertificateNames.Contains(_value.ToLower()))
                                                                {
                                                                    InvalidDocumentDTO.Add(new InvalidDocumentDTO
                                                                    {
                                                                        DocumnetCode = documentItem.DocumentCode,
                                                                        DocuemntId = documentItem.Id,
                                                                        Attempt = InvalidAttempt,
                                                                        fieldsError = fieldsError,
                                                                        DocumentError = "The uploaded document does not match the required document type. Please upload the correct document",
                                                                    });
                                                                }
                                                                break;
                                                            case "DMSEBNEC":
                                                                if (!DMSEBNECCertificateNames.Contains(_value.ToLower()))
                                                                {
                                                                    InvalidDocumentDTO.Add(new InvalidDocumentDTO
                                                                    {
                                                                        DocumnetCode = documentItem.DocumentCode,
                                                                        DocuemntId = documentItem.Id,
                                                                        Attempt = InvalidAttempt,
                                                                        fieldsError = fieldsError,
                                                                        DocumentError = "The uploaded document does not match the required document type. Please upload the correct document",
                                                                    });
                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                        break;
                                                    case "DateTimeExpiry":
                                                        DateTime? _dateExtractionFromDi = DateValidators.IsDateGreater(_value, documentExpiryCompareBaseline, documentItem.DocumentCode);
                                                        if (_dateExtractionFromDi is null)
                                                        {
                                                            _dateExtractionFromDiForTC = false;
                                                            fieldsError.Add(new InvalidDocumentFieldDTO
                                                            {
                                                                ModelId = _documentModel,
                                                                FieldName = requiredfield.FieldName,
                                                                FieldType = _documentFields[_fieldKey].type,
                                                                FieldValue = finalValue,
                                                                ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                FieldError = "Expiry Date" + " - Required information could not be extracted. Please upload a clearer copy of your document.",
                                                            });
                                                            if (InvalidAttempt != 2) continue;
                                                        }
                                                        _dateExtractionFromDiForTC = true;
                                                        if (!(_dateExtractionFromDi > documentExpiryCompareBaseline))
                                                        {
                                                            fieldsError.Add(new InvalidDocumentFieldDTO
                                                            {
                                                                ModelId = _documentModel,
                                                                FieldName = requiredfield.FieldName,
                                                                FieldType = _documentFields[_fieldKey].type,
                                                                FieldValue = finalValue,
                                                                ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                FieldError = documentItem.DocumentCode == "DMSTNCONT" ?
                                                                $"The tenancy contract must have at least {documentExpiryOffsetDays} days validity to upload"
                                                                : "Expiry Date" + " - The document has expired. Please upload a valid document.",
                                                            });
                                                            if (InvalidAttempt != 2) continue;
                                                        }
                                                        break;

                                                    case "MobileNumber":
                                                        string _mobilenumberValidateresponse = DocumentIntelligentVAlidators.GetAllValidUaeMobileNumbers(_documentFields[_fieldKey].valueString);
                                                        if (string.IsNullOrEmpty(_mobilenumberValidateresponse))
                                                        {
                                                            fieldsError.Add(new InvalidDocumentFieldDTO
                                                            {
                                                                ModelId = _documentModel,
                                                                FieldName = requiredfield.FieldName,
                                                                FieldType = _documentFields[_fieldKey].type,
                                                                FieldValue = finalValue,
                                                                ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                //FieldError = _fieldKey + " - Email (or) Mobile number field is missing or could not be identified. Please check your document.",
                                                                FieldError = "Contact details are incorrect Please verify the tenancy contract once again",
                                                            });
                                                            if (InvalidAttempt != 2) continue;
                                                        }
                                                        else if (documentItem.DocumentCode == "DMSTNCONT")
                                                        {

                                                            bool _mobileresponse = false;

                                                            if (requiredfield.FieldName == "Landlord Phone")
                                                            {
                                                                _mobileresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "LANDLORDPHONE", _mobilenumberValidateresponse);
                                                            }
                                                            else
                                                            {
                                                                _mobileresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", _mobilenumberValidateresponse);
                                                            }
                                                            if (!_mobileresponse)
                                                            {
                                                                fieldsError.Add(new InvalidDocumentFieldDTO
                                                                {
                                                                    ModelId = _documentModel,
                                                                    FieldName = requiredfield.FieldName,
                                                                    FieldType = _documentFields[_fieldKey].type,
                                                                    FieldValue = finalValue,
                                                                    ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                    FieldError = _fieldKey + " - Email (or) Mobile number field is missing or could not be identified. Please check your document.",
                                                                });
                                                                if (InvalidAttempt != 2) continue;
                                                            }

                                                            if (fieldsError.Where(x => x.FieldName == requiredfield.FieldName).Any())
                                                            {
                                                                try
                                                                {
                                                                    fieldsError.Remove(fieldsError.FirstOrDefault(x => x.FieldName == requiredfield.FieldName));
                                                                }
                                                                catch (Exception) { }
                                                            }
                                                            tenancyContractValidation.TenantPhone = true;
                                                            tenancyContractValidation.TenantPhoneStatus = 2;
                                                        }
                                                        break;

                                                    case "EmailAddress":
                                                        string _emailValidateresponse = DocumentIntelligentVAlidators.ExtractValidEmails(_documentFields[_fieldKey].valueString);
                                                        if (string.IsNullOrEmpty(_emailValidateresponse))
                                                        {
                                                            fieldsError.Add(new InvalidDocumentFieldDTO
                                                            {
                                                                ModelId = _documentModel,
                                                                FieldName = requiredfield.FieldName,
                                                                FieldType = _documentFields[_fieldKey].type,
                                                                FieldValue = finalValue,
                                                                ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                //FieldError = _fieldKey + " - Email (or) Mobile number field is missing or could not be identified. Please check your document.",
                                                                FieldError = "Contact details are incorrect Please verify the tenancy contract once again",
                                                            });
                                                            if (InvalidAttempt != 2) continue;
                                                        }
                                                        else if (documentItem.DocumentCode == "DMSTNCONT")
                                                        {
                                                            bool _emailresponse = false;
                                                            if (requiredfield.FieldName == "Landload Email")
                                                            {
                                                                _emailresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "LANDLORDEMAIL", _emailValidateresponse);
                                                            }
                                                            else
                                                            {
                                                                _emailresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", _emailValidateresponse);
                                                            }
                                                            if (!_emailresponse)
                                                            {
                                                                fieldsError.Add(new InvalidDocumentFieldDTO
                                                                {
                                                                    ModelId = _documentModel,
                                                                    FieldName = requiredfield.FieldName,
                                                                    FieldType = _documentFields[_fieldKey].type,
                                                                    FieldValue = finalValue,
                                                                    ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                    FieldError = _fieldKey + " - Email (or) Mobile number field is missing or could not be identified. Please check your document.",
                                                                });
                                                                if (InvalidAttempt != 2) continue;
                                                            }
                                                            if (fieldsError.Where(x => x.FieldName == requiredfield.FieldName).Any())
                                                            {
                                                                try
                                                                {
                                                                    fieldsError.Remove(fieldsError.FirstOrDefault(x => x.FieldName == requiredfield.FieldName));
                                                                }
                                                                catch (Exception) { }
                                                            }
                                                            tenancyContractValidation.TenantEmail = true;
                                                            tenancyContractValidation.TenantEmailStatus = 2;
                                                        }
                                                        break;
                                                    case "Name":
                                                        bool _nameresponse = false;
                                                        if (requiredfield.FieldName == "Tenant Name")
                                                        {
                                                            _nameresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTNAME", Regex.Replace(_documentFields[_fieldKey].valueString, @"[^\u0000-\u007F]+", ""));
                                                        }
                                                        else
                                                        {
                                                            _nameresponse = await InsertTenantEmailAndMobileNumber(ReferenceNumber, "LANDLORDNAME", Regex.Replace(_documentFields[_fieldKey].valueString, @"[^\u0000-\u007F]+", ""));
                                                        }

                                                        if (!_nameresponse)
                                                        {
                                                            fieldsError.Add(new InvalidDocumentFieldDTO
                                                            {
                                                                ModelId = _documentModel,
                                                                FieldName = requiredfield.FieldName,
                                                                FieldType = _documentFields[_fieldKey].type,
                                                                FieldValue = finalValue,
                                                                ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                                FieldError = _fieldKey + " - Name field is missing or could not be identified. Please check your document.",
                                                            });
                                                            if (InvalidAttempt != 2) continue;
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            else if (_fieldKey == "DocumentNumber" && documentItem.DocumentCode == "DMSEID")
                                            {
                                                if (!_value.StartsWith("784") || !(_value.Replace("-", "").Length == 15))
                                                {
                                                    fieldsError.Add(new InvalidDocumentFieldDTO
                                                    {
                                                        ModelId = _documentModel,
                                                        FieldName = requiredfield.FieldName,
                                                        FieldType = _documentFields[_fieldKey].type,
                                                        FieldValue = _value ?? string.Empty,
                                                        ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                        FieldError = "The uploaded document does not match the required document type. Please upload the correct document",
                                                    });
                                                    if (InvalidAttempt != 2) continue;
                                                }
                                            }



                                            finalValue += _documentFields[_fieldKey].valueString;


                                            if (fieldkeys.Count == index)
                                            {
                                                kycitems.Add(new KYCObjectDTO
                                                {
                                                    FieldName = requiredfield.FieldName,
                                                    FieldValue = requiredfield.FieldName == "Tenant Name" || requiredfield.FieldName == "Landlord Name" ?
                                                    Regex.Replace(finalValue, @"[^\u0000-\u007F]+", "") : finalValue,
                                                    FieldType = _documentFields[_fieldKey].type,
                                                    ConfidenceScore = _documentFields[_fieldKey].confidence,
                                                });
                                            }

                                            _documentFields.Remove(_fieldKey);
                                        }
                                    }

                                    if (fieldsError.Count == 0 || (InvalidAttempt >= 2))
                                    {
                                        foreach (var field in _documentFields)
                                        {
                                            KYCObjectDTO kYCObjectDTO = new();
                                            switch (_documentFields[field.Key].type)
                                            {
                                                case "string":
                                                    kYCObjectDTO.FieldValue = _documentFields[field.Key].valueString;
                                                    break;
                                                case "date":
                                                    kYCObjectDTO.FieldValue = _documentFields[field.Key].valueDate;
                                                    break;
                                                case "array":
                                                    kYCObjectDTO.FieldValue = JsonSerializer.Serialize(_documentFields[field.Key].valueArray);
                                                    break;
                                                default:
                                                    break;
                                            }
                                            kYCObjectDTO.FieldName = field.Key;
                                            kYCObjectDTO.FieldType = _documentFields[field.Key].type;
                                            kYCObjectDTO.ConfidenceScore = _documentFields[field.Key].confidence;
                                            kycitems.Add(kYCObjectDTO);

                                        }
                                    }
                                }

                                if (fieldsError.Count > 0)
                                {
                                    if (documentItem.DocumentCode == "DMSTNCONT" && fieldsError.Where(x => x.FieldName == "Contract To" ||
                                     (x.FieldName == "EmailAddress" && x.FieldName == "MobileNumber")).Any() && _dateExtractionFromDiForTC)
                                    {
                                        tenancyContractValidation.TenantPhone = false;
                                        tenancyContractValidation.TenantEmail = false;
                                        tenancyContractValidation.TenantPhoneStatus = 1;
                                        tenancyContractValidation.TenantEmailStatus = 1;
                                    }
                                    if (documentItem.DocumentCode == "DMSTNCONT" && fieldsError.Any(x => x.FieldName == "Tenant Email")
                                        && fieldsError.Any(x => x.FieldName == "Tenant Phone"))
                                    {
                                        tenancyContractValidation.TenantPhone = false;
                                        tenancyContractValidation.TenantEmail = false;
                                        tenancyContractValidation.TenantPhoneStatus = 1;
                                        tenancyContractValidation.TenantEmailStatus = 1;
                                    }

                                    if (documentItem.DocumentCode == "DMSTNCONT" && fieldsError.Where(x => x.FieldName == "EmailAddress").Any())
                                        await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", string.Empty);
                                    if (documentItem.DocumentCode == "DMSTNCONT" && fieldsError.Where(x => x.FieldName == "MobileNumber").Any())
                                        await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", string.Empty);

                                    int indexofInvalidDocument = InvalidDocumentDTO.FindIndex(x => x.DocumnetCode == documentItem.DocumentCode);
                                    if (indexofInvalidDocument >= 0)
                                    {
                                        int attempt = InvalidDocumentDTO[indexofInvalidDocument].Attempt;
                                        InvalidDocumentDTO[indexofInvalidDocument] = new InvalidDocumentDTO
                                        {
                                            DocumnetCode = documentItem.DocumentCode,
                                            DocuemntId = documentItem.Id,
                                            Attempt = attempt + 1,
                                            fieldsError = fieldsError,
                                            DocumentError = fieldsError.Count != 0 ? fieldsError[0].FieldError : string.Empty
                                        };
                                    }
                                    else
                                    {
                                        InvalidDocumentDTO.Add(new InvalidDocumentDTO
                                        {
                                            DocumnetCode = documentItem.DocumentCode,
                                            DocuemntId = documentItem.Id,
                                            Attempt = InvalidAttempt,
                                            fieldsError = fieldsError,
                                            DocumentError = fieldsError.Count != 0 ? fieldsError[0].FieldError : string.Empty
                                        });
                                    }
                                }
                                else
                                {
                                    documentvalid = true;
                                }
                            }
                            else
                            {
                                if (documentItem.DocumentCode == "DMSTNCONT")
                                {
                                    await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", string.Empty);
                                    await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", string.Empty);
                                }
                                ;

                                InvalidDocumentDTO.Add(new InvalidDocumentDTO
                                {
                                    DocumnetCode = documentItem.DocumentCode,
                                    DocuemntId = documentItem.Id,
                                    Attempt = InvalidAttempt,
                                    fieldsError = fieldsError,
                                    DocumentError = "The uploaded document does not match the required document type. Please upload the correct document",
                                });
                            }
                        }
                        else
                        {
                            if (documentItem.DocumentCode == "DMSTNCONT")
                            {
                                await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", string.Empty);
                                await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", string.Empty);
                            }
                            ;
                            InvalidDocumentDTO.Add(new InvalidDocumentDTO
                            {
                                DocumnetCode = documentItem.DocumentCode,
                                DocuemntId = documentItem.Id,
                                Attempt = InvalidAttempt,
                                fieldsError = fieldsError,
                            });
                        }
                    }
                    catch (Exception)
                    {
                        if (documentItem.DocumentCode == "DMSTNCONT")
                        {
                            await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", string.Empty);
                            await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", string.Empty);
                        }
                        ;
                        InvalidDocumentDTO.Add(new InvalidDocumentDTO
                        {
                            DocumnetCode = documentItem.DocumentCode,
                            DocuemntId = documentItem.Id,
                            Attempt = 1,
                            fieldsError = fieldsError,
                        });
                    }
                    if ((documentItem.DocumentCode == "DMSTNCONT") && !documentItem.ZSystemVerified)
                    {
                        if (tenancyContractValidation.TenantEmailStatus != 2)
                        {
                            tenancyContractValidation.TenantEmailStatus = 1;
                        }
                        if (tenancyContractValidation.TenantPhoneStatus != 2)
                        {
                            tenancyContractValidation.TenantPhoneStatus = 1;
                        }
                    }

                    bool result = await InsertSubTenantKYCDetailsAsync(ReferenceNumber, documentItem.DocumentCode,
                                                             documentItem.Id, InvalidAttempt + 1, (fieldsError.Count == 0 && documentvalid), JsonSerializer.Serialize(kycitems));
                }

                ProgressValue = (progresseddocumentVerification * 100) / totalCountdocumentVerification;
                await InvokeAsync(StateHasChanged);
            }


            return InvalidDocumentDTO.Count <= 0;
        }

        private async Task<bool> InsertSubTenantKYCDetailsAsync(string RefNo, string DocumentCode, int DocumentId, int attempt, bool verified, string kycItems)
        {
            var responsecode = await NOCService.EOInsertSubTenantKYCDetailsAsync(RefNo, DocumentCode, DocumentId, attempt, verified, kycItems);
            if (responsecode > 0)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> InsertTenantEmailAndMobileNumber(string RefNo, string Type, string Value)
        {
            var rescode = await NOCService.EOInsertTenantEmailAndMobileAsync(RefNo, Type, Value);
            if (rescode > 0)
            {
                return true;
            }
            return false;
        }

        #region OnChange Methods 
        /// <summary>
        /// Called when the user picks a file, before the browser POSTs it over HTTP.
        /// File bytes never travel over the SignalR circuit (see uploadNocDocument in site.js
        /// and NocDocumentUploadController).
        /// </summary>
        public async Task OnUploadStarted(ClsEODocument clsEODocument)
        {
            Logger.LogInformation(
                "Upload[{RefNo}/{DocCode}] STEP 1 - upload started (HTTP path). DocumentId: {DocumentId}",
                ReferenceNumber, clsEODocument.DocumentCode, clsEODocument.Id);

            clsEODocument.Uploading = true;
            clsEODocument.attempt = true;
            ErrorMessageMissingDocuments = string.Empty;
            ErrorMessageUpload = null;
            clsEODocument.ErrorMessageUpload = string.Empty;
            indexError = -1;
            DocumentsListDocumentStatusType(clsEODocument);
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>Called with the JSON result of the HTTP upload.</summary>
        public async Task OnUploadCompleted(ClsEODocument clsEODocument, NocUploadResult result)
        {
            try
            {
                Logger.LogInformation(
                    "Upload[{RefNo}/{DocCode}] STEP 2 - HTTP upload finished. Success: {Success}, ErrorCode: '{ErrorCode}', AttachmentName: '{AttachmentName}'.",
                    ReferenceNumber, clsEODocument.DocumentCode, result?.Success, result?.ErrorCode, result?.AttachmentName);

                if (result == null || !result.Success)
                {
                    clsEODocument.Uploading = false;
                    clsEODocument.IsUploadedSpecified = true;
                    indexError = clsEODocument.Id;
                    switch (result?.ErrorCode)
                    {
                        case "FileTooLarge":
                            clsEODocument.ErrorMessageUpload = L["FileSize5MB"];
                            clsEODocument.attempt = true;
                            break;
                        case "InvalidType":
                            clsEODocument.ErrorMessageUpload = L["InvalidType"];
                            clsEODocument.attempt = false;
                            break;
                        case "PageCountExceeded":
                            clsEODocument.ErrorMessageUpload = "Document exceeds 2 pages. Please re-upload a valid file.";
                            clsEODocument.attempt = false;
                            break;
                        case "ReadTimeout":
                        case "ReadError":
                        case "NoFile":
                            clsEODocument.ErrorMessageUpload = "We could not read this file from your device. Please save a local copy of the file and try again.";
                            clsEODocument.attempt = false;
                            break;
                        default: // "UploadFailed", "Network", "InvalidRequest", null
                            clsEODocument.ErrorMessageUpload = L["ErrorWhileUploadingTheDocument"];
                            break;
                    }
                    DocumentsListDocumentStatusType(clsEODocument);
                    if (clsEODocument.DocumentCode == "DMSTNCONT")
                    {
                        await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTEMAIL", string.Empty);
                        await InsertTenantEmailAndMobileNumber(ReferenceNumber, "TENANTPHONE", string.Empty);
                    }
                    await InvokeAsync(StateHasChanged);
                    return;
                }

                clsEODocument.Uploading = false;
                var documentsGridResult = await NOCService.GetDocumentsListAsync(ReferenceNumber);
                ClsEODocument? clsEODocument1 = documentsGridResult.FirstOrDefault(x => x.DocumentCode == clsEODocument.DocumentCode);
                int DocumentIndex = DocumentsGrid.FindIndex(x => x.DocumentCode == clsEODocument.DocumentCode);
                if (InvalidAttempt == 3)
                {
                    InvalidAttempt = 2;
                }
                if (DocumentIndex >= 0 && clsEODocument1 is not null)
                {
                    DocumentsGrid[DocumentIndex] = clsEODocument1;
                    DocumentsGrid[DocumentIndex].attempt = true;
                    DocumentsListDocumentStatusType(clsEODocument1);
                }
                Logger.LogInformation(
                    "Upload[{RefNo}/{DocCode}] STEP 3 - completed successfully. Grid refreshed, new StatusType: {StatusType}.",
                    ReferenceNumber, clsEODocument.DocumentCode, clsEODocument1?.DocumentStatusType);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    "Upload[{RefNo}/{DocCode}] FAILED while processing upload result / refreshing grid.",
                    ReferenceNumber, clsEODocument.DocumentCode);
                clsEODocument.Uploading = false;
                clsEODocument.IsUploadedSpecified = true;
                clsEODocument.ErrorMessageUpload = L["ErrorWhileUploadingTheDocument"];
                indexError = clsEODocument.Id;
                DocumentsListDocumentStatusType(clsEODocument);
                await InvokeAsync(StateHasChanged);
            }
        }
        #endregion

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private Task OnModalClosing(ModalClosingEventArgs e)
        {
            e.Cancel = modelclose
                || e.CloseReason != CloseReason.UserClosing;

            return Task.CompletedTask;
        }
    }
}
