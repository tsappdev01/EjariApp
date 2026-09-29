using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using StgDipService;
using DIP.SoapServices;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Globalization;
using DIP.PageInfos;
using System.Linq;

namespace DIP.Blazor.Pages.Site.PaySlipReason
{
    public partial class PaySlipReason
    {
        private string encryptedReferenceNumber;

        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }


        public NocDetailsDto NocDetailsDto { get; set; }

        public string ValidateTradeLicenseNo { get; set; }
        public bool hasError { get; set; }


        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public ClsRegistration Info { get; private set; }
        public ArrayOfKeyValueOfstringstringKeyValueOfstringstring[] lstDictsReasons { get; private set; }
        private string ReasonUploadingPayment { get; set; }
        private string ValidateMessage { get; set; }

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        private async Task OnSubmit()
        {
            if (ReasonUploadingPayment == null || ReasonUploadingPayment.IsNullOrEmpty())
            {
                hasError = true;
                ValidateMessage = @L["PleaseSelectTheReason"];

            }
            else
            {
                await NOCService.InsertPaymentSlipReasonsAsync(ReferenceNumber, Convert.ToString(ReasonUploadingPayment));

                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/UploadPayslip/{encryptedReferenceNumber}");

            }
        }

        private async Task OnCancelClick()
        {

            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber.Trim());
            if (status.StatusName.Equals("Payment"))
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");

            }
            else
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
            }
        }



        protected override async Task OnInitializedAsync()
        {
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);

            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);


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


        private async Task PrepareData()
        {
            lstDictsReasons = await NOCService.GetPaymentSlipReasonsAsync();
            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
        }


    }
}
