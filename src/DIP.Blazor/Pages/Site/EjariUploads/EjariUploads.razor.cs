using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using Org.BouncyCastle.Crypto.Engines;
using System.Globalization;
using StgDipService;
using DIP.SoapServices;
using System.Linq;
using System.IO;
using Blazorise;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using DIP.EFormServiceSubCategories;
using Volo.Abp.BlobStoring;

using System.Collections;
using DIP.UploadEjariFiles;
using Microsoft.JSInterop;
using DIP.PageInfos;
using MiniExcelLibs.Utils;
using System.Text.RegularExpressions;

namespace DIP.Blazor.Pages.Site.EjariUploads
{
    public partial class EjariUploads
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private EditContext? EditContextEjariDetails;
        private string encryptedReferenceNumber;

        public NocDetailsDto NocDetailsDto { get; set; }

        private ValidationMessageStore? ValidationMessageStoreEjariDetails { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public bool hasError { get; set; }


        public int NumberDocumentPending = 3;

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public ClsRegistration Info { get; private set; }
        public List<ClsEODocument> DocumentsGrid { get; private set; }
        public byte[] FormFileContent { get; set; }
        public string FormFile { get; set; } = "";

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        [Inject]
        public IBlobContainer<UploadEjariFilesContainer> EFormServiceSubCategoryContainer { get; set; }
        public string ErrorMessageUpload { get; set; }

        private bool haveMissingDocument = false;

        public string MissingDocumnetErrorMessage { get; set; }

        private int indexError;



        [Inject]
        public IJSRuntime JS { get; set; }


        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        private async Task OnSubmit()
        {
            var result = await NOCService.EOSendForPaymentAsync(ReferenceNumber);
            if (result)
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
            }
            haveMissingDocument = true;
            MissingDocumnetErrorMessage = @L["PleaseMakeSureAllDocumentsAreUploadedSuccessfully"];
        }


        private void EditContextEjariDetails_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreEjariDetails?.Clear();
        }


        protected override async Task OnInitializedAsync()
        {
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);


            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);


            NocDetailsDto = new NocDetailsDto();
            EditContextEjariDetails = new(NocDetailsDto);
            ValidationMessageStoreEjariDetails = new ValidationMessageStore(EditContextEjariDetails);
            EditContextEjariDetails.OnValidationRequested += EditContextEjariDetails_OnValidationRequested;

            GetEServicesInput getEServicesInput = new GetEServicesInput
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


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
        private async Task PrepareData()
        {
            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            if (!status.StatusName.Equals("Upload") || Info?.ZzApplicationType == "NewURL")
            {
                switch (status.StatusName)
                {
                    case "Registered":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Verified":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Upload":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}");
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
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                }
            }
            DocumentsGrid = (await NOCService.GetDocumentsListAsync(ReferenceNumber)).OrderBy(x => x.Id).ToList();
        }

        private bool IsAllowedFileType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            // List of allowed MIME types
            var allowedTypes = new[]
            {
        "application/pdf",          // PDF
        "image/png",                // PNG
        "image/jpeg",               // JPEG/JPG
        "image/gif",                // GIF
        "image/svg+xml"             // SVG
    };

            return allowedTypes.Contains(contentType);
        }
        public async Task OnFileUpload(ClsEODocument clsEODocument, InputFileChangeEventArgs e, int index)
        {
            ErrorMessageUpload = null;
            indexError = -1;
            var isuploaded = false;

            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    FormFileContent = await result.GetAllBytesAsync();
                    var size = FormFileContent.Length / 1024;

                    if (size > 5000)
                    {
                        clsEODocument.IsUploadedSpecified = true;
                        ErrorMessageUpload = L["FileSize5MB"];
                        indexError = clsEODocument.Id;

                        return;
                    }
                    string fileName = Path.GetFileNameWithoutExtension(e.File.Name);

                    // Replace spaces with nothing (remove)
                    fileName = fileName.Replace(" ", "");

                    // Remove special characters except letters, numbers, dots, dashes, and underscores
                    fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9\.\-_]", "");
                    FormFile = $"{fileName}{Path.GetExtension(e.File.Name)}";
                    var contentType = e.File.ContentType.ToLowerInvariant();
                    if (!IsAllowedFileType(contentType))
                    {

                        clsEODocument.IsUploadedSpecified = true;
                        ErrorMessageUpload = L["InvalidType"];
                        indexError = clsEODocument.Id;

                        return;

                    }
                    try
                    {

                        var obj = new ClsEODocument()
                        {
                            RefNo = ReferenceNumber.ToString(),
                            AttachmentName = FormFile,
                            DMSDocument = Convert.ToBase64String(FormFileContent),
                            DocSize = (FormFileContent.Length / 1024).ToString(),
                            Id = clsEODocument.Id,
                            IdSpecified = true,
                        };
                        isuploaded = await NOCService.UploadDocumentForRefNoNEWAsync(obj);

                        if (isuploaded)
                        {
                            if (!FormFile.IsNullOrEmpty() && !FormFileContent.IsNullOrEmpty())
                            {
                                await EFormServiceSubCategoryContainer.SaveAsync($"{ReferenceNumber}/{clsEODocument.DocName}_{clsEODocument.Id}/{FormFile}"
                                , FormFileContent, true);
                            }
                        }
                        else
                        {
                            clsEODocument.IsUploadedSpecified = true;
                            ErrorMessageUpload = L["ErrorWhileUploadingTheDocument"];
                            indexError = clsEODocument.Id;

                            return;
                        }

                        DocumentsGrid = (await NOCService.GetDocumentsListAsync(ReferenceNumber)).OrderBy(x => x.Id).ToList();

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }



    }
}
