using DIP.CCPayment;
using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.Blazor.Pages.Site.PGResponse.CCAvenueResponse
{
    public partial class PaymentResult
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }
        [Parameter]
        public string OrderStatus { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        ILogger<PaymentResult> _logger { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        [Inject]
        IDistributedCache Cache { get; set; }
        public OrderConfirm Confirmation { get; private set; }
        public CCPaymentResponse CCPaymentResponse { get; set; }
        private string ReferenceNumberDecrypt { get; set; }
        public int InvalidAttempt { get; set; } = 2; //0-failure,1-success,2-loading
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ReferenceNumberDecrypt = EncryptionHelper.DecryptUrlSafe(ReferenceNumber);

            GetEServicesInput getEServicesInput = new()
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

            InvalidAttempt = OrderStatus switch
            {
                "Success" => 1,
                "Failure" => 0,
                "Aborted" => 0,
                _ => 0,
            };

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }
            await CreateResponseForOrderId();
        }
        private async Task CreateResponseForOrderId()
        {
            var cacheresponseBytes = await Cache.GetAsync(ReferenceNumberDecrypt);
            _logger.LogInformation("Cache response bytes for ReferenceNumber {ReferenceNumberDecrypt}: {CacheResponseBytes}", ReferenceNumberDecrypt, cacheresponseBytes);
            if (cacheresponseBytes == null)
            {
                InvalidAttempt = 0;
                return;
            }
            string json = Encoding.UTF8.GetString(cacheresponseBytes);
            _logger.LogInformation("Cache response JSON for ReferenceNumber {ReferenceNumberDecrypt}: {CacheResponseJson}", ReferenceNumberDecrypt, json);
            CCPaymentResponse = JsonConvert.DeserializeObject<CCPaymentResponse>(json) ?? throw new NullReferenceException("Argument not be nul in the response value.(CCresponse)");
            var key = "LoggingId" + CCPaymentResponse.OrderId.ToString();
            var LogIdCache = await Cache.GetStringAsync(key);
            int logId = Int32.Parse(LogIdCache ?? "0");

            TransactionResponseInfo objResponse = new()
            {
                ReferenceNumber = ReferenceNumberDecrypt,
                VersionNumber = "1.0",
                TxnType = CCPaymentResponse.PaymentMode,
                Language = CCPaymentResponse.Language,
                MerchantId = CCPaymentResponse.MerchantId,
                TxnRefNo = CCPaymentResponse.TxnRefNo,
                Amount = Convert.ToInt64((CCPaymentResponse.Amount * 100)).ToString(),
                BillReference = CCPaymentResponse.BillReference,
                txtCurrency = CCPaymentResponse.Currency.ToString(),
                txtDateTime = string.Empty,
                ResponseCode = CCPaymentResponse.OrderStatus.Equals("success", StringComparison.CurrentCultureIgnoreCase) ? "000" : CCPaymentResponse.StatusCode,
                ResponseMessage = CCPaymentResponse.StatusMessage,
                RetreivalReferenceNo = CCPaymentResponse.TrackingId,
                AuthCode = string.Empty,
                BankId = CCPaymentResponse.BankRefNo,
                CustomerCardNo = string.Empty,
                SecureHash = "Need to fix",
                SettlementExpiry = string.Empty,
                ProductID = string.Empty,
                ppmbf_1 = CCPaymentResponse.CardName,
                ppmbf_2 = string.Empty,
                ppmbf_3 = string.Empty,
                ppmbf_4 = string.Empty,
                ppmbf_5 = string.Empty,
            };

            Confirmation = await NOCService.CreateResponseForOrderIdAsync(objResponse, "PGPaymentResponseCard");

            string XmlStr = "rawHashData";
            await NOCService.UpdateUaePgsResponseLogAsync(new ResponseLog
            {
                LogId = logId,
                pp_ResponseCode = objResponse.ResponseCode,
                pp_ResponseMessage = objResponse.ResponseMessage,
                pp_SecureHash = objResponse.SecureHash,
                ReturnString = XmlStr
            });

            return;
        }
    }
}
