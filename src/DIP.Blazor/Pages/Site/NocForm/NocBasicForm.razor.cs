using Blazorise;
using DIP.Blazor.Pages.Site.EjariLogin;
using DIP.EServices;
using Excubo.Blazor.TreeViews;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NUglify.Helpers;
using StgDipService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;
using DIP.SoapServices;

namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocBasicForm
    {
        [Parameter]
        public string Lang { get; set; }

        #region StepProgress
        private int currentStep = 1;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion
        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        public ClsRegistration Info { get; private set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        public string StatusName { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }


        private EditContext? EditContextEjariNocBasicFormDetails;
        private ValidationMessageStore? ValidationMessageStoreEjariNocBasicFormDetails { get; set; }

        public NocDetailsDtoV1 NocDetailsDto { get; set; }

        public List<SelectListItem> Categories = [];

        //public List<SelectListItem> BuildingNames = [];

        public IEnumerable<SelectListItem> BusinessActivitys = [];

        public List<SelectListItem> MaterialClassification = [];
        public bool ButtonDisable { get; set; } = false;
        public List<SelectListItem> SubLeaseList = [
            new(){Text = "None", Value = "0",},
            new(){Text = "Landlord", Value = "1",},
            new(){Text = "DIP", Value = "2",},
            new(){Text = "Real Estate Broker", Value = "3",},
        ];
        public List<SelectListItem> BuildingNamesTemp = [];
        public IEnumerable<UnitListDto> UnitListTemp = [];
        public IEnumerable<BrokerDetailDto> BrokerDetailTemp = [];
        public List<BusinessActivityDto> BusinessActivityDtoTemp = [];
        public List<MaterialClassificationDto> MaterialClassificationDtoTemp = [];
        public IEnumerable<CountryItem> CountryItems = [];
        private List<string> UnitSelectedValues { get; set; } = [];
        private string BuildingSelectedValue { get; set; }
        private List<string> BusinessActivitysValues { get; set; } = [];
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DateTimeOffset? _ContractFromDate { get; set; }
        private DateTimeOffset? _ContractEndDate { get; set; }
        private DateTimeOffset MaxDateOffset { get; set; }
        public DateTime MinDate { get; set; } = DateTime.Today.AddYears(-10).Date;
        public DateTime MaxDate { get; set; } = DateTime.Today.AddYears(100).Date;
        public bool IsContractFromDateHaveBetween { get; set; } = false;
        public bool IsContractEndDateHaveBetween { get; set; } = false;
        private bool IsBusinessActivityInvalid { get; set; } = false;

        private bool UnitDropdownInvalid { get; set; } = false;
        private string UnitDropdownInvalidText { get; set; } = "Units is required.";
        private bool IsModelValid { get; set; } = false;

        DatePicker<DateTime> datePicker;
        DatePicker<DateTime> datePickerV1;
        private bool unitsLoading = false;

        public NocBasicForm()
        {
        }

        private void EditContextEjariNocBasicFormDetails_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            if (NocDetailsDto.ContractFromDate.Date < MinDate.Date || NocDetailsDto.ContractFromDate.Date > MaxDate.Date)
                IsContractFromDateHaveBetween = true;
            else
                IsContractFromDateHaveBetween = false;

            if (NocDetailsDto.ContractEndDate.Date < MinDate.Date || NocDetailsDto.ContractEndDate.Date > MaxDate.Date)
                IsContractEndDateHaveBetween = true;
            else
                IsContractEndDateHaveBetween = false;

            if (BusinessActivitysValues.Count() == 0)
                IsBusinessActivityInvalid = true;

            if (UnitSelectedValues.Count() == 0)
                UnitDropdownInvalid = true;

            ValidationMessageStoreEjariNocBasicFormDetails?.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);
            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            NocDetailsDto = new NocDetailsDtoV1();
            NocDetailsDto.ContractFromDate = DateTime.Now.ClearTime();
            NocDetailsDto.ContractEndDate = DateTime.Now.ClearTime().AddMonths(6);
            EditContextEjariNocBasicFormDetails = new(NocDetailsDto);
            ValidationMessageStoreEjariNocBasicFormDetails = new ValidationMessageStore(EditContextEjariNocBasicFormDetails);
            EditContextEjariNocBasicFormDetails.OnValidationRequested += EditContextEjariNocBasicFormDetails_OnValidationRequested;


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
            await PrepareData();

            // Initialize country list for autocomplete
            CountryItems = CountryList.Countries;
            MaxDateOffset = new DateTimeOffset(DateTime.Now.AddYears(100).Date);

            _ContractFromDate = NocDetailsDto.ContractFromDate;
            _ContractEndDate = NocDetailsDto.ContractEndDate;

            //UnitSelectedValues.Clear();
        }


        private string GetMobileNumber(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }
            if (mobile.Contains('-'))
            {
                var splitmobile = mobile.Split('-');
                if (splitmobile.Length >= 2)
                {
                    return splitmobile[1];
                }
            }
            return mobile.Replace("+971", string.Empty).Replace("971", string.Empty);
        }

        private string GetCountryCode(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }
            if (mobile.Contains('-'))
            {
                var splitmobile = mobile.Split("-");
                if (splitmobile.Length >= 2)
                {
                    return splitmobile[0];
                }
            }
            return "+971";
        }

        private async Task PrepareData()
        {
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            StatusName = status.StatusName;

            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            NocDetailsDto.TypeName = Info.TypeName;
            await RedirectionToCorrespondingPage();
            await PopulateCategories();
            await PopulateBrokerDetails();
            await PopulateMaterialClassification();
            await PopulateBusinessActivitys();
            await PopulateBuildingNames();
            await GetNocBasiDetails();
            StateHasChanged();

        }

        private async Task RedirectionToCorrespondingPage()
        {
            if (!StatusName.Equals("Verified"))
            {
                switch (StatusName)
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
                        if (Info?.ZzApplicationType == "NewURL")
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}"); //new flow
                        }
                        else
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");
                        }
                        break;
                    case "ApplicantAcknowledgement":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocAcknowledgement/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "PendingAuth":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
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
                        if (Info?.ZzApplicationType == "NewURL")
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        }
                        else
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        }
                        break;
                }
            }
        }


        private async Task GetNocBasiDetails()
        {
            if (Info.IsActive != null && Info.IsActive.Value)
            {
                var details = await NOCService.GetRegistrationDetailsAsync(ReferenceNumber);
                if (details != null)
                {

                    NocDetailsDto.NocType = (details.IsCatering.HasValue && details.IsCatering.Value) ? 3 : (details.IsNewNOC.HasValue && details.IsNewNOC.Value) ? 1 : 2;
                    NocDetailsDto.ContractType = (details.IsNewContract.HasValue && details.IsNewContract.Value) ? 1 : 2;
                    NocDetailsDto.IsSisterCompany = (details.IsSisterCompany.HasValue && details.IsSisterCompany == true) ? 2 : 1;
                    NocDetailsDto.BasicRent = details.RentAmount.HasValue ? double.Parse(details.RentAmount.Value.ToString(CultureInfo.CurrentCulture))
                            : 0;
                    NocDetailsDto.SecurityDeposit = details.SecurityDeposit.HasValue
                            ? double.Parse(details.SecurityDeposit.Value.ToString(CultureInfo.CurrentCulture))
                            : 0;
                    NocDetailsDto.NumberOfUnits = details.NoOfUnits;
                    NocDetailsDto.UnitsName = HttpUtility.HtmlEncode(details.UnitDetails);
                    NocDetailsDto.BuildingName = HttpUtility.HtmlEncode(details.BuildingName);
                    BuildingSelectedValue = HttpUtility.HtmlEncode(details.BuildingName);
                    NocDetailsDto.NOCFor = HttpUtility.HtmlEncode(details.NOCFor);
                    if (details.PrimaryActivity != null)
                    {
                        BusinessActivitys.ForEach(x => x.Selected = details.PrimaryActivity.Split(',').Contains(x.Value));
                        BusinessActivitysValues = BusinessActivitys.Where(x => details.PrimaryActivity.Split(',').Contains(x.Value)).Select(x => x.Value).ToList();
                    }
                    if (details.UnitDetails != null)
                    {
                        if (!string.IsNullOrEmpty(details.BuildingName))
                            await PopulateUnitList();
                        UnitSelectedValues = [.. UnitListTemp.Where(u => details.UnitDetails.Split(", ", StringSplitOptions.RemoveEmptyEntries).Contains(u.Value)).Select(x => x.Value)];
                        NocDetailsDto.unitTotalArea = UnitListTemp.Where(u => details.UnitDetails.Split(", ", StringSplitOptions.RemoveEmptyEntries).Contains(u.Value)).Sum(x => x.UnitArea);
                        NocDetailsDto.NumberOfUnits = UnitSelectedValues.Count;//details.UnitDetails.Split(", ", StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                    NocDetailsDto.SubLease = details.SubleaseSource ?? 0;
                    NocDetailsDto.SubleaseBrokerID = (details.SubleaseSource ?? 0) == 3 ? details.SubleaseBrokerID : "";
                    NocDetailsDto.SubleaseDIPStaffName = (details.SubleaseSource ?? 0) == 2 ? details.SubleasedDIPStaffName : "";
                    NocDetailsDto.LicenseMainActivity = details.MainLicenseActivityDescription;
                    NocDetailsDto.IndustrialWaste = details.IndustrialWasteDescription.Length > 0;
                    NocDetailsDto.IndustrialWasteDescription = details.IndustrialWasteDescription;
                    NocDetailsDto.Drainage = details.Drainage ?? false;
                    NocDetailsDto.DrainageDescription = (details.Drainage ?? false) ? details.DrainageDescription : string.Empty;
                    NocDetailsDto.Hazardous = details.Hazardous ?? false;
                    NocDetailsDto.PropertyCategoryName = details.PropertyCategoryName ?? string.Empty;
                    NocDetailsDto.PropertySubCategoryNames = details.PropertySubCategoryNames ?? string.Empty;
                    if (details.MaterialClassification != null)
                    {
                        MaterialClassification.ForEach(x => x.Selected = details.MaterialClassification.Split(',').Contains(x.Value));
                    }
                    NocDetailsDto.TenantAddress = details.TenantAddress;
                    NocDetailsDto.TenatCompanyName = details.TenatCompanyName;
                    NocDetailsDto.RenewalFlag = details.RenewalFlag;
                    NocDetailsDto.UtilityCharge = Convert.ToDouble(details.UtilityCharge);
                    NocDetailsDto.ContractValue = Convert.ToDouble(details.ContractValue);
                    NocDetailsDto.ChangeInActivities = details.IsNewContract ?? true;//NocDetailsDto.RenewalFlag > 0 ? false : true;
                    NocDetailsDto.FloorNumber = String.IsNullOrEmpty(details.FloorNumber) ? 0 : Convert.ToInt32(details.FloorNumber);
                    NocDetailsDto.NumberOfRooms = UnitSelectedValues.Count;//details.NumberOfRooms ?? 0;
                    NocDetailsDto.ContractFromDate = details.RenewalFlag > 0 || details.FromDate.Year == 1947 ? DateTime.Now : details.FromDate;
                    NocDetailsDto.ContractEndDate = details.ToDate.Date.Equals(DateTime.Now.Date) || details.RenewalFlag > 0 || details.ToDate.Year == 1947 ? DateTime.Now.AddMonths(6) : details.ToDate;
                    NocDetailsDto.LicenseType = details.LicenseType;
                    NocDetailsDto.PropertyBillable = details.ZPropertyBillable;
                    if (NocDetailsDto.RenewalFlag > 0)
                    {
                        IsModelValid = Validator.TryValidateObject(
                                            NocDetailsDto,
                                            new ValidationContext(NocDetailsDto),
                                            null,
                                            true
                                        );
                    }
                }
            }
            else
            {
                NocDetailsDto.ContractFromDate = DateTime.Now;
                NocDetailsDto.ContractEndDate = DateTime.Now.AddMonths(6);
            }
        }
        private async Task PopulateCategories()
        {
            var categories = await NOCService.GetCategoryTypesAsync();
            Categories = (from c in categories
                          select new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList();
        }

        private async Task PopulateUnitList()
        {
            string response = await NOCService.GetUnitsListByPropertyCodeAsync(Info?.PropertyCode, [BuildingSelectedValue]);
            UnitListTemp = JsonConvert.DeserializeObject<List<UnitListDto>>(response);
        }

        private async Task PopulateBuildingNames()
        {
            var buildingNames = await NOCService.GetBuidlingNamesByPropertyAsync(Info!.PropertyCode);
            BuildingNamesTemp = [.. buildingNames.Where(x => x != "Select").Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
            })];
        }

        private async Task OnSubmit()
        {
            if (BusinessActivitysValues.Count() == 0 || UnitSelectedValues.Count() == 0
                || IsContractFromDateHaveBetween || IsContractEndDateHaveBetween)
                return;

            ButtonDisable = true;
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            StatusName = status.StatusName;

            if (status.StatusName.Equals("Verified"))
            {
                var result = await NOCService.InsertRegistrationDetailsAsync(new ClsRegistrationDetails
                {
                    BuildingName = HttpUtility.HtmlEncode(NocDetailsDto.BuildingName),
                    FromDate = NocDetailsDto.ContractFromDate,
                    ToDate = NocDetailsDto.ContractEndDate,
                    IsNewNOC = NocDetailsDto.RenewalFlag > 0 || NocDetailsDto.NocType == 2 ? false : true,
                    IsNewContract = NocDetailsDto.RenewalFlag > 0 && NocDetailsDto.ChangeInActivities ? true :
                    NocDetailsDto.RenewalFlag > 0 && !NocDetailsDto.ChangeInActivities ? false : true,
                    IsSisterCompany = NocDetailsDto.IsSisterCompany == 2,
                    RefNo = ReferenceNumber.ToString(),
                    RentAmount = decimal.Parse(NocDetailsDto.BasicRent.ToString(CultureInfo.CurrentCulture)),
                    SecurityDeposit = decimal.Parse(NocDetailsDto.SecurityDeposit.ToString(CultureInfo.CurrentCulture)),
                    UnitDetails = HttpUtility.HtmlEncode(UnitSelectedValues.JoinAsString(", ")),
                    NoOfUnits = NocDetailsDto.NumberOfUnits,
                    NOCFor = HttpUtility.HtmlEncode(NocDetailsDto.NOCFor),
                    FromDateSpecified = true,
                    ToDateSpecified = true,
                    IsNewNOCSpecified = true,
                    IsNewContractSpecified = true,
                    IsSisterCompanySpecified = true,
                    NoOfUnitsSpecified = true,
                    RentAmountSpecified = true,
                    SecurityDepositSpecified = true,
                    IsActive = true,
                    IsActiveSpecified = true,
                    DetailsId = 0,
                    DetailsIdSpecified = true,
                    IsCatering = NocDetailsDto.NocType == 3,
                    IsCateringSpecified = true,
                    //New props 
                    Area = NocDetailsDto.unitTotalArea,
                    Drainage = NocDetailsDto.Drainage,
                    DrainageDescription = NocDetailsDto.DrainageDescription,
                    FloorNumber = NocDetailsDto.FloorNumber.ToString(),
                    Hazardous = NocDetailsDto.Hazardous,
                    IndustrialWasteDescription = NocDetailsDto.IndustrialWaste ?
                                                string.IsNullOrEmpty(NocDetailsDto.IndustrialWasteDescription) ? " " : NocDetailsDto.IndustrialWasteDescription
                                                : string.Empty,
                    LicenseType = NocDetailsDto.LicenseType,
                    MainLicenseActivityDescription = NocDetailsDto.LicenseMainActivity,
                    MaterialClassification = MaterialClassification.Count != 0 ? MaterialClassification.Where(x => x.Selected == true).Select(x => x.Value).JoinAsString(",") : string.Empty,
                    PrimaryActivity = BusinessActivitys.Where(x => x.Selected == true).Select(x => x.Value).JoinAsString(","),
                    SubleaseBrokerID = NocDetailsDto.SubleaseBrokerID,
                    SubleaseSource = NocDetailsDto.SubLease,
                    SubleasedDIPStaffName = NocDetailsDto.SubleaseDIPStaffName,
                    SubtenantDesignation = NocDetailsDto.SubtenantDesignation,
                    NumberOfRooms = NocDetailsDto.NumberOfRooms,
                    TenatCompanyName = NocDetailsDto.TenatCompanyName,
                    TenantAddress = NocDetailsDto.TenantAddress,
                    TenantCity = NocDetailsDto.TenantCity,
                    UtilityCharge = Convert.ToDecimal(NocDetailsDto.UtilityCharge),
                    ContractValue = Convert.ToDecimal(NocDetailsDto.ContractValue),
                });
                if (result)
                {
                    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}");
                }
            }
            else
            {
                ButtonDisable = false;
                await RedirectionToCorrespondingPage();
            }
        }


        private async Task PopulateBrokerDetails()
        {
            string response = await GetMasterDatadetailAsync(spname: "GetBrokerDetails");
            BrokerDetailTemp = JsonConvert.DeserializeObject<List<BrokerDetailDto>>(response);
        }
        private async Task PopulateBusinessActivitys()
        {
            string response = await GetMasterDatadetailAsync(spname: "GetBusinessActivity");
            response = response.Replace("<![CDATA[", "").Replace("]]>", "").Trim();
            BusinessActivityDtoTemp = JsonConvert.DeserializeObject<List<BusinessActivityDto>>(response);
            BusinessActivitys = [.. BusinessActivityDtoTemp.Select(x => new SelectListItem
            {
                Text = x.MasterActivity,
                Value = x.MasterActivityId.ToString()
            })];
        }
        private async Task PopulateMaterialClassification()
        {
            string response = await GetMasterDatadetailAsync(spname: "GetMaterialClassification");
            MaterialClassificationDtoTemp = JsonConvert.DeserializeObject<List<MaterialClassificationDto>>(response);
            MaterialClassification = [.. MaterialClassificationDtoTemp.Select(x => new SelectListItem
            {
                Text = x.MaterialClassification,
                Value = x.id.ToString()
            })];
        }

        private async Task<string> GetMasterDatadetailAsync(string spname)
        {
            return await NOCService.GetMasterDatadetailAsync(spname);
        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private async Task ToggleDatePicker()
        {
            // This method is a placeholder for the calendar button click
            // The native date input will show its picker when clicked
            await datePicker.ToggleAsync();
            //await Task.CompletedTask;
        }
        private async Task ToggleDatePickerV1()
        {
            // This method is a placeholder for the calendar button click
            // The native date input will show its picker when clicked
            await datePickerV1.ToggleAsync();
            //await Task.CompletedTask;
        }

        #region Onchange Methods
        private void UnitSelectedValuesChanged(List<string> values)
        {
            UnitSelectedValues = values;
            UnitSelectedValues.Remove(UnitSelectedValues.Where(x => x == string.Empty || x == " ").FirstOrDefault() ?? string.Empty);
            NocDetailsDto.unitTotalArea = UnitListTemp.Where(u => values.Contains(u.Value)).Sum(x => x.UnitArea);
            NocDetailsDto.NumberOfUnits = UnitSelectedValues.Count;
            if (UnitSelectedValues.Count() == 0)
                UnitDropdownInvalid = true;
            else
                UnitDropdownInvalid = false;
            NocDetailsDto.NumberOfRooms = UnitSelectedValues.Count;
        }

        private async Task BuildingNameSelectedValueChanged(string values)
        {
            unitsLoading = true;
            StateHasChanged();
            BuildingSelectedValue = values;
            //BuildingSelectedValue.Remove(BuildingSelectedValue.Where(x => x == string.Empty || x == " ").FirstOrDefault() ?? string.Empty);
            NocDetailsDto.BuildingName = values;
            if (!string.IsNullOrEmpty(values)) await PopulateUnitList();
            unitsLoading = false;
            StateHasChanged();
        }

        private void OnBusinessActivityChanged(List<string> values)
        {
            BusinessActivitys.ForEach(x => x.Selected = values.Contains(x.Value));
            BusinessActivitysValues = values;
            if (BusinessActivitysValues.Count == 0)
                IsBusinessActivityInvalid = true;
            else
                IsBusinessActivityInvalid = false;
        }
        //private void OnBusinessActivityChanged(bool value, SelectListItem item)
        //{
        //    var existing = BusinessActivitys.FirstOrDefault(x => x.Value == item.Value);

        //    if (existing != null)
        //        existing.Selected = !existing.Selected;
        //}

        private void OnMaterialClassificationChanged(bool value, SelectListItem item)
        {
            var existing = MaterialClassification.FirstOrDefault(x => x.Value == item.Value);

            if (existing != null)
                existing.Selected = !existing.Selected;
        }

        private void OnFromDateChanged(DateTimeOffset? newValue)
        {
            if (newValue != null)
            {
                if (newValue.Value.Year < DateTime.Now.AddYears(-10).Year)
                {
                    NocDetailsDto.ContractFromDate = DateTime.Now.AddYears(-10).Date;
                }
                _ContractFromDate = newValue;
                NocDetailsDto.ContractFromDate = newValue.Value.Date;
            }

        }

        private void OnEndDateChanged(DateTimeOffset? newValue)
        {
            if (newValue != null)
            {
                if (newValue.Value.Year < DateTime.Now.AddYears(-10).Year)
                {
                    NocDetailsDto.ContractEndDate = DateTime.Now.AddYears(-10).Date;
                }
                _ContractEndDate = newValue;
                NocDetailsDto.ContractEndDate = newValue.Value.Date;
            }
        }
        #endregion
    }
}
