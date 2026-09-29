using DIP.EServices;
using DIP.PageInfos;
using DIP.PageInfoSections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using StgDipService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DIP.SoapServices;


namespace DIP.Blazor.Pages.Site.Disclaimer
{
    public partial class Disclaimer
    {
        private Dictionary<string, StringValues>? fields;
        SortedList transactionData = new SortedList(new VPCStringComparer());
        private string dbTransactionref;

        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        [Inject]
        public IOptions<SoapServicesConfiguration> SoapServicesOptions { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }





        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        [Inject]
        public IPageInfoSectionsAppService PageInfoSectionsAppService { get; set; }


        [Inject]
        public IEServicesAppService eServicesAppService { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }


        private HttpResponse HttpResponse { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        IHttpClientFactory ClientFactory { get; set; }

        //[Inject]
        //MyReferenceNumberContainer MyReferenceNumberContainer { get; set; }

        //[Inject]
        //ProtectedLocalStorage LocalStorage { get; set; }
        [Inject]
        IDistributedCache Cache { get; set; }
        string url;

        //[Inject]
        //MyLoginContainer myLoginContainer { get; set; }

        [Inject]
        ILogger<Disclaimer> _logger { get; set; }

        public Disclaimer()
        {
            fields = new Dictionary<string, StringValues>();
            url = "https://cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform";


            //For stg
            //url = "https://pgt.cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform";
            //url = "https://pgtservices.cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await LocalStorage.SetAsync("ReferenceNumber", ReferenceNumber);


        }

        protected override async Task OnInitializedAsync()
        {

            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);


            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);


            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("UaepgsGateway/Disclaimer");

            //MyReferenceNumberContainer.SetValue(ReferenceNumber);

            await PrepareData();
        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }


        private async Task onDeniedClick()
        {

            await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
        }



        private async Task PrepareData()
        {
            List<Order> selectedpayments = new List<Order>();
            string requestString = "";

            int LoggingId = 0;

            selectedpayments = (await NOCService.GetOrderDetailsAsync(ReferenceNumber, "0")).ToList();


            {
                Utilities objUtil = new Utilities(SoapServicesOptions.Value.NOCService.AppKey);
                string transactionRef = Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetNumber()) + Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetLetter()).ToUpper();
                string Transactiondate = DateTime.Now.ToString("yyyyMMddHHmmss");

                //
                //string pp_MerchantId = "Test5039015002";
                string pp_MerchantId = objUtil.MerchantId;

                //string pp_Password = ""; 
                string pp_Password = objUtil.MerchantPassword;

                string pp_TxnRefNo = transactionRef;

                await Cache.SetStringAsync(transactionRef, ReferenceNumber,
                       new DistributedCacheEntryOptions
                       {
                           AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                       });
                //string pp_Version = "1.1"; // dev and uat
                string pp_Version = objUtil.PGSVersion;  //Prod

                //string pp_Amount = Convert.ToString(Convert.ToInt32(float.Parse(Convert.ToString(10)) * 100));
                string pp_Amount = Convert.ToInt64(selectedpayments[0].TotalAmount * 100).ToString();
                string pp_TxnDateTime = Transactiondate;
                string pp_BillReference = "DIPNE" + Convert.ToString(selectedpayments[0].OrderId);

                //string pp_TxnType = "DD"; //Dev
                string pp_TxnType = objUtil.PGSTranType; //Prod

                //For stg env
                //string pp_Description = "Descriptionof transaction";
                string pp_Description = "Payment for NOC and Subleasing";
                //For stg env
                //string HASHKEY = "";
                string HASHKEY = objUtil.HASHKEY;

                //For stg env
                //fields.Add("pp_MerchantId", "Test5039015002");//Test5039015002
                //fields.Add("pp_Password", "3093058365");

                fields.Add("pp_MerchantId", objUtil.MerchantId);
                fields.Add("pp_Password", objUtil.MerchantPassword);
                fields.Add("pp_TxnRefNo", pp_TxnRefNo);
                fields.Add("pp_Version", pp_Version);
                fields.Add("pp_Amount", pp_Amount);
                fields.Add("pp_TxnDateTime", pp_TxnDateTime);
                fields.Add("pp_BillReference", pp_BillReference); //"billRef");
                fields.Add("pp_TxnType", pp_TxnType);
                fields.Add("pp_Description", pp_Description);
                fields.Add("HASHKEY", HASHKEY);
                //fields.Add("pp_ReturnURL", "https://localhost:44332/uaepgsgateway/PGResponse");
                //fields.Add("pp_ReturnURL", "https://uat.dipark.com/uaepgsgateway/PGResponse");
                fields.Add("pp_ReturnURL", "https://www.dipark.com/uaepgsGateway/PGResponse");

