using Blazorise;
using DIP.Blazor.Pages.Site.NocForm;
using DIP.Blazor.Shared;
using DIP.EServices;
using DIP.LastEventss;
using DIP.PageInfos;
using DIP.SoapServices;
using DIP.SupportedBanks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;


namespace DIP.Blazor.Pages.Site.PaymentProcess
{
    public partial class CreditDebitProess
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
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        public CreditDebitProess()
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

            GetSupportedBanksInput supportedBanksInput = new GetSupportedBanksInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };


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

        }

        private async Task PrepareData()
        {
            IsNOC = await NOCService.CheckRegisterTypeIsNOCAsync(ReferenceNumber);

            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber.Trim());
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

            info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
    }
}
