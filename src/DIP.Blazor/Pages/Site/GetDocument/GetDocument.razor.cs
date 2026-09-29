using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using StgDipService;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotLiquid.Util;
using System.Web;
using DIP.SoapServices;
using Blazorise;
using System.Globalization;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;
using DIP.PageInfos;
using DIP.Blazor.Shared;
using DIP.Settings;
using System.IO;
using Microsoft.JSInterop;
using Volo.Abp.DependencyInjection;

namespace DIP.Blazor.Pages.Site.GetDocument
{
    public partial class GetDocument
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private EditContext? EditContextRegistration;
        private EditContext? EditContextForget;

        private RegistrationDto RegistrationDto { get; set; }
        private DocumentDto DocumentDto { get; set; }


        private ValidationMessageStore? ValidationMessageStoreRegistration { get; set; }
        private ValidationMessageStore? ValidationMessageStoreForget { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public string RegisterMessageError { get; set; }
        public string DoneMessage { get; set; }
        public bool hasError { get; set; }
        public bool hasForgetError { get; set; }
        public bool resetDone { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public List<SelectListItem> Issuers = new List<SelectListItem>();
        public List<SelectListItem> Categories = new List<SelectListItem>();
        private List<SelectListItem> Directions = new List<SelectListItem>();

        private bool DisableButton = false;
        private bool isCaptchaValid = false;
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }


        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public string base64String;

        private string GetExtensionFromContentType(string contentType)
        {
            return contentType switch
            {
                "application/pdf" => ".pdf",
                // Common raster image formats
                "image/jpeg" => ".jpg",  // or ".jpeg"
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/webp" => ".webp",
                "image/tiff" => ".tiff",  // or ".tif"
                "image/x-icon" => ".ico",
                "image/vnd.microsoft.icon" => ".ico",

                // Vector image formats
                "image/svg+xml" => ".svg",
                "image/svg" => ".svg",  // Older variant
                "image/svg-xml" => ".svg",  // Rare variant

                // RAW image formats (photography)
                "image/x-canon-cr2" => ".cr2",
                "image/x-nikon-nef" => ".nef",
                "image/x-sony-arw" => ".arw",
                "image/x-adobe-dng" => ".dng",
                "image/x-fuji-raf" => ".raf",
                "image/x-panasonic-rw2" => ".rw2",
                "image/x-olympus-orf" => ".orf",
                "image/x-pentax-pef" => ".pef",
                "image/x-sigma-x3f" => ".x3f",

                // Less common but still used
                "image/jp2" => ".jp2",
                "image/jpx" => ".jpx",
                "image/jpm" => ".jpm",
                "image/heic" => ".heic",
                "image/heif" => ".heif",
                "image/avif" => ".avif",
                "image/vnd.djvu" => ".djvu",
                "image/vnd.dwg" => ".dwg",
                "image/vnd.wap.wbmp" => ".wbmp",
                "image/x-portable-anymap" => ".pnm",
                "image/x-portable-bitmap" => ".pbm",
                "image/x-portable-graymap" => ".pgm",
                "image/x-portable-pixmap" => ".ppm",
                "image/x-xbitmap" => ".xbm",
                "image/x-xpixmap" => ".xpm",
                "image/x-cmu-raster" => ".ras",
                "image/x-rgb" => ".rgb",

                // Photoshop formats
                "image/vnd.adobe.photoshop" => ".psd",
                "image/x-photoshop" => ".psd",

                // Medical imaging
                "image/dicom-rle" => ".dcm",
                "image/x-dicom" => ".dcm",

                // 3D and CAD formats
                "image/x-3ds" => ".3ds",
                "image/x-mrsid-image" => ".sid",

                // Fallback for unknown image types
                _ => ".bin"
            };
        }

        private async Task DownloadBase64File(string base64Content, string fileName, string contentType)
        {
            var bytes = Convert.FromBase64String(base64Content);

            // If no file extension is provided, try to determine it from content type
            if (!Path.HasExtension(fileName))
            {
                var extension = GetExtensionFromContentType(contentType);
                fileName = $"{fileName}{extension}";
            }

            // Use JS interop to trigger the download
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, contentType, bytes);
        }

