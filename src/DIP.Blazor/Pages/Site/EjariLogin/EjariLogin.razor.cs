using Blazorise;
using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using StgDipService;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace DIP.Blazor.Pages.Site.EjariLogin
{
    public partial class EjariLogin
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        ILogger<EjariLogin> _logger { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        private EditContext? EditContextRegistration;
        private EditContext? EditContextLogin;

        private RegistrationDto RegistrationDto { get; set; }
        private LoginDto LoginDto { get; set; }


        private ValidationMessageStore? ValidationMessageStoreRegistration { get; set; }
        private ValidationMessageStore? ValidationMessageStoreLogin { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public string RegisterMessageError { get; set; }
        public bool hasError { get; set; }
        public bool hasLogInError { get; set; }
        public bool IsValidPropetyValue { get; set; }
        public string ValidatePropetyValue { get; set; }



        public List<SelectListItem> Issuers = new List<SelectListItem>();
        public List<SelectListItem> Categories = new List<SelectListItem>();
        private List<SelectListItem> Directions = new List<SelectListItem>();
        public IEnumerable<CountryItem> CountryItems = [];

        private string _selectedCountryDialCode = "+971";
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        //private Captcha captchaComponentLogIn { get; set; }
        //private GoogleReCaptachDto GoogleReCaptachDto { get; set; } = null;
        //private CheckCaptcha CheckCaptcha { get; set; }
        private bool DisableButtonLogin = false;
        //private Captcha captchaComponentRegistration { get; set; }
        private bool DisableButtonRegistration = false;
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        private bool isInput1Enabled;
        private bool isInput2Enabled;
        private bool isInput3Enabled;
        private string value0;
        private string value1;
        private string value2;
        public Dictionary<string, string> valuesForm;
        private bool isCaptchaLogInValid = false;
        private bool isCaptchaRegistrationValid = false;

        private int _selectedRadio = -1;
        private DateTimeOffset? ExpiryDateOffset { get; set; }
        private DateTimeOffset MaxDateOffset { get; set; }
        private DateTimeOffset MinDateOffset { get; set; }
        public int SelectedRadio
        {
            get => _selectedRadio;
            set
            {
                _selectedRadio = value;
                RadioGroupChange();
            }
        }
        public DateTime MinDate { get; set; } = DateTime.Today.AddDays(1);
        public DateTime MaxDate { get; set; } = DateTime.Today.AddYears(100);
        public bool IsDateHaveBetween { get; set; } = false;
        public bool IsConfirmed { get; set; } = false;
        public bool IsSameAsExistingRenewalActivity { get; set; } = true;

        Modal confirmModalRef;

        DatePicker<DateTime> datePicker;

        public EjariLogin()
        {
            // Service initialization handled by dependency injection
        }

        private async Task OnClickRegistration()
        {
            IsValidPropetyValue = false;
            StateHasChanged();

            var allString = RegistrationDto.PropertyCode + "-" + RegistrationDto.PropertyValue;
            var res = await NOCService.GetBuidlingNamesByPropertyAsync(allString);
            if (res != null && res.Length == 1)
            {
                hasLogInError = false;
                IsValidPropetyValue = true;
                ValidatePropetyValue = L["PropertyValueNotValid"];
            }


            if (IsValidPropetyValue || hasError || IsDateHaveBetween)
            {
                DisableButtonRegistration = false;
                return;

            }

            if (isCaptchaRegistrationValid == false)
            {
                hasLogInError = false;

                DisableButtonRegistration = false;
                return;
            }
            var validProperty = await NOCService.GetPropertyValidationAsync(RegistrationDto.PropertyCode, RegistrationDto?.PropertyValue);
            if (!validProperty)
            {
                hasError = true;
                hasLogInError = false;
                RegisterMessageError = L["PropertyCodeIsInvalid"];
                DisableButtonRegistration = false;
                return;
            }

            var propertyHaveTenantCheck = await NOCService.CheckProprtyCodeHaveTenantAsync(RegistrationDto?.PropertyCode + "-" + RegistrationDto?.PropertyValue);
            if (!propertyHaveTenantCheck)
            {
                //hasLogInError = true;
                //RegisterMessageError = "The property code has not been assigned to a tenant. Please contact the administrator. You cannot proceed with registration until a tenant is assigned to the property code.";


                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    icon = "warning",
                    html = $"The landlord is not registered with UAEPASS for this property code (Plot No): {RegistrationDto?.PropertyCode}-{RegistrationDto?.PropertyValue}.<br/><br/>" +
                            //"Please complete the registration to proceed.<br/><br/>" +
                            "<a href='https://reg.dipark.com' target='_blank'>Please click here to register</a>",

                    showConfirmButton = true,
                    confirmButtonText = "Ok",
                    confirmButtonColor = "#6f6259",

                    allowOutsideClick = false,
                    allowEscapeKey = false
                });
                DisableButtonRegistration = false;
                return;
            }

            IsConfirmed = false;
            await confirmModalRef.Show();

        }

        private async Task FinalSubmit()
        {
            DisableButtonRegistration = true;
            await confirmModalRef.Hide();
            string referenceNumber = "0";
            try
            {
                var issuer = Issuers.SingleOrDefault(t => t.Value == RegistrationDto.TradeLicenseIssuerId);

                var registerModel = new ClsRegistration()
                {


                    TypeId = int.Parse(HttpUtility.HtmlEncode(RegistrationDto.NocType)),
                    //TypeName = "Individual",
                    TypeIdSpecified = true,
                    PropertyCode = $"{HttpUtility.HtmlEncode(RegistrationDto?.PropertyCode)}-{HttpUtility.HtmlEncode(RegistrationDto?.PropertyValue)}",

                    CompanyName = HttpUtility.HtmlEncode(RegistrationDto.SubTenantName),

                    MobileNo = HttpUtility.HtmlEncode(RegistrationDto.CountryCode + "-" + RegistrationDto.MobileNumber),
                    Email = HttpUtility.HtmlEncode(RegistrationDto.Email),
                    TradeLicenseIssuerId = int.Parse(HttpUtility.HtmlEncode(RegistrationDto.TradeLicenseIssuerId)),
                    TradeLicenseType = HttpUtility.HtmlEncode(RegistrationDto.TradeLicenseType), //text box input
                    TradeLicenseNo = RegistrationDto.InputValueTradeLicenseNo, //input number
                    TradeLicenseIssuerIdSpecified = true,
                    //TradeLicenseIssuerName = "Initial Approval",
                    TLExpiryDate = RegistrationDto.ExpiryDate,
                    TLExpiryDateSpecified = true,
                    RegisterType = 1,
                    RegisterTypeSpecified = true,
                    ZExpired = false,
                    ZExpiredIn = 0,
                    ZzApplicationType = string.Empty,
                    ZzzNocTypeField = RegistrationDto.NocRegistrationType,
                };
                referenceNumber = await NOCService.RegistrationConfirmationAsync(registerModel);
                _logger.LogInformation("RegistrationConfirmation completed with reference number: {ReferenceNumber}", referenceNumber);
                if (referenceNumber == "0")
                {
                    hasLogInError = true;
                    DisableButtonRegistration = false;
                    RegisterMessageError = L["TheDetailsAreAlreadyRegistered"];
                }
                else
                {
                    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Verify/{RegistrationDto.Email}/{RegistrationDto.CountryCode + "-" + RegistrationDto.MobileNumber}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegistrationConfirmation exception!");
                DisableButtonRegistration = false;
            }

            if (hasError)
            {
                return;
            }
        }

        private void OnNocTypeChanged(string value)
        {
            hasError = false;
            ValidateTradeLicenseNo = string.Empty;
            RegistrationDto.TradeLicenseIssuerId = string.Empty;
            RegistrationDto.NocType = value;
            if (value == "1")
            {
                RegistrationDto.TradeLicenseIssuerId = "51";
                SelectedRadio = 2;
            }
            else if (value == "2")
            {
                SelectedRadio = 1;
            }
            else if (value == "3")
            {
                SelectedRadio = 1;
            }
            // your logic here
        }

        private async void OnIsSameAsExistingRenewalActivityValueChanged(bool value)
        {
            IsSameAsExistingRenewalActivity = value;
            if (!value)
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    icon = "warning",
                    title = "Warning",
                    html = @"The main activity to be carried out at the premises is different from the activity specified in the expired NOC.<br/><br/>
                            A <b>Renewal NOC</b> can only be submitted when the main activity remains unchanged.<br/><br/>
                            Please submit as <b>'New'</b> for <b>Registration Type</b> to proceed.",
                    showConfirmButton = true,
                    confirmButtonText = "OK",
                    confirmButtonColor = "#6f6259",
                    allowOutsideClick = false,
                    allowEscapeKey = false
                });
                IsSameAsExistingRenewalActivity = true;
                RegistrationDto.NocRegistrationType = true;
            }
            StateHasChanged();
        }
        private async Task ToggleDatePicker()
        {
            // This method is a placeholder for the calendar button click
            // The native date input will show its picker when clicked
            await datePicker.ToggleAsync();
            //await Task.CompletedTask;
        }

        private async Task OnClickLogin()
        {
            DisableButtonLogin = false;
            if (!string.IsNullOrEmpty(LoginDto.ReferenceNumber) && !string.IsNullOrEmpty(LoginDto.PassCode))
            {
                var refno = LoginDto.ReferenceNumber.Trim();

                var encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(refno);

                var verificationResponse = await NOCService.RegistrationVerificationAsync(refno, LoginDto.PassCode.Trim());
                if (verificationResponse)
                {
                    DisableButtonLogin = true;
                    if (isCaptchaLogInValid == false)
                    {
                        DisableButtonLogin = false;
                        return;
                    }
                    var statusResponse = await NOCService.CheckProcessingPageAsync(refno);

                    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                    switch (statusResponse.StatusName)
                    {
                        case "Payment":
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                            break;
                        case "PaymentProcess":
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                            break;
                        default:
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                            break;
                    }
                }
                else
                {
                    hasLogInError = true;

                    RegisterMessageError = @L["InvalidCredentialsApplicationIsCancelled"];

                }
            }

        }

        private void EditContextRegistration_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreRegistration?.Clear();

            // License number path (SelectedRadio == 1): skip TL expiry range validation; other options still require date between MinDate and MaxDate.
            if (SelectedRadio == 1)
            {
                IsDateHaveBetween = false;
            }
            else if (RegistrationDto.ExpiryDate < MinDate || RegistrationDto.ExpiryDate > MaxDate)
            {
                IsDateHaveBetween = true;
            }
            else
            {
                IsDateHaveBetween = false;
            }

            switch (RegistrationDto.TradeLicenseNo)
            {
                case "0":
                    if (string.IsNullOrEmpty(RegistrationDto.InitialApproval))
                    {
                        hasError = true;

                        ValidateTradeLicenseNo = L["InitialApprovalIsRequired"];
                    }
                    else
                    {
                        RegistrationDto.TradeLicenseType = "Initial Approval";
                        RegistrationDto.InputValueTradeLicenseNo = RegistrationDto.InitialApproval;
                        hasError = false;


                    }

                    break;

                case "1":
                    if (string.IsNullOrEmpty(RegistrationDto.LicenseNo))
                    {
                        hasError = true;

                        ValidateTradeLicenseNo = L["LicenseNoIsRequired"];
                    }
                    else
                    {

                        RegistrationDto.TradeLicenseType = "License";
                        RegistrationDto.InputValueTradeLicenseNo = RegistrationDto.LicenseNo;
                        hasError = false;

                    }

                    break;

                case "2":
                    if (string.IsNullOrEmpty(RegistrationDto.EmiratesId))
                    {
                        hasError = true;

                        ValidateTradeLicenseNo = L["EmiratesIdApprovalIsRequired"];
                    }
                    else
                    {

                        RegistrationDto.TradeLicenseType = "Emirates Id";
                        RegistrationDto.InputValueTradeLicenseNo = RegistrationDto.EmiratesId;
                        hasError = false;


                    }


                    break;
            }

            // Emirates ID Validation
            if (RegistrationDto.TradeLicenseNo == "2" && !string.IsNullOrEmpty(RegistrationDto.EmiratesId))
            {
                if (!EmiratesIdRegex().IsMatch(RegistrationDto.EmiratesId))
                {
                    hasError = true;
                    // You might want to use a specific error message variable for this, 
                    // or reuse ValidateTradeLicenseNo if it fits, or add a new one.
                    // For now, I will append to or set ValidateTradeLicenseNo as it seems to be the one displayed for this section.
                    ValidateTradeLicenseNo = "Invalid Emirates ID format. Expected format: 784-xxxx-xxxxxxx-x";
                }
            }

            if (hasError || IsDateHaveBetween)
                return;
        }

        private void EditContextLogin_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreLogin?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            value0 = "0";
            value1 = "1";
            value2 = "2";
            valuesForm = new Dictionary<string, string>();
            RegistrationDto = new RegistrationDto();
            DateTime d = DateTime.Now.AddYears(120);

            MaxDateOffset = new DateTimeOffset(new DateTime(d.Year, d.Month, d.Day));
            MinDateOffset = new DateTimeOffset(DateTime.Now.AddDays(1).Date);

            // THIS WILL DISPLAY CORRECTLY
            ExpiryDateOffset = new DateTimeOffset(DateTime.Now.AddDays(1).Date);
            RegistrationDto.ExpiryDate = DateTime.Now.AddDays(1).Date; //Dhi
            //DefaultDate = new DateTime(MinDate.Year, MinDate.Month, MinDate.Day);
            LoginDto = new LoginDto();

            EditContextRegistration = new(RegistrationDto);
            ValidationMessageStoreRegistration = new ValidationMessageStore(EditContextRegistration);
            EditContextRegistration.OnValidationRequested += EditContextRegistration_OnValidationRequested;



            EditContextLogin = new(LoginDto);
            ValidationMessageStoreLogin = new ValidationMessageStore(EditContextLogin);
            EditContextLogin.OnValidationRequested += EditContextLogin_OnValidationRequested;

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


            await PopulateIssuers();
            await PopulateDirections();

            // Initialize country list for autocomplete
            CountryItems = CountryList.Countries;

            valuesForm = new Dictionary<string, string>
             {
                    { "1", @L["Individual"] },
                    { "2", @L["Company"] },
                    { "3", @L["IndividualWithLicense"] }
             };

        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }



        public async Task PopulateIssuers()
        {
            var issuersResponse = await NOCService.GetTradeLicenseIssuersAsync();
            Issuers = (from d in issuersResponse
                       select new SelectListItem()
                       {
                           Text = d.IssuerName,
                           Value = d.Issuerid.ToString()
                       }).ToList();
        }

        private async void PopulateCategories()
        {
            var CategorieResponse = await NOCService.GetCategoryTypesAsync();
            Categories = (from c in CategorieResponse
                          select new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList();
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

        private void OnSuccessCaptchaRegistration(bool isValid)
        {
            isCaptchaRegistrationValid = isValid;

            if (isValid)
            {
                RegistrationDto.Captcha = "DONE";
                if (EditContextRegistration != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextRegistration.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextRegistration.NotifyFieldChanged(fieldIdentifier);
                }
                //   EditContextContactUs.Validate();
                StateHasChanged();
            }
            else
            {
                RegistrationDto.Captcha = null;
            }

        }

        //private void OnExpiredCaptchaRegistration()
        //{
        //    RegistrationDto.Captcha = String.Empty;
        //    StateHasChanged();
        //}
        private void OnSuccessCaptchaLogIn(bool isValid)
        {
            isCaptchaLogInValid = isValid;
            if (isValid)
            {
                LoginDto.Captcha = "DONE";
                if (EditContextLogin != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextLogin.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextLogin.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }
            else
            {
                LoginDto.Captcha = null;
            }

        }

        //private void OnExpiredCaptchaLogIn()
        //{
        //    LoginDto.Captcha = string.Empty;
        //    StateHasChanged();
        //}

        private void RadioGroupChange()
        {
            //var input = args.ToString();
            //isInput1Enabled = input == "0";
            //isInput2Enabled = input == "1";
            //isInput3Enabled = input == "2";
            RegistrationDto.TradeLicenseNo = _selectedRadio.ToString();
            if (_selectedRadio == 0)
            {
                RegistrationDto.LicenseNo = "";
                RegistrationDto.EmiratesId = "";
            }
            else if (_selectedRadio == 1)
            {
                RegistrationDto.InitialApproval = "";
                RegistrationDto.EmiratesId = "";
            }
            else if (_selectedRadio == 2)
            {
                RegistrationDto.InitialApproval = "";
                RegistrationDto.LicenseNo = "";
            }
            else
            {
                RegistrationDto.InitialApproval = "";
                RegistrationDto.LicenseNo = "";
                RegistrationDto.EmiratesId = "";
            }
        }



        private string _emiratesIdFormatted;

        public string EmiratesIdFormatted
        {
            get => RegistrationDto.EmiratesId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RegistrationDto.EmiratesId = value;
                    return;
                }

                // Remove all non-digits
                var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

                // Limit to 15 digits (784 + 12 other digits)
                if (digitsOnly.Length > 15)
                {
                    digitsOnly = digitsOnly.Substring(0, 15);
                }

                // Format: 784-1234-1234567-1
                // 3 - 4 - 7 - 1
                var formatted = digitsOnly;

                if (digitsOnly.Length > 3)
                {
                    formatted = digitsOnly.Substring(0, 3) + "-" + digitsOnly.Substring(3);
                }
                if (digitsOnly.Length > 7)
                {
                    formatted = formatted.Substring(0, 8) + "-" + digitsOnly.Substring(7);
                }
                if (digitsOnly.Length > 14)
                {
                    formatted = formatted.Substring(0, 16) + "-" + digitsOnly.Substring(14);
                }

                RegistrationDto.EmiratesId = formatted;
            }
        }

        public string InitialApprovalFormatted
        {
            get => RegistrationDto.InitialApproval;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RegistrationDto.InitialApproval = value;
                    return;
                }

                // Remove spaces
                var noSpaces = value.Replace(" ", "");

                // Limit to 15 chars
                if (noSpaces.Length > 15)
                {
                    noSpaces = noSpaces.Substring(0, 15);
                }

                RegistrationDto.InitialApproval = noSpaces;
            }
        }

        public string LicenseNoFormatted
        {
            get => RegistrationDto.LicenseNo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RegistrationDto.LicenseNo = value;
                    return;
                }

                // Remove spaces
                var noSpaces = value.Replace(" ", "");

                // Limit to 15 chars
                if (noSpaces.Length > 15)
                {
                    noSpaces = noSpaces.Substring(0, 15);
                }

                RegistrationDto.LicenseNo = noSpaces;
            }
        }

        [GeneratedRegex("^784-\\d{4}-\\d{7}-\\d{1}$")]
        private static partial Regex EmiratesIdRegex();

    }
}
