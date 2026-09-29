using Blazorise;
using DIP.Blazor.Pages.Site.NocForm;
using DIP.Blazor.Shared;
using DIP.CCPayment;
using DIP.EServices;
using DIP.Interface;
using DIP.LastEventss;
using DIP.PageInfos;
using DIP.SupportedBanks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;
using DIP.SoapServices;
using System.Text.RegularExpressions;


namespace DIP.Blazor.Pages.Site.PaymentProcess
{
    public partial class PaymentProcess
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;
        #region StepProgress
        private int currentStep = 4;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISupportedBanksAppService SupportedBanksAppService { get; set; }
        private OnlinePaymentDto OnlinePaymentDto { get; set; }

        public List<OnlinePaymentDto> listOnlinePayment = new List<OnlinePaymentDto>();
        //private Validations OnlinePaymentValidations { get; set; } = new();
        //private EditContext? EditContext;

        public List<SupportedBankFrontEnd> SupportedBankFrontEnds = new List<SupportedBankFrontEnd>();
        private ProcessingPage checkProcessingPage;
        private PaymentHistory[] getPaymentHistory;
        private bool disableConfirm;

        //private ValidationMessageStore? ValidationMessageStore { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        public bool IsNOC { get; set; } = true;
        public bool isAgree { get; set; }
        public bool hasCancel { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public ICCPaymentHelpers ICcPaymentHelpers { get; set; }
        [Inject]
        public IDIPAppsettingService IdIPAppsettingService { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        public List<Order> Matrix { get; private set; }
        public string OrderName { get; private set; }
        public string OrderId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public ClsRegistration info { get; private set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public int paymentMethodValue { get; set; } = 1;
        public CCPaymentConstant CCPaymentConstant { get; set; } = new();
        public PaymentInitiationRequest PaymentInitiationRequest { get; set; }
        public CCPaymentConfigValues CCPaymentConfigValues { get; set; }
        private string transactionRef { get; set; } = string.Empty;

        public List<Order> selectedpayments = new List<Order>();

        public ClsEOMinMaxAmountValueDTO EOMinMaxAmountValueDTO { get; set; } = new ClsEOMinMaxAmountValueDTO();
        public string Errormessage { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;

        [Inject]
        IDistributedCache Cache { get; set; }

        public PaymentProcess()
        {
            isAgree = false;
        }

        private void EditContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            //ValidationMessageStore?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);

            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);


            OnlinePaymentDto = new OnlinePaymentDto();
            GetSupportedBanksInput supportedBanksInput = new GetSupportedBanksInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            SupportedBankFrontEnds = await SupportedBanksAppService.GetListFrontEndAsync(supportedBanksInput);


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

            await PrepareData();
            await PrepareCardPaymentFormDataAsync();
        }

        private async Task PrepareData()
        {
            var checkRegisterTypeIsNoc = await NOCService.CheckRegisterTypeIsNOCAsync(ReferenceNumber);

            if (checkRegisterTypeIsNoc)
            {
                IsNOC = true;
            }
            else
            {
                IsNOC = false;
            }



            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber.Trim());
            if (!status.StatusName.Equals("PaymentProcess"))
            {
                switch (status.StatusName)
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
                    case "PendingAuth":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocSummary/{encryptedReferenceNumber}");
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




            info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            getPaymentHistory = await NOCService.GetPaymentHistoryAsync(ReferenceNumber);
            EOMinMaxAmountValueDTO = await NOCService.EOGETMinAndMaxAmountValueByRefno(ReferenceNumber);
            if (getPaymentHistory != null)
            {
                foreach (PaymentHistory his in getPaymentHistory)
                {
                    if (his.PayStatus.Equals("Authorization Pending"))
                    {
                        disableConfirm = true;
                    }
                }
            }
            else
            {
                disableConfirm = false;
            }



            List<Order> selectedpayments = new List<Order>();
            selectedpayments = (await NOCService.GetOrderDetailsAsync(ReferenceNumber, "0")).ToList();


            if (selectedpayments.Count > 0)
            {
                Matrix = selectedpayments;
                OrderName = "DIPNE" + Convert.ToString(selectedpayments[0].OrderId);
                OrderId = Convert.ToString(selectedpayments[0].OrderId);
                TotalAmount = selectedpayments[0].TotalAmount;
            }
        }

        private async Task<Task> PrepareCardPaymentFormDataAsync()
        {
            Utilities objUtil = new();
            selectedpayments = (await NOCService.GetOrderDetailsAsync(ReferenceNumber, "0")).ToList();
            transactionRef = Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetNumber()) + Convert.ToString(objUtil.GetLetter()).ToUpper() + Convert.ToString(objUtil.GetLetter()).ToUpper();
            //decimal pp_Amount = Convert.ToDecimal(float.Parse(Convert.ToString(10)) * 100);
            decimal pp_Amount = Convert.ToDecimal(selectedpayments[0].TotalAmount);
            string buildingName = SanitizeCompanyNameForCCAvenue(info.CompanyName);
            PaymentInitiationRequest = new PaymentInitiationRequest
            {
                OrderId = transactionRef,
                Amount = pp_Amount,//selectedpayments[0].TotalAmount,
                BillingName = buildingName.Length > 14 ? buildingName.Substring(0, 14) : buildingName,
                BillingEmail = info.Email,
                BillingTel = info.MobileNo,
                BillingNotes = info.RefNo,
            };
            CCPaymentConfigValues = await IdIPAppsettingService.GetCardPaymentConfigurations();
            CCPaymentConstant = await ICcPaymentHelpers.BuildCcPaymentFormDataAsync(PaymentInitiationRequest);

            return Task.CompletedTask;
        }

        public async void HandleDebitCardPayment()
        {
            if (!isAgree)
            {
                IsError = true;
                Errormessage = "Please agree to the terms and conditions before proceeding.";
                return;
            }

            if (PaymentInitiationRequest.Amount < EOMinMaxAmountValueDTO.MinimumCCAmount)
            {
                IsError = true;
                Errormessage = "The minimum amount for card payments is " + EOMinMaxAmountValueDTO.MinimumCCAmount + " AED." +
                    "Currently you cannot proceed with the card payment. Please use Internet Banking";
                return;
            }
            if (PaymentInitiationRequest.Amount > EOMinMaxAmountValueDTO.MaximumCCAmount)
            {
                IsError = true;
                Errormessage = "The maximum amount for card payments is " + EOMinMaxAmountValueDTO.MaximumCCAmount + " AED." +
                    "Currently you cannot proceed with the card payment. Please use Internet Banking";
                return;
            }
            Utilities objUtil = new();

            string Transactiondate = DateTime.Now.ToString("yyyyMMddHHmmss");
            CCPaymentCacheDTO CPaymentCache = new()
            {
                BillReference = "DIPNE" + Convert.ToString(selectedpayments[0].OrderId),
                TxnRefNo = transactionRef,
                MerchantId = CCPaymentConfigValues.MerchantId,
                Language = "en",
                ReferenceNumber = encryptedReferenceNumber
            };
            await Cache.SetStringAsync(transactionRef, JsonConvert.SerializeObject(CPaymentCache),
                      new DistributedCacheEntryOptions
                      {
                          AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                      });

            await NOCService.CreateTransactionRefForOrderIdAsync(new TransactionRequestInfo
            {
                ReferenceNumber = ReferenceNumber,
                OrderId = Convert.ToString(selectedpayments[0].OrderId),
                MerchantId = CCPaymentConfigValues.MerchantId,
                TransactionRefNo = transactionRef,//pp_TxnRefNo,
                TransactionType = "CC",//objUtil.PGSTranType,//pp_TxnType,
                TransactionDateTime = Transactiondate,
                BillReference = "DIPNE" + Convert.ToString(selectedpayments[0].OrderId), //pp_BillReference,
                Description = "Payment for NOC and Subleasing",
                SecureHash = "Need to fix",//CCPaymentConstant.EncRequest,
                TotalAmount = Convert.ToInt64((PaymentInitiationRequest.Amount * 100)).ToString()
            });

            var requestLog = new RequestLog
            {
                XMLRequest = "",
                ConfirmType = "",
                MethodName = "PGPaymentRequest",
                pp_RetrivalReferenceNo = ReferenceNumber,
                pp_SecureHash = CCPaymentConstant.EncRequest,
                pp_TxnDateTime = Transactiondate,
                pp_TxnRefNo = PaymentInitiationRequest.OrderId,
                RequestedBy = "Customer",
                RequestedTime = DateTime.Now,
                RequestedTimeSpecified = true
            };

            await NOCService.InsertUaePgsRequestLogAsync(requestLog);

            await JS.InvokeVoidAsync("eval", @"document.getElementById('ccpaymentform')?.submit();");
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("dip_modal_popup", null);
        }
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private void AddService()
        {

        }
        private void CheckboxChanged(ChangeEventArgs e)
        {
            // get the checkbox state
            var value = e.Value;
            isAgree = (bool)e.Value;

        }


        private async Task UploadDepostSlip()
        {
            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/PayslipReason/{encryptedReferenceNumber}");

        }

        private void RemovePaymentService(OnlinePaymentDto deletePayment)
        {

            listOnlinePayment.Remove(deletePayment);
        }

        private void CancelSubmit()
        {
            hasCancel = true;
        }

        // Add this method to sanitize CompanyName from special characters not allowed by CCAvenue
        private static string SanitizeCompanyNameForCCAvenue(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return companyName;

            // CCAvenue allows only: alphanumeric characters and spaces
            var sanitized = System.Text.RegularExpressions.Regex.Replace(
                companyName,
                @"[^a-zA-Z0-9\s]",
                string.Empty
            );

            return sanitized.Trim();
        }

    }
}