        public string DetectFileType(byte[] bytes)
        {
            if (bytes.Length < 4) return "application/octet-stream";

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return "image/png";

            // JPEG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return "image/jpeg";

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return "image/gif";

            // PDF
            if (bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46)
                return "application/pdf";

            // Add more signatures as needed...
            if (bytes.Length >= 5)
            {
                // Check for "<?xml" (UTF-8)
                if (bytes[0] == 0x3C && bytes[1] == 0x3F && bytes[2] == 0x78 && bytes[3] == 0x6D && bytes[4] == 0x6C)
                    return "image/svg+xml";

                // Check for "<svg" (UTF-8)
                if (bytes[0] == 0x3C && bytes[1] == 0x73 && bytes[2] == 0x76 && bytes[3] == 0x67)
                    return "image/svg+xml";
            }
            return "application/octet-stream"; // Default for unknown types
        }
        private async Task DownloadFile()
        {
            try
            {
                var (contentType, cleanBase64) = ParseBase64String(base64String);
                var bytes = Convert.FromBase64String(cleanBase64);
                // If content type couldn't be determined, default to octet-stream
                contentType ??= DetectFileType(bytes);

                // Generate a filename with appropriate extension
                var fileName = $"{DocumentDto.DocumentId}{GetExtensionFromContentType(contentType)}";
                await DownloadBase64File(cleanBase64, fileName, contentType);

            }
            catch (Exception ex)
            {
                hasForgetError = true;
                RegisterMessageError = @L["FaildToDownloadTheFile"];
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        private (string? contentType, string cleanBase64) ParseBase64String(string base64)
        {
            // Check if the string includes content type metadata
            var match = System.Text.RegularExpressions.Regex.Match(
                base64,
                @"^data:(?<type>[a-z]+\/[a-z0-9\-\.\+]+);base64,(?<data>.+)$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return (match.Groups["type"].Value, match.Groups["data"].Value);
            }

            // No content type metadata found - return just the clean base64
            return (null, base64);
        }
        private async Task OnClickDownload()
        {
            try
            {
                hasForgetError = false;
                DisableButton = true;
                if (isCaptchaValid == false)
                {
                    DisableButton = false;
                    return;
                }
                DisableButton = false;
                var statusRes = await NOCService.CheckProcessingPageAsync(DocumentDto.ReferenceNumber);
                if (!statusRes.StatusName.Equals("Completed"))
                {
                    if (statusRes.StatusName.Equals("Payment") || statusRes.StatusName.Equals("PaymentProcess") || statusRes.StatusName.Equals("Submitted") || statusRes.StatusName.Equals("Upload"))
                    {
                        bool success = Int32.TryParse(DocumentDto.DocumentId, out int res);
                        if (success)
                        {
                            var result = await NOCService.GetDocumentForRefNoNEWAsync(DocumentDto.ReferenceNumber, Int32.Parse(DocumentDto.DocumentId));
                            if (result != null)
                            {
                                resetDone = true;
                                base64String = result.DMSDocument;
                                await DownloadFile();
                                DocumentDto = new DocumentDto();

                            }
                            else
                            {
                                hasForgetError = true;
                                RegisterMessageError = @L["FaildToDownloadTheFile"];
                            }
                        }
                        else
                        {
                            hasForgetError = true;
                            RegisterMessageError = @L["FaildToDownloadTheFile"];

                        }

                    }
                    else
                    {
                        hasForgetError = true;
                        RegisterMessageError = @L["FaildToDownloadTheFile"];
                    }
                }
                else
                {
                    hasForgetError = true;
                    RegisterMessageError = @L["FaildToDownloadTheFile"];
                }
            }
            catch (Exception e)
            {
                hasForgetError = true;
                RegisterMessageError = @L["FaildToDownloadTheFile"];
            }



        }



        private void EditContextRegistration_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreRegistration?.Clear();
        }

        private void EditContextForget_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreForget?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            DocumentDto = new DocumentDto();

            EditContextForget = new(DocumentDto);
            ValidationMessageStoreForget = new ValidationMessageStore(EditContextForget);
            EditContextForget.OnValidationRequested += EditContextForget_OnValidationRequested;

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
        }


        private async Task Navigat()
        {
            NavigationManager.NavigateTo($"{CultureInfo.CurrentCulture.Name}/Ejari");
        }



        private void OnSuccessCaptchaForget(bool isValid)
        {
            isCaptchaValid = isValid;
            if (isValid)
            {
                DocumentDto.Captcha = "DONE";
                if (EditContextForget != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextForget.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextForget.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }

        }


    }
}
