using Azure;
using DIP.CCPayment;
using DIP.EServices;
using DIP.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StgDipService;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DIP.Blazor.Pages.Site.PGResponse.CCAvenueResponse
{
    [IgnoreAntiforgeryToken]
    public class SuccessResponseModel : PageModel
    {
        public IActionResult OnGet()
        {
            return BadRequest("Invalid payment response");
        }

        public string EncResp { get; set; }
        [Inject]
        public IDIPAppsettingService IdIPAppsettingService { get; set; }
        public CCPaymentConfigValues CCPaymentConfigValues { get; set; }
        [Inject]
        public ICCPaymentHelpers ICcPaymentHelpers { get; set; }
        public CCPaymentResponse PaymentResponse { get; set; } = new CCPaymentResponse();
        [Inject]
        IDistributedCache Cache { get; set; }
        public OrderConfirm Confirmation { get; private set; }
        private string ReferenceNumber { get; set; }
        private CCPaymentCacheDTO CCPaymentCacheDTO { get; set; }

        public SuccessResponseModel(IDistributedCache distributedCache)
        {
            Cache = distributedCache;
        }
        public async Task<IActionResult> OnPostAsync([FromForm] string encResp)
        {
            if (string.IsNullOrWhiteSpace(encResp))
            {
                return BadRequest("Invalid payment response");
            }

            CCPaymentConfigValues = await IdIPAppsettingService.GetCardPaymentConfigurations();

            PaymentResponse = await ICcPaymentHelpers.ParseEncryptedResponseAsync(encResp);

            var cacheresponseBytes = await Cache.GetAsync(PaymentResponse.OrderId);
            if (cacheresponseBytes == null)
            {
                return BadRequest("Invalid payment response");
            }
            string json = Encoding.UTF8.GetString(cacheresponseBytes);

            CCPaymentCacheDTO = JsonConvert.DeserializeObject<CCPaymentCacheDTO>(json) ?? throw new NullReferenceException("Argument not be nul in the response value.(CC)");
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(CCPaymentCacheDTO.ReferenceNumber);

            PaymentResponse.BillReference = CCPaymentCacheDTO.BillReference;
            PaymentResponse.TxnRefNo = CCPaymentCacheDTO.TxnRefNo;
            PaymentResponse.MerchantId = CCPaymentCacheDTO.MerchantId;
            PaymentResponse.Language = CCPaymentCacheDTO.Language;
            PaymentResponse.ReferenceNumber = CCPaymentCacheDTO.ReferenceNumber;


            await Cache.SetStringAsync(ReferenceNumber, JsonConvert.SerializeObject(PaymentResponse),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        });

            EncResp = encResp;
            return Redirect($"/{CultureInfo.CurrentCulture.Name}/Payment/Result/{CCPaymentCacheDTO.ReferenceNumber}/{PaymentResponse.OrderStatus}");
        }
    }
}