                fields.Add("pp_Language", "EN");
                fields.Add("pp_TxnCurrency", "AED");
                //fields.Add("pp_ProductID", "RETL");
                //fields.Add("pp_RetreivalReferenceNo", "1011040471601");

                fields.Add("pp_TxnExpiryDateTime", "-1");


                //fields.Add("ppmbf_1", "1");
                //fields.Add("ppmbf_2", "2");
                //fields.Add("ppmbf_3", "3");
                //fields.Add("ppmbf_4", "4");
                //fields.Add("ppmbf_5", "5");

                //fields.Add("ppmpf_1", "1");
                //fields.Add("ppmpf_2", "2");
                //fields.Add("ppmpf_3", "3");
                //fields.Add("ppmpf_4", "4");
                //fields.Add("ppmpf_5", "5");
                //fields.Add("pp_SubMerchantID", "");

                //fields.Add("pp_productID", "RETL");
                //fields.Add("pp_BankID", "0005");
                //fields.Add("pp_BankID", "");
                //fields.Add("pp_productID", "");
                FormCollection form = new FormCollection(fields);
                foreach (string item in form.Keys)
                {
                    if ((form[item] != "") && item.StartsWith("pp"))
                    {
                        transactionData.Add(item, form[item]);
                    }
                }

                string rawHashData = HASHKEY;
                string signatureSeparattor = "&";
                string seperator = "?";

                foreach (DictionaryEntry item in transactionData)
                {
                    if (HASHKEY.Length > 0)
                    {
                        if (!item.Key.ToString().Equals("pp_SecureHash"))
                        {
                            rawHashData += signatureSeparattor + item.Value.ToString();
                        }
                    }

                }


                string signature = string.Empty;
                if (HASHKEY.Length > 0)
                {
                    signature = objUtil.CreateHMACSha256Str(HASHKEY, rawHashData);
                }

                transactionData["pp_SecureHash"] = signature;

                _logger.LogError("Object Payment" + JsonConvert.SerializeObject(fields));
                _logger.LogError("Object Payment transactionData" + JsonConvert.SerializeObject(transactionData));
                _logger.LogError("Object Payment signature" + JsonConvert.SerializeObject(signature));

                await NOCService.CreateTransactionRefForOrderIdAsync(new TransactionRequestInfo
                {
                    ReferenceNumber = ReferenceNumber,
                    OrderId = Convert.ToString(selectedpayments[0].OrderId),
                    MerchantId = pp_MerchantId,
                    TransactionRefNo = pp_TxnRefNo,
                    TransactionType = pp_TxnType,
                    TransactionDateTime = pp_TxnDateTime,
                    BillReference = pp_BillReference,
                    Description = pp_Description,
                    SecureHash = signature,
                    TotalAmount = pp_Amount
                });

                dbTransactionref = pp_TxnRefNo;
                var RawData = "";

                // Comment the required below mentioned.
                // QA  URL's
                //  string queryString = "https://pgt.cbuaepay.ae/PGCustomerPortalBkp/transactionmanagement/merchantform";

                //string url = "https://cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform";

                //string url = "https://url.uk.m.mimecastprotect.com/s/UIkZCYQ7lF62PNPhMtWix4oJB?domain=pgt.cbuaepay.ae";
                // PRODUCTION URLS
                // string url = "https://cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform";
                string queryString = "https://cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform"; //live
                //string queryString = "https://pgt.cbuaepay.ae/PGCustomerPortal/transactionmanagement/merchantform"; //Stg



                if (dbTransactionref.Equals(transactionData["pp_TxnRefNo"].ToString()))
                {
                    foreach (DictionaryEntry item in transactionData)
                    {
                        RawData += item.Key.ToString() + "=" + item.Value.ToString() + "<br/>";

                        queryString += seperator + HttpUtility.UrlEncode(item.Key.ToString()) + "=" +
                                       HttpUtility.UrlEncode(item.Value.ToString());
                        seperator = "&";
                    }

                    // Created response for redirection

                    var requestLog = new RequestLog { XMLRequest = requestString, ConfirmType = "", MethodName = "PGPaymentRequest", pp_RetrivalReferenceNo = ReferenceNumber, pp_SecureHash = signature, pp_TxnDateTime = pp_TxnDateTime, pp_TxnRefNo = pp_TxnRefNo, RequestedBy = "Customer", RequestedTime = DateTime.Now, RequestedTimeSpecified = true };

                    LoggingId = await NOCService.InsertUaePgsRequestLogWithResultAsync(requestLog);
                    //myLoginContainer.SetValue(LoggingId);
                    var key = "LoggingId" + transactionRef;
                    await Cache.SetStringAsync(key, LoggingId.ToString(),
                 new DistributedCacheEntryOptions
                 {
                     AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                 });

                    //await LocalStorage.SetAsync("LoggingId", LoggingId);

                }
            }
        }

    }
}
