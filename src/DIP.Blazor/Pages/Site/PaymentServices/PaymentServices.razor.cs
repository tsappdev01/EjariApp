using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.LastEventss;
using System.Collections.Generic;
using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using DIP.SupportedBanks;
using StgDipService;
using DIP.SoapServices;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using DIP.Blazor.Shared;
using Newtonsoft.Json;
using DotLiquid.Util;
using System.Web;
using DIP.PageInfos;
using DIP.Settings;
using Blazorise;
using Microsoft.Extensions.Logging;
using Azure;

namespace DIP.Blazor.Pages.Site.PaymentServices
{
    public partial class PaymentServices
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public ISupportedBanksAppService SupportedBanksAppService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }


        [Inject]
        NavigationManager NavigationManager { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        public List<SupportedBankFrontEnd> SupportedBankFrontEnds = new List<SupportedBankFrontEnd>();


        private OnlinePaymentDto OnlinePaymentDto { get; set; }


        private ServiceForPaymentDto ServiceForPayment { get; set; }
        public List<ServiceForPaymentDto> ListServiceForPayment = new List<ServiceForPaymentDto>();
        //private Validations OnlinePaymentValidations { get; set; } = new();
        private EditContext? EditContext = default!;
        private EditContext? ServicePaymentEditContext;

        private ValidationMessageStore? ValidationMessageStore = default!;
        private ValidationMessageStore? ValidationMessageStoreServicePaymentEditContext { get; set; }
        private string ValidateServicePayment { get; set; }
        private string errorMessage { get; set; }
        private bool hasError { get; set; }
        private bool hasErrorMessage { get; set; }


        [Inject]
        public INOCServiceWrapper NOCService { get; set; }


        private List<SelectListItem> Directions = new List<SelectListItem>();
        private List<Payments> otherCharges = new List<Payments>();
        private List<MY> listOfYear = new List<MY>();
        private List<MY> listOfMonth = new List<MY>();

        private double totalAmount;

        //private Captcha captchaComponent { get; set; }
        //private GoogleReCaptachDto GoogleReCaptachDto { get; set; } = null;
        //private CheckCaptcha CheckCaptcha { get; set; }
        private bool DisableButton = false;

        private ServiceForPaymentCaptchaDto ServiceForPaymentCaptchaDto { get; set; }
        private EditContext? ServicePaymentCaptchaEditContext;
        private ValidationMessageStore? ValidationMessageStoreServicePaymentCaptchaEditContext { get; set; }


        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        [Inject]
        ILogger<PaymentServices> _logger { get; set; }
        private bool isCaptchaValid = false;
        public PaymentServices()
        {
        }


        public async Task PopulateDirections()
        {
            var directions = await NOCService.GetDirectionsAsync();
            Directions = (from d in directions
                          select new SelectListItem()
                          {
                              Text = d,
                              Value = d
                          }).ToList();


        }
        private void EditContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStore?.Clear();
        }

        private void ServicePaymentEditContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreServicePaymentEditContext?.Clear();
        }
        private void ServicePaymentCaptchaEditContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreServicePaymentCaptchaEditContext?.Clear();
        }
        protected override async Task OnInitializedAsync()
        {

            OnlinePaymentDto = new OnlinePaymentDto();
            EditContext = new(OnlinePaymentDto);
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += EditContext_OnValidationRequested;



            ServiceForPayment = new ServiceForPaymentDto();
            ServicePaymentEditContext = new(ServiceForPayment);
            ValidationMessageStoreServicePaymentEditContext = new ValidationMessageStore(ServicePaymentEditContext);
            ServicePaymentEditContext.OnValidationRequested += ServicePaymentEditContext_OnValidationRequested;


            ServiceForPaymentCaptchaDto = new ServiceForPaymentCaptchaDto();
            ServicePaymentCaptchaEditContext = new(ServiceForPaymentCaptchaDto);
            ValidationMessageStoreServicePaymentCaptchaEditContext = new ValidationMessageStore(ServicePaymentCaptchaEditContext);
            ServicePaymentCaptchaEditContext.OnValidationRequested += ServicePaymentCaptchaEditContext_OnValidationRequested;

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
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Online/PaymentServices"));
            }



            await PopulateDirections();
            otherCharges = await GetOtherCharges();

            listOfMonth = GetMonths();
            listOfYear = GetYears();

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
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }



        private void AddPaymentServices()
        {
            hasError = false;
            hasErrorMessage = false;
            //if(EditContext.Validate())
            //{
            ListServiceForPayment.Add(ServiceForPayment);
            ServiceForPayment = new ServiceForPaymentDto();
            ServicePaymentEditContext = new(ServiceForPayment);
            // }
            totalAmount = 0;
            foreach (var payment in ListServiceForPayment)
            {
                totalAmount = totalAmount + Double.Parse(payment.Amount);

            }


        }

        private async Task RemovePaymentServices(ServiceForPaymentDto serviceForPayment)
        {
            string message = L["ConfirmDeleteMessage"];
            bool confirmed = await JS.InvokeAsync<bool>("confirm", message);
            if (confirmed)
            {
                hasError = false;

                //if(EditContext.Validate())
                //{
                ListServiceForPayment.Remove(serviceForPayment);

                totalAmount = 0;
                foreach (var payment in ListServiceForPayment)
                {
                    totalAmount = totalAmount + Double.Parse(payment.Amount);

                }
            }




        }
        private async Task HandleAddServiceSubmit()
        {
            if ((await this.AddServiceValidate()))
            {

                //if (!(ListServiceForPayment != null && ListServiceForPayment.Count > 0))
                //{

                //    hasError = true;

                //    ValidateServicePayment = "Please Add Payment Services";
                //}
                if (ServiceForPayment != null && ServiceForPayment.Amount != null)
                {
                    var amount = float.Parse(ServiceForPayment.Amount);
                    if (amount > 0)
                    {
                        if (await this.AddServiceValidate())
                        {
                            AddPaymentServices();
                            // valid so do your valid stuff   
                        }
                    }

                }
            }


        }

        private async Task<bool> AddServiceValidate()
        {
            // run the standard Validation
            var valid = ServicePaymentEditContext.Validate();
            // clear our custom store
            ValidationMessageStoreServicePaymentEditContext.Clear();
            // Fake a database async call to check if the user exists
            // await Task.Delay(100);
            //Just for Testing trip validation
            //if (true)
            //{
            //    // log message to store and notify the edit context
            //    ValidationMessageStoreServicePaymentEditContext.Add(() => OnlinePaymentDto.Email, "Not Complex Enough");
            //    EditContext.NotifyValidationStateChanged();
            //    valid = false;
            //}
            return valid;
        }

        private async Task HandleSubmit()
        {
            _logger.LogError("HandleSubmit 1");

            DisableButton = true;
            if (!(await this.ValidateCaptcha()))
            {
                DisableButton = false;
                return;
            }

            DisableButton = false;
            var allString = OnlinePaymentDto.PropertyCode + "-" + OnlinePaymentDto.PropertyValue;
            var res = await NOCService.GetBuidlingNamesByPropertyAsync(allString);
            var count = res.ToList().Count;
            if (res != null && res.ToList().Count == 1)
            {
                hasError = true;


                ValidateServicePayment = L["PropertyValueNotValid"];
                return;
            }
            _logger.LogError("HandleSubmit after GetBuidlingNamesByPropertyAsync" + JsonConvert.SerializeObject(res));

            if (!(ListServiceForPayment != null && ListServiceForPayment.Count > 0))
            {

                hasError = true;


                ValidateServicePayment = L["PleaseAddPaymentServices"];
                return;
            }
            if (await this.Validate())
            {
                _logger.LogError("HandleSubmit after this.Validate");

                string referenceNumber = "0";

                try
                {
                    if (ListServiceForPayment != null && ListServiceForPayment.Count > 0)
                    {
                        _logger.LogError("HandleSubmit after ListServiceForPayment");

                        if (isCaptchaValid == false)
                        {
                            _logger.LogError("HandleSubmit in if return CheckCaptcha.CheckCaptchaResponse");

                            DisableButton = false;
                            return;
                        }
                        var clsReg = new ClsRegistration()
                        {
                            TypeId = 2,
                            TypeIdSpecified = true,
                            PropertyCode = $"{HttpUtility.HtmlEncode(OnlinePaymentDto.PropertyCode)}-{HttpUtility.HtmlEncode(OnlinePaymentDto.PropertyValue)}",
                            CompanyName = HttpUtility.HtmlEncode(OnlinePaymentDto?.TenantName),
                            MobileNo = HttpUtility.HtmlEncode(OnlinePaymentDto?.MobileNumber),
                            Email = HttpUtility.HtmlEncode(OnlinePaymentDto?.Email),
                            TradeLicenseIssuerId = 0,
                            TradeLicenseType = "",// model.IssuerType == "0" ? "Initial Approval" : "Trade License",
                            TradeLicenseNo = "", // model.IssuerType == "0" ? model.InitialApproval : model.TradeLicense,
                            TLExpiryDate = DateTime.Now,
                            RegisterType = 2,
                            TradeLicenseIssuerIdSpecified = true,
                            TLExpiryDateSpecified = true,
                            RegisterTypeSpecified = true,
                            ZExpired = false,
                            ZExpiredIn = 0,
                            ZzApplicationType = string.Empty
                        };




                        referenceNumber = await NOCService.RegistrationConfirmationAsync(clsReg);

                        _logger.LogError("HandleSubmit after referenceNumber" + JsonConvert.SerializeObject(referenceNumber));

                        totalAmount = 0;
                        foreach (var pay in ListServiceForPayment)
                        {
                            await NOCService.CreateOSPaymentsOrderAsync(referenceNumber, pay.paymentFor, pay.Amount, string.IsNullOrWhiteSpace(pay.Comments) ? "" : pay.Comments, string.IsNullOrWhiteSpace(pay.Month) ? "" : pay.Month, string.IsNullOrWhiteSpace(pay.Year) ? "" : pay.Year);
                        }
                        var createOsOrderResponse = await NOCService.CreateOSOrderIdAsync(referenceNumber);

                        if (referenceNumber == "0")
                        {
                            hasErrorMessage = true;
                            errorMessage = L["TheDetailsAreAlreadyRegistered"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    //PopulateDirections();
                }


                _logger.LogError("HandleSubmit after CheckCaptcha.CheckCaptchaResponse");

                var encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(referenceNumber);
                _logger.LogError("HandleSubmit after CheckCaptcha.CheckCaptchaResponse" + JsonConvert.SerializeObject(encryptedReferenceNumber));


                await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                // valid so do your valid stuff   
            }



        }

        private async Task<bool> Validate()
        {
            // run the standard Validation
            var valid = EditContext.Validate();
            // clear our custom store
            ValidationMessageStore.Clear();
            // Fake a database async call to check if the user exists
            await Task.Delay(100);

            return valid;
        }

        private async Task<bool> ValidateCaptcha()
        {

            // run the standard Validation
            var valid = ServicePaymentCaptchaEditContext.Validate();
            // clear our custom store
            ValidationMessageStoreServicePaymentCaptchaEditContext.Clear();
            // Fake a database async call to check if the user exists
            await Task.Delay(100);
            return valid;
        }

        public async Task<List<Payments>> GetOtherCharges()
        {
            var otherCharges = await NOCService.GetPaymentsMatrixAsync("OtherCharges");
            var result = (from os in otherCharges
                          select new Payments
                          {
                              AccCode = os.AccCode,
                              ChargeAmount = os.ChargeAmount,
                              Name = os.Name,
                              PaymentCode = os.PaymentCode,
                              TaxableAmount = os.TaxableAmount,
                              VatAmount = os.VatAmount,
                              PaymentId = os.PaymentId,
                              IsSelected = os.IsSelected
                          }).ToList();


            return result;
        }

        public List<MY> GetYears()
        {
            int presentYear = DateTime.Now.Year;
            List<string> lstYears = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                lstYears.Add(presentYear.ToString());
                presentYear = presentYear - 1;
            }
            var result = (from yr in lstYears
                          select new MY
                          { Id = yr, Name = yr }).ToList();

            return result;


        }

        public List<MY> GetMonths()
        {
            var engCulture = new CultureInfo("en-US");
            var result = (from m in engCulture.DateTimeFormat.MonthNames
                          where !String.IsNullOrEmpty(m)
                          select new MY
                          { Id = m, Name = m }).ToList();

            return result;
        }


        public string getNameFromAccCode(string accCode)
        {
            if (accCode.IsNullOrEmpty())
                return "";

            var name = otherCharges.FirstOrDefault(t => t.AccCode == accCode).Name;
            return name;
        }
        public async Task PaymentForChanged(ChangeEventArgs e)
        {
            ServiceForPayment.Amount = null;
            var value = e.Value.ToString();
            ServiceForPayment.paymentFor = value;
            var otherCharges = await GetOtherCharges();

            foreach (var otherCharge in otherCharges)
            {
                if (otherCharge.AccCode == value)
                {
                    if (otherCharge.ChargeAmount != 0)

                        ServiceForPayment.Amount = otherCharge.ChargeAmount.ToString();
                    //if (value["AccCode"] == "FPP" || value["AccCode"] == "AFP")
                    //{
                    //    vm.isvalidamount = false;
                    //}
                }
            }

        }

        private void OnSuccessCaptchaForget(bool isValid)
        {
            isCaptchaValid = isValid;
            if (isValid)
            {

                ServiceForPaymentCaptchaDto.Captcha = "DONE";
                if (ServicePaymentCaptchaEditContext != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = ServicePaymentCaptchaEditContext.Field("Captcha");

                    // Validate the field when notifying change
                    ServicePaymentCaptchaEditContext.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }

        }

        //private void OnExpiredCaptchaForget()
        //{
        //    ServiceForPaymentCaptchaDto.Captcha = string.Empty;
        //    StateHasChanged();
        //}

    }

    public class MY
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
