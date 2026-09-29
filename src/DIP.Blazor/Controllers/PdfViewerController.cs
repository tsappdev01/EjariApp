using DIP.SoapServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using StgDipService;
using static StgDipService.NOCServiceClient;
using Volo.Abp.AspNetCore.Mvc;

namespace DIP.Blazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfViewerController : AbpController
    {
        private readonly string _appKey;

        public PdfViewerController(IOptions<SoapServicesConfiguration> soapServicesOptions)
        {
            _appKey = soapServicesOptions.Value.NOCService.AppKey;
        }

        [HttpGet("GetPdf")]
        public async Task<IActionResult> GetPdf(string referenceNumber, string documentId)
        {
            try
            {
                var decodedRef = Uri.UnescapeDataString(referenceNumber);
                var decodedId = Uri.UnescapeDataString(documentId);
                
                var decryptedRef = EncryptionHelper.DecryptUrlSafe(decodedRef);
                var decryptedId = Convert.ToInt32(EncryptionHelper.DecryptUrlSafe(decodedId));

                var credentials = new ClsCredentials { AppKey = _appKey };
                var client = new NOCServiceClient(EndpointConfiguration.BasicHttpsBinding_INOCService);

                var documentResponse = await client.GetDocumentForRefNoNEWAsync(decryptedRef, decryptedId, credentials);
                
                if (documentResponse?.GetDocumentForRefNoNEWResult == null)
                {
                    return NotFound();
                }

                var pdfBytes = Convert.FromBase64String(documentResponse.GetDocumentForRefNoNEWResult.DMSDocument);
                string mimetype = PdfSharpCoreUtilities.GetMimeType(documentResponse.GetDocumentForRefNoNEWResult.DocumentName);
                return File(pdfBytes, mimetype);
                //return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log exception
                return BadRequest("Error retrieving document: " + ex.Message);
            }
        }
    }
}
