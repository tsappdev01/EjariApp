using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.LastEventss;
using System.Collections.Generic;
using DIP.PageInfos;
using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using DIP.SupportedBanks;
using StgDipService;
using DIP.SoapServices;
using Blazorise;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Excubo.Blazor.TreeViews;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Distributed;
using static DIP.Controllers.CallBackBankController;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Excubo.Generators.Blazor.ExperimentalDoNotUseYet;


namespace DIP.Blazor.Pages.Site.PGResponse
{
    public partial class PGResponse
    {
        [Parameter]
        public string Lang { get; set; }



        [Parameter]

        public string ReferenceNumber { get; set; }

        [Parameter]

        public IFormCollection FormCallBack { get; set; }

        private string encryptedReferenceNumber;

        [Inject]
        public IOptions<SoapServicesConfiguration> SoapServicesOptions { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private OnlinePaymentDto OnlinePaymentDto { get; set; }

        public List<OnlinePaymentDto> listOnlinePayment = new List<OnlinePaymentDto>();

        public List<SupportedBankFrontEnd> SupportedBankFrontEnds = new List<SupportedBankFrontEnd>();
        private ProcessingPage checkProcessingPage;
        private PaymentHistory[] getPaymentHistory;
        private bool disableConfirm;

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        public bool IsNOC { get; set; }
        public bool isAgree { get; set; }
        public bool hasCancel { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public List<Order> Matrix { get; private set; }
        public string OrderName { get; private set; }
        public string OrderId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public ClsRegistration info { get; private set; }
        public OrderConfirm Confirmation { get; private set; }

        SortedList transactionResponse = new SortedList(new VPCStringComparer());
        int LoggingId { get; set; }

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        string ErrorMessage { get; set; }



        [Inject]
        IDistributedCache Cache { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string Ref { get; set; }

        private BankCallbackFormDto FormData;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

        }
        protected override async Task OnInitializedAsync()
        {

            if (!string.IsNullOrEmpty(Ref))
            {
                var json = await Cache.GetStringAsync(Ref);
                if (!string.IsNullOrEmpty(json))
                {
                    FormData = JsonConvert.DeserializeObject<BankCallbackFormDto>(json);

                    // Convert dictionary to FormCollection
                    var formDict = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();

                    foreach (var kvp in FormData.FormFields)
                    {
                        formDict.Add(kvp.Key, kvp.Value);
                    }

                    FormCallBack = new FormCollection(formDict);


                }
            }


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
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Online/PaymentServices"));
            }


            Logger.LogError("FormCallBack" + JsonConvert.SerializeObject(FormCallBack));

            await fetchData();
        }


        private async Task fetchData()
        {

            {
                try
                {

                    Utilities objUtil = new Utilities(SoapServicesOptions.Value.NOCService.AppKey);
                    //string HASHKEY = "";
                    string HASHKEY = objUtil.HASHKEY;

                    StringBuilder RawData = new StringBuilder();
                    StringBuilder NormalDateLbyLRawData = new StringBuilder();
                    string rawHashData = HASHKEY;
                    string signatureSeparattor = "&";
                    Dictionary<string, StringValues>? fields = new Dictionary<string, StringValues>();


                    //await formCallBack.InvokeAsync(form);
                    foreach (string item in FormCallBack.Keys)
                    {

                        if ((FormCallBack[item] != "") && item.StartsWith("pp"))
                        {
                            transactionResponse.Add(item, FormCallBack[item]);

                            RawData.Append(Environment.NewLine);
                            RawData.Append(item + ":" + transactionResponse[item].ToString());

                            Logger.LogError("item after Append PgResponse" + JsonConvert.SerializeObject(item));

                            if (!item.Equals("pp_SecureHash"))
                            {
                                rawHashData += signatureSeparattor + transactionResponse[item].ToString();
                            }
                        }
                    }
                    Logger.LogError("rawHashData PgResponse" + JsonConvert.SerializeObject(rawHashData));
                    Logger.LogError("transactionResponse PgResponse" + JsonConvert.SerializeObject(transactionResponse));



                    #region Response Processing
                    var t = transactionResponse["pp_SecureHash"].ToString();
                    var tt = objUtil.CreateHMACSha256Str(HASHKEY, rawHashData);
                    Logger.LogError("transactionResponse" + JsonConvert.SerializeObject(transactionResponse));
                    Logger.LogError("objUtil.CreateHMACSha256Str(HASHKEY, rawHashData)" + objUtil.CreateHMACSha256Str(HASHKEY, rawHashData));

                    if (transactionResponse["pp_SecureHash"].ToString().Equals(objUtil.CreateHMACSha256Str(HASHKEY, rawHashData)))
                    {

                        // insert into response
                        TransactionResponseInfo objResponse = new TransactionResponseInfo();
                        OrderConfirm obj = new OrderConfirm();
                        var RefNumberCache = await Cache.GetStringAsync(transactionResponse["pp_TxnRefNo"].ToString());
                        Logger.LogError("LogIdCache LogIdCache" + JsonConvert.SerializeObject(RefNumberCache));

                        objResponse.ReferenceNumber = RefNumberCache;

                        var key = "LoggingId" + transactionResponse["pp_TxnRefNo"].ToString();
                        var LogIdCache = await Cache.GetStringAsync(key);
                        Logger.LogError("LogIdCache LogIdCache" + JsonConvert.SerializeObject(LogIdCache));

                        int logId = Int32.Parse(LogIdCache); ;
                        if (transactionResponse["pp_Version"] != null)
                        {
                            objResponse.VersionNumber = transactionResponse["pp_Version"].ToString();
                        }
                        else
                        {
                            objResponse.VersionNumber = string.Empty;
                        }

                        if (transactionResponse["pp_TxnType"] != null)
                        {
                            objResponse.TxnType = transactionResponse["pp_TxnType"].ToString();
                        }
                        else
                        {
                            objResponse.TxnType = string.Empty;
                        }

                        if (transactionResponse["pp_Language"] != null)
                        {
                            objResponse.Language = transactionResponse["pp_Language"].ToString();
                        }
                        else
                        {
                            objResponse.Language = string.Empty;
                        }

                        if (transactionResponse["pp_MerchantID"] != null)
                        {
                            objResponse.MerchantId = transactionResponse["pp_MerchantID"].ToString();
                        }
                        else
                        {
                            objResponse.MerchantId = string.Empty;
                        }
                        if (transactionResponse["pp_TxnRefNo"] != null)
                        {
                            objResponse.TxnRefNo = transactionResponse["pp_TxnRefNo"].ToString();
                        }
                        else
                        {
                            objResponse.TxnRefNo = string.Empty;
                        }
                        if (transactionResponse["pp_Amount"] != null)
                        {
                            objResponse.Amount = transactionResponse["pp_Amount"].ToString();
                        }
                        else
                        {
                            objResponse.Amount = string.Empty;
                        }
                        if (transactionResponse["pp_BillReference"] != null)
                        {
                            objResponse.BillReference = transactionResponse["pp_BillReference"].ToString();


                        }
                        else
                        {
                            objResponse.BillReference = string.Empty;
                        }
                        if (transactionResponse["pp_TxnCurrency"] != null)
                        {
                            objResponse.txtCurrency = transactionResponse["pp_TxnCurrency"].ToString();
                        }
                        else
                        {
                            objResponse.txtCurrency = string.Empty;
                        }
                        if (transactionResponse["pp_TxnDateTime"] != null)
                        {
                            objResponse.txtDateTime = transactionResponse["pp_TxnDateTime"].ToString();
                        }
                        else
                        {
                            objResponse.txtDateTime = string.Empty;
                        }

                        if (transactionResponse["pp_ResponseCode"] != null)
                        {
                            objResponse.ResponseCode = transactionResponse["pp_ResponseCode"].ToString();
                        }
                        else
                        {
                            objResponse.ResponseCode = string.Empty;
                        }

                        if (transactionResponse["pp_ResponseMessage"] != null)
                        {
                            objResponse.ResponseMessage = transactionResponse["pp_ResponseMessage"].ToString();
                        }
                        else
                        {
                            objResponse.ResponseMessage = string.Empty;
                        }

                        if (transactionResponse["pp_RetreivalReferenceNo"] != null)
                        {
                            objResponse.RetreivalReferenceNo = transactionResponse["pp_RetreivalReferenceNo"].ToString();
                        }
                        else
                        {
                            objResponse.RetreivalReferenceNo = string.Empty;
                        }
                        if (transactionResponse["pp_AuthCode"] != null)
                        {
                            objResponse.AuthCode = transactionResponse["pp_AuthCode"].ToString();
                        }
                        else
                        {
                            objResponse.AuthCode = string.Empty;
                        }

                        if (transactionResponse["pp_BankID"] != null)
                        {
                            objResponse.BankId = transactionResponse["pp_BankID"].ToString();
                        }
                        else
                        {
                            objResponse.BankId = string.Empty;
                        }

                        if (transactionResponse["pp_CustomerCardNo"] != null)
                        {
                            objResponse.CustomerCardNo = transactionResponse["pp_CustomerCardNo"].ToString();
                        }
                        else
                        {
                            objResponse.CustomerCardNo = string.Empty;
                        }

                        if (transactionResponse["pp_CustomerIBAN"] != null)
                        {
                            objResponse.CustomerCardNo = transactionResponse["pp_CustomerIBAN"].ToString();
                        }
                        else
                        {
                            objResponse.CustomerCardNo = string.Empty;
                        }

                        if (transactionResponse["pp_SecureHash"] != null)
                        {
                            objResponse.SecureHash = transactionResponse["pp_SecureHash"].ToString();
                        }
                        else
                        {
                            objResponse.SecureHash = string.Empty;
                        }
                        if (transactionResponse["pp_SettlementExpiry"] != null)
                        {
                            objResponse.SettlementExpiry = transactionResponse["pp_SettlementExpiry"].ToString();// transactionResponse["pp_SettlementExpiry"].ToString();
                        }
                        else
                        {
                            objResponse.SettlementExpiry = string.Empty;
                        }
                        if (transactionResponse["pp_ProductID"] != null)
                        {
                            objResponse.ProductID = transactionResponse["pp_ProductID"].ToString();
                        }
                        else
                        {
                            objResponse.ProductID = string.Empty;
                        }
                        if (transactionResponse["ppmbf_1"] != null)
                        {
                            objResponse.ppmbf_1 = transactionResponse["ppmbf_1"].ToString();
                        }
                        else
                        {
                            objResponse.ppmbf_1 = string.Empty;
                        }

                        if (transactionResponse["ppmbf_2"] != null)
                        {
                            objResponse.ppmbf_2 = transactionResponse["ppmbf_2"].ToString();
                        }
                        else
                        {
                            objResponse.ppmbf_2 = string.Empty;
                        }

                        if (transactionResponse["ppmbf_3"] != null)
                        {
                            objResponse.ppmbf_3 = transactionResponse["ppmbf_3"].ToString();
                        }
                        else
                        {
                            objResponse.ppmbf_3 = string.Empty;
                        }

                        if (transactionResponse["ppmbf_4"] != null)
                        {
                            objResponse.ppmbf_4 = transactionResponse["ppmbf_4"].ToString();
                        }
                        else
                        {
                            objResponse.ppmbf_4 = string.Empty;
                        }

                        if (transactionResponse["ppmbf_5"] != null)
                        {
                            objResponse.ppmbf_5 = transactionResponse["ppmbf_5"].ToString();
                        }
                        else
                        {
                            objResponse.ppmbf_5 = string.Empty;
                        }


                        objResponse.MethodName = "PGPaymentResponse";
                        Logger.LogError("objResponse at if" + JsonConvert.SerializeObject(objResponse));

                        try
                        {
                            Confirmation = await NOCService.CreateResponseForOrderIdAsync(objResponse, "PGPaymentResponse");
                            Logger.LogError("Confirmation" + JsonConvert.SerializeObject(Confirmation));

                            string XmlStr = rawHashData;
                            await NOCService.UpdateUaePgsResponseLogAsync(new ResponseLog
                            {
                                LogId = logId,
                                pp_ResponseCode = objResponse.ResponseCode,
                                pp_ResponseMessage = objResponse.ResponseMessage,
                                pp_SecureHash = objResponse.SecureHash,
                                ReturnString = XmlStr
                            });
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("UpdateUaePgsResponseLogAsync Exception" + JsonConvert.SerializeObject(ex));

                        }


                    }
                    else
                    {
                        ErrorMessage = L["ErrorMessageCallBack"];
                    }
                    #endregion                    
                }
                catch (Exception ex)
                {

                }
            }

        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

    }
}
