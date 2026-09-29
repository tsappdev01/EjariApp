using DIP.Blazor.Pages.Site.NocForm;
using DIP.Blazor.Shared;
using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using DIP.SupportedBanks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Nito.AsyncEx;
using NUglify.JavaScript;
using Scriban.Syntax;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;

namespace DIP.Blazor.Pages.Site.EjariPayment
{
    public partial class EjariPayment
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        #region StepProgress
        private int currentStep = 4;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion

        private EditContext? EditContextEjariDetails;

        public NocDetailsDto NocDetailsDto { get; set; }

        private ValidationMessageStore? ValidationMessageStoreEjariDetails { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public bool hasCancel { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        private string encryptedReferenceNumber;

        [Inject]
        public ISupportedBanksAppService SupportedBanksAppService { get; set; }
        public ClsRegistration Info { get; set; }
        public Payments[] Matrix { get; set; }

        public List<SupportedBankFrontEnd> SupportedBankFrontEnds = new List<SupportedBankFrontEnd>();
        private PaymentHistory[] getPaymentHistory;

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        private double TotalAmount;
        private List<string> PaymentId = new List<string>();

        private void OnSubmit()
        {
            var test = NocDetailsDto;
            var t = test;
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
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }

            await PrepareData();
        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private async Task onPaySubmit()
        {
            string strIds = string.Empty;
            if (PaymentId.Count() > 0)
            {
                string tAmount = Convert.ToString(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", TotalAmount));
                strIds = string.Empty;
                foreach (string str in PaymentId)
                {
                    if (str.All(char.IsDigit))
                    {
                        strIds = strIds + "," + str;
                    }
                }
                if (!string.IsNullOrEmpty(strIds))
                {
                    var tt = strIds.Substring(1);
                    string OrderId = await NOCService.CreateOrderIdAsync(ReferenceNumber, "0", tAmount, strIds.Substring(1));
                    await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                }
            }
        }



        private void CancelSubmit()
        {
            hasCancel = true;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            GetSupportedBanksInput supportedBanksInput = new GetSupportedBanksInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            SupportedBankFrontEnds = await SupportedBanksAppService.GetListFrontEndAsync(supportedBanksInput);
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("dip_modal_popup", null);

        }

        private async Task OnRadioChanged(ChangeEventArgs e, Payments payments)
        {
            return;
            var value = e.Value;
            if (value != null && (bool)value)
            {
                TotalAmount = TotalAmount + Decimal.ToDouble(payments.ChargeAmount);
                PaymentId.Add(payments.PaymentId.ToString());
            }
            else
            {
                TotalAmount = TotalAmount - Decimal.ToDouble(payments.ChargeAmount);
                PaymentId.Remove(payments.PaymentId.ToString());


            }

        }
        private async Task PrepareData()
        {
            {
                var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);

                if (!status.StatusName.Equals("Payment"))
                {
                    switch (status.StatusName)
                    {
                        case "Registered":
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                            break;
                        case "Verified":
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocBasicForm/{encryptedReferenceNumber}");
                            break;
                        case "Upload":
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}");
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



                Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);

                getPaymentHistory = await NOCService.GetPaymentHistoryAsync(ReferenceNumber);

                Matrix = await NOCService.GetPaymentsMatrixAsync(ReferenceNumber);
                foreach (var matrix in Matrix)
                {
                    TotalAmount = TotalAmount + Decimal.ToDouble(matrix.ChargeAmount);
                    PaymentId.Add(matrix.PaymentId.ToString());
                }
            }

        }
    }
}
