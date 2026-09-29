using DIP.Connected_Services.DocumentIntelligent;
using DIP.EServices;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StgDipService;
using SweetAlertBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;


namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocDashBoard
    {
        [Parameter]
        public string Lang { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        #region StepProgress
        private int currentStep = 0;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion

        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        public ClsRegistration Info { get; private set; } = new ClsRegistration();
        public ClsRegistrationDetails Details { get; private set; } = new ClsRegistrationDetails();

        public string StatusName { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public string StatusNotes { get; set; } = string.Empty;
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        public string Errormessage { get; set; }

        public NocDashBoard()
        {
            // Initialization handled by DI
        }

        protected override async Task OnInitializedAsync()
        {
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);
            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            //GoogleReCaptachDto = await DIPSettingAppService.GetGoogleReCaptachSettingAsync();

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


        private async Task PrepareData()
        {
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            StatusName = status.StatusName;
            StatusMessage = status.Description;
            StatusNotes = status.Notes;
            await StatusSet();

            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            Details = await NOCService.GetRegistrationDetailsAsync(ReferenceNumber);
        }

        private async Task StatusSet()
        {
            switch (StatusName)
            {
                case "Registered":
                    await RegistrationVerificationusingLink();
                    break;
                case "Verified":
                    currentStep = 1;
                    break;
                case "Upload":
                    currentStep = 2;
                    break;
                case "ApplicantAcknowledgement":
                    currentStep = 3;
                    break;
                case "Payment":
                    currentStep = 4;
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> RegistrationVerificationusingLink()
        {
            var verificationResponse = await NOCService.EORegistrationLinkVerification(ReferenceNumber);
            if (verificationResponse)
            {
                var statusResponse = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
                StatusName = statusResponse.StatusName;
                StatusMessage = statusResponse.Description;
                StatusNotes = statusResponse.Notes;
                await StatusSet();
            }

            return verificationResponse;
        }

        #region Onclick
        private async void HandleRenewProcess()
        {
            string renewalrefNo = await NOCService.UpdateNocApplicationRenewalAsync(ReferenceNumber);
            if (renewalrefNo != "0")
            {
                var swalAlertModel = new SwalModel($"Your NOC application renewal was commenced." +
                    $" Your new reference number and passcode has been sent to your email address ({MaskEmail(Info?.Email ?? string.Empty)}). " +
                    $"Please use the new reference number and passcode for all future correspondence.", "")
                            .WithIcon(SweetAlert.Icon.Info) // Other Icons are Success, Error and Warning
                            .WithButton(SweetAlert.Button.Ok())
                            .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                await JS.ShowSwalAsync(swalAlertModel);

                var encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(renewalrefNo);

                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocBasicForm/{encryptedReferenceNumber}");
            }
            else
            {
                Errormessage = "Something Went Wrong.Please try again later.";
                return;
            }
        }
        #endregion

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return string.Empty;

            var parts = email.Split('@');
            var local = parts[0];
            var domain = parts[1];

            // Keep only last 4 chars before '@'
            var visibleCount = Math.Min(4, local.Length);
            var maskedPart = new string('*', Math.Max(0, local.Length - visibleCount));
            var visiblePart = local.Substring(local.Length - visibleCount);

            return maskedPart + visiblePart + "@" + domain;
        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
    }
}
