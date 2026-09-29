using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using StgDipService;
using DIP.UploadEjariFiles;
using Volo.Abp.BlobStoring;
using System.IO;
using System.Linq;
using Volo.Abp;
using DIP.SoapServices;
using System.Globalization;
using DIP.PageInfos;
using System.Text.RegularExpressions;

namespace DIP.Blazor.Pages.Site.EjariUploadPayslip
{
    public partial class EjariUploadPayslip
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


        [Inject]
        public IBlobContainer<UploadPaySlipContainer> EFormServiceSubCategoryContainer { get; set; }
        public string ErrorMessageUpload { get; set; }

        private bool haveMissingDocument = false;

        public string MissingDocumnetErrorMessage { get; set; }

        private int indexError;

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }



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

            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);

            if (!status.StatusName.Equals("Payment") && !status.StatusName.Equals("PaymentProcess"))
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
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");
                        break;
                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;



                }
            }

            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            DocumentsGrid = (await NOCService.GetPaymentSlipListAsync(ReferenceNumber)).OrderBy(x => x.Id).ToList();
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
            haveMissingDocument = false;
            indexError = -1;
            var isuploaded = false;
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    FormFileContent = await result.GetAllBytesAsync();
                    var size = FormFileContent.Length / 1024;

                    if (size > 2000)
                    {
                        clsEODocument.IsUploadedSpecified = true;
                        ErrorMessageUpload = @L["FileSize2MB"];
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
                            RefNo = ReferenceNumber,
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


                        DocumentsGrid = (await NOCService.GetPaymentSlipListAsync(ReferenceNumber)).OrderBy(x => x.Id).ToList();

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


        private async Task OnSubmit()
        {

            var documentUpload = DocumentsGrid?.Count() - DocumentsGrid?.Count(x => x.IsUploaded == true);
            if (documentUpload == 0)
            {
                var result = await NOCService.EOSendForPaymentAsync(ReferenceNumber);
                if (result)
                {
                    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                }

            }
            else
            {

                haveMissingDocument = true;
                MissingDocumnetErrorMessage = @L["PleaseMakeSureAllDocumentsAreUploadedSuccessfully"];
            }


        }

        private async Task OnCancelClick()
        {

            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber.Trim());
            if (status.StatusName.Equals("Payment"))
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");

            }
            else
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
            }
        }

    }
}
