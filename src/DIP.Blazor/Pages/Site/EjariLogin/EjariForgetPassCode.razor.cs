using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace DIP.Blazor.Pages.Site.EjariLogin
{
    public partial class EjariForgetPassCode
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }


        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        private EditContext? EditContextRegistration;
        private EditContext? EditContextForget;

        private RegistrationDto RegistrationDto { get; set; }
        private ForgetDto ForgetDto { get; set; }


        private ValidationMessageStore? ValidationMessageStoreForget { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public string RegisterMessageError { get; set; }
        public string DoneMessage { get; set; }
        public bool hasError { get; set; }
        public bool hasForgetError { get; set; }
        public bool resetDone { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        public IEnumerable<CountryItem> CountryItems = [];
        public List<SelectListItem> Issuers = new List<SelectListItem>();
        public List<SelectListItem> Categories = new List<SelectListItem>();
        private List<SelectListItem> Directions = new List<SelectListItem>();

        //private Captcha captchaComponent { get; set; }
        //private GoogleReCaptachDto GoogleReCaptachDto { get; set; } = null;
        //private CheckCaptcha CheckCaptcha { get; set; }
        private bool DisableButton = false;
        private bool isCaptchaValid = false;
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public EjariForgetPassCode()
        {
            // Service initialization handled by dependency injection
        }

        private async Task OnClickForget()
        {
            DisableButton = true;
            StateHasChanged();
            try
            {
                if (!isCaptchaValid)
                {
                    return;
                }

                var contact = !ForgetDto.ChangeContact ? ForgetDto.CountryCode + '-' + ForgetDto.MobileNumber : ForgetDto.Email;
                var result = await NOCService.ForgotPassCodeAsync(ForgetDto.ReferenceNumber, contact);
                if (result)
                {
                    resetDone = true;
                    hasForgetError = false;
                    DoneMessage = @L["AnEmailHasBeenSentPleaseCheckForDetails"];
                }
                else
                {
                    hasForgetError = true;
                    resetDone = false;
                    RegisterMessageError = !ForgetDto.ChangeContact ? @L["ReferenceNumberMobileIsNotValid"] : @L["ReferenceNumberEmailIsNotValid"];
                }
            }
            finally
            {
                DisableButton = false;
                StateHasChanged();
            }
        }



        //private void EditContextRegistration_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        //{
        //    ValidationMessageStoreRegistration?.Clear();
        //}

        private void EditContextForget_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            hasForgetError = false;
            resetDone = false;
            ValidationMessageStoreForget?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {

            ForgetDto = new ForgetDto();
            CountryItems = CountryList.Countries;

            EditContextForget = new(ForgetDto);
            ValidationMessageStoreForget = new ValidationMessageStore(EditContextForget);
            EditContextForget.OnValidationRequested += EditContextForget_OnValidationRequested;

            GetEServicesInput getEServicesInput = new()
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
            await PopulateIssuers();
            await PopulateDirections();
        }


        private async Task Navigat()
        {
            NavigationManager.NavigateTo($"{CultureInfo.CurrentCulture.Name}/Ejari");
        }


        public async Task PopulateIssuers()
        {
            var issuersResult = await NOCService.GetTradeLicenseIssuersAsync();
            Issuers = (from d in issuersResult
                       select new SelectListItem()
                       {
                           Text = d.IssuerName,
                           Value = d.Issuerid.ToString()
                       }).ToList();
        }


        public async Task PopulateDirections()
        {
            var directionsResult = await NOCService.GetDirectionsAsync();
            Directions = (from d in directionsResult
                          select new SelectListItem()
                          {
                              Text = d,
                              Value = d
                          }).ToList();
        }

        private void OnSuccessCaptchaForget(bool isValid)
        {
            isCaptchaValid = isValid;
            if (isValid)
            {
                ForgetDto.Captcha = "DONE";
                if (EditContextForget != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextForget.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextForget.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }
            else
            {
                ForgetDto.Captcha = null;
            }

        }

        private void OnExpiredCaptchaForget()
        {
            //ForgetDto.Captcha = string.Empty;
            StateHasChanged();
        }

    }
}
