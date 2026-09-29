using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using StgDipService;
using SweetAlertBlazor;
using System.Globalization;

namespace DIP.Blazor.Pages.Site.EjariLogin
{
    public partial class EjariRenewal
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Parameter]
        public string Flag { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private EditContext? EditContextRenewal;

        private RenewalDTO RenewalDTO { get; set; }

        private ValidationMessageStore? ValidationMessageStoreRenewal { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public string RegisterMessageError { get; set; }
        public string DoneMessage { get; set; }
        public bool hasError { get; set; }
        public bool hasForgetError { get; set; }
        public bool resetDone { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public List<SelectListItem> Issuers = [];
        public List<SelectListItem> Categories = [];
        private List<SelectListItem> Directions = [];

        private bool DisableButton = false;
        private bool isCaptchaValid = false;
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        public EjariRenewal()
        {
            // Service initialization handled by dependency injection
        }

        private async Task OnClickRenewal()
        {
            DisableButton = true;
            if (isCaptchaValid == false)
            {
                DisableButton = false;
                return;
            }
            DisableButton = false;

            var response = await NOCService.EoGetEjariRefNoAsync(RenewalDTO.EjariContractNo, RenewalDTO.EmailAddress);
            if (response.Length > 0)
            {
                ClsEoGetEjariRefByContractNoResDTO datas = response[0];
                string renewalrefNo = string.Empty;
                //string renewalResponse = string.Empty;
                if (datas == null)
                {

                    hasForgetError = true;
                    RegisterMessageError = "The entered details are invalid. Please provide a valid Ejari Contract No and registered Email Address.";
                    return;
                }
                datas.Email = RenewalDTO.EmailAddress;
                switch (datas.Datasource)
                {
                    case "MaintainPropertySubLeased/MaintainSubleasedTenants":
                        //renewalResponse = EncryptionHelper.EncryptUrlSafe(JsonConvert.SerializeObject(datas));
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/{renewalResponse}");
                        hasForgetError = true;
                        RegisterMessageError = "We found your record, but it is currently registered as a walk-in registration. To renew your walk-in registration, please use the Ejari Register page.";
                        break;
                    case "MaintainProperty/MaintainTenants":
                        renewalrefNo = await NOCService.UpdateNocApplicationRenewalAsync(datas.RefNo);
                        if (renewalrefNo != "0")
                        {
                            hasForgetError = false;
                            var swalAlertModel = new SwalModel($"Your NOC application renewal was commenced." +
                                $" Your new reference number and passcode has been sent to your email address ({MaskEmail(datas?.Email ?? string.Empty)}). " +
                                $"Please use the new reference number and passcode for all future correspondence.", "")
                                        .WithIcon(SweetAlert.Icon.Info) // Other Icons are Success, Error and Warning
                                        .WithButton(SweetAlert.Button.Ok())
                                        .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                            await InvokeAsync(StateHasChanged);

                            await JS.ShowSwalAsync(swalAlertModel);

                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari");
                        }
                        else
                        {
                            hasForgetError = true;
                            RegisterMessageError = "System couldn't find relevant existing record for your contract ID. Please contact DIPark support.";
                            return;
                        }
                        break;
                    default:
                        hasForgetError = true;
                        RegisterMessageError = "System couldn't find relevant existing record for your contract ID. Please contact DIPark support.";
                        return;
                }
            }
            else
            {

                hasForgetError = true;
                RegisterMessageError = "The entered details are invalid. Please provide a valid Ejari Contract No and registered Email Address.";
            }
        }

        private void EditContextRenewal_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            hasForgetError = false;
            ValidationMessageStoreRenewal?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            RenewalDTO = new RenewalDTO();

            EditContextRenewal = new(RenewalDTO);
            ValidationMessageStoreRenewal = new ValidationMessageStore(EditContextRenewal);
            EditContextRenewal.OnValidationRequested += EditContextRenewal_OnValidationRequested;

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
        }


        private async Task NavigatToLogin()
        {
            NavigationManager.NavigateTo($"{CultureInfo.CurrentCulture.Name}/Ejari");
        }
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private void OnSuccessCaptchaRenewal(bool isValid)
        {
            isCaptchaValid = isValid;
            if (isValid)
            {
                RenewalDTO.Captcha = "DONE";
                if (EditContextRenewal != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextRenewal.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextRenewal.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }
            else
            {
                RenewalDTO.Captcha = null;
            }

        }

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

        private void OnExpiredCaptchaForget()
        {
            RenewalDTO.Captcha = string.Empty;
            StateHasChanged();
        }
    }
}
