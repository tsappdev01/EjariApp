using DIP.SoapServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StgDipService;
using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace DIP.Blazor.Controllers
{
    /// <summary>
    /// Receives NOC document uploads as a plain multipart HTTP POST so that file bytes
    /// never travel over the Blazor SignalR circuit (which is unreliable on long polling).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NocDocumentUploadController : AbpController
    {
        private const long MaxFileSizeBytes = 5000 * 1024; // 5 MB (same limit as the page)

        private readonly INOCServiceWrapper _nocService;

        public NocDocumentUploadController(INOCServiceWrapper nocService)
        {
            _nocService = nocService;
        }

        public class UploadResultDto
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("errorCode")]
            public string ErrorCode { get; set; }

            [JsonPropertyName("attachmentName")]
            public string AttachmentName { get; set; }
        }

        [HttpPost("Upload")]
        [IgnoreAntiforgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromForm] string encryptedReferenceNumber,
            [FromForm] string encryptedDocumentId,
            [FromForm] string documentCode)
        {
            string referenceNumber;
            int documentId;
            try
            {
                referenceNumber = EncryptionHelper.DecryptUrlSafe(Uri.UnescapeDataString(encryptedReferenceNumber ?? string.Empty));
                documentId = Convert.ToInt32(EncryptionHelper.DecryptUrlSafe(Uri.UnescapeDataString(encryptedDocumentId ?? string.Empty)));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "NocDocumentUpload - could not decrypt reference/document id parameters.");
                return BadRequest(new UploadResultDto { Success = false, ErrorCode = "InvalidRequest" });
            }

            if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(documentCode))
            {
                Logger.LogWarning("Upload[{RefNo}/{DocCode}] API - REJECTED: missing file or document code.", referenceNumber, documentCode);
                return BadRequest(new UploadResultDto { Success = false, ErrorCode = "InvalidRequest" });
            }

            Logger.LogInformation(
                "Upload[{RefNo}/{DocCode}] API STEP 1 - received HTTP upload. File: '{FileName}', Size: {FileSize} bytes, ContentType: '{ContentType}', DocumentId: {DocumentId}",
                referenceNumber, documentCode, file.FileName, file.Length, file.ContentType, documentId);

            try
            {
                if (file.Length > MaxFileSizeBytes)
                {
                    Logger.LogWarning(
                        "Upload[{RefNo}/{DocCode}] API STEP 2 - REJECTED: file size {FileSize} bytes exceeds 5 MB limit.",
                        referenceNumber, documentCode, file.Length);
                    return Ok(new UploadResultDto { Success = false, ErrorCode = "FileTooLarge" });
                }

                var diConfigValues = await _nocService.GetDIModelConfigurationValueAsync();
                var diDocumentCodes = diConfigValues.Select(x => x.DocumentCode).Distinct().ToList();

                var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();
                bool typeAllowed = diDocumentCodes.Contains(documentCode)
                    ? contentType == "application/pdf"
                    : contentType is "application/pdf" or "image/png" or "image/jpeg";
                if (!typeAllowed)
                {
                    Logger.LogWarning(
                        "Upload[{RefNo}/{DocCode}] API STEP 3 - REJECTED: content type '{ContentType}' not allowed.",
                        referenceNumber, documentCode, contentType);
                    return Ok(new UploadResultDto { Success = false, ErrorCode = "InvalidType" });
                }

                byte[] fileContent;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileContent = ms.ToArray();
                }
                Logger.LogInformation(
                    "Upload[{RefNo}/{DocCode}] API STEP 4 - file read completed ({ReceivedBytes} bytes).",
                    referenceNumber, documentCode, fileContent.Length);

                var prebuildDocumentCodes = diConfigValues
                    .Where(x => x.ModelId != null && x.ModelId.Contains("prebuilt-idDocument"))
                    .Select(x => x.DocumentCode)
                    .Distinct()
                    .ToList();
                if (prebuildDocumentCodes.Contains(documentCode))
                {
                    int pageCount = PdfSharpCoreUtilities.GetPdfPageCount(fileContent);
                    Logger.LogInformation(
                        "Upload[{RefNo}/{DocCode}] API STEP 5 - page count check: {PageCount} page(s).",
                        referenceNumber, documentCode, pageCount);
                    if (pageCount > 2)
                    {
                        Logger.LogWarning(
                            "Upload[{RefNo}/{DocCode}] API STEP 5 - REJECTED: {PageCount} pages exceeds the 2-page limit.",
                            referenceNumber, documentCode, pageCount);
                        return Ok(new UploadResultDto { Success = false, ErrorCode = "PageCountExceeded" });
                    }
                }

                // Same sanitization as the previous circuit-based upload.
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                fileName = fileName.Replace(" ", "");
                fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9\.\-_]", "");
                string attachmentName = $"{(fileName.Length > 15 ? fileName.Substring(0, 12) : fileName)}{Path.GetExtension(file.FileName)}";

                var document = new ClsEODocument
                {
                    RefNo = referenceNumber,
                    AttachmentName = attachmentName,
                    DMSDocument = Convert.ToBase64String(fileContent),
                    DocSize = (fileContent.Length / 1024).ToString(),
                    Id = documentId,
                    IdSpecified = true,
                };

                Logger.LogInformation(
                    "Upload[{RefNo}/{DocCode}] API STEP 6 - calling UploadDocumentForRefNoNEW (SOAP). DocSize: {DocSizeKb} KB, AttachmentName: '{AttachmentName}'.",
                    referenceNumber, documentCode, document.DocSize, attachmentName);

                bool isUploaded = await _nocService.UploadDocumentForRefNoNEWAsync(document);

                Logger.LogInformation(
                    "Upload[{RefNo}/{DocCode}] API STEP 7 - SOAP upload returned: {IsUploaded}.",
                    referenceNumber, documentCode, isUploaded);

                return Ok(new UploadResultDto
                {
                    Success = isUploaded,
                    ErrorCode = isUploaded ? null : "UploadFailed",
                    AttachmentName = attachmentName,
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    "Upload[{RefNo}/{DocCode}] API FAILED with unhandled exception. File: '{FileName}'.",
                    referenceNumber, documentCode, file.FileName);
                return Ok(new UploadResultDto { Success = false, ErrorCode = "UploadFailed" });
            }
        }
    }
}
