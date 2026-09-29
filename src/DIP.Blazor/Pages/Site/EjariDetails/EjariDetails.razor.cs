using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using System.Globalization;
using System.Web;
using StgDipService;
using DIP.SoapServices;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using DIP.Blazor.Shared;
using Volo.Abp.AspNetCore.Components.Alerts;
using DIP.PageInfos;
using SweetAlertBlazor;
using Microsoft.JSInterop;
using DIP.Settings;
using DIP.EServices;

namespace DIP.Blazor.Pages.Site.EjariDetails
{
    public partial class EjariDetails
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        [Inject]
        NavigationManager NavigationManager { get; set; }


        private EditContext? EditContextEjariDetails;
        public NocDetailsDto NocDetailsDto { get; set; }

        private ValidationMessageStore? ValidationMessageStoreEjariDetails { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public bool hasError { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }

        public ClsRegistration Info { get; private set; }
        public DateTime ContractFromDate { get; private set; }
        public DateTime ContractEndDate { get; private set; }

        public List<SelectListItem> Categories = new List<SelectListItem>();
        private NocDetailsModel nocDetailsModel;
        private bool isSubmit;


        [Inject]
        public IJSRuntime JS { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        private Captcha captchaComponent { get; set; }
        private bool DisableButton = false;

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }



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
            NocDetailsDto.ContractFromDate = DateTime.Now;
            NocDetailsDto.ContractEndDate = DateTime.Now;
            EditContextEjariDetails = new(NocDetailsDto);
            ValidationMessageStoreEjariDetails = new ValidationMessageStore(EditContextEjariDetails);
            EditContextEjariDetails.OnValidationRequested += EditContextEjariDetails_OnValidationRequested;

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



        private async Task OnSubmit()
        {
            {
                isSubmit = true;
            }
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);


            if (status.StatusName.Equals("Verified"))
            {
                await NOCService.InsertRegistrationDetailsAsync(new ClsRegistrationDetails
                {


                    BuildingName = HttpUtility.HtmlEncode(NocDetailsDto.BuildingName),
                    FromDate = NocDetailsDto.ContractFromDate,
                    ToDate = NocDetailsDto.ContractEndDate,
                    IsNewNOC = NocDetailsDto.NocType == 1,
                    IsNewContract = NocDetailsDto.ContractType == 1,
                    IsSisterCompany = NocDetailsDto.IsSisterCompany == 2,
                    RefNo = ReferenceNumber.ToString(),
                    RentAmount = decimal.Parse(NocDetailsDto.BasicRent.ToString(CultureInfo.CurrentCulture)),
                    SecurityDeposit = decimal.Parse(NocDetailsDto.SecurityDeposit.ToString(CultureInfo.CurrentCulture)),
                    UnitDetails = HttpUtility.HtmlEncode(NocDetailsDto.UnitsName),
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
                    IsCateringSpecified = true
                }, isSubmit);
            }
            else
            {

                switch (status.StatusName)
                {


                    case "Payment":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                        break;
                    case "PaymentProcess":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                        break;
                    case "Registered":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Verified":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Upload":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");
                        break;
                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                }
            }
            if (isSubmit)
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");

        }


        private async Task SaveAsDraft()
        {

            {
                isSubmit = false;
            }
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            if (status.StatusName.Equals("Verified"))
            {
                var clsRegistrationDetails = new ClsRegistrationDetails
                {
                    BuildingName = HttpUtility.HtmlEncode(NocDetailsDto.BuildingName),
                    FromDate = NocDetailsDto.ContractFromDate,
                    ToDate = NocDetailsDto.ContractEndDate,
                    IsNewNOC = NocDetailsDto.NocType == 1,
                    IsNewContract = NocDetailsDto.ContractType == 1,
                    IsSisterCompany = NocDetailsDto.IsSisterCompany == 2,
                    RefNo = ReferenceNumber.ToString(),
                    RentAmount = decimal.Parse(NocDetailsDto.BasicRent.ToString(CultureInfo.CurrentCulture)),
                    SecurityDeposit = decimal.Parse(NocDetailsDto.SecurityDeposit.ToString(CultureInfo.CurrentCulture)),
                    UnitDetails = HttpUtility.HtmlEncode(NocDetailsDto.UnitsName),
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

                };

                var res = await NOCService.InsertRegistrationDetailsAsync(clsRegistrationDetails, isSubmit);
                if (res == true)
                {
                    var swalAlertModel = new SwalModel(L["SaveToDraftSuccessfully"], "")
                        .WithIcon(SweetAlert.Icon.Success)
                        .WithButton(SweetAlert.Button.Ok())
                        .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                    // Show the alert
                    await JS.ShowSwalAsync(swalAlertModel);
                }
                else
                {

                    var swalAlertModel = new SwalModel(L["UnfortunatelyItWasNotSaved"], "")
                     .WithIcon(SweetAlert.Icon.Warning)
                     .WithButton(SweetAlert.Button.Ok())
                     .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                    // Show the alert
                    await JS.ShowSwalAsync(swalAlertModel);

                }



            }
            else
            {
                switch (status.StatusName)
                {


                    case "Payment":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                        break;
                    case "PaymentProcess":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                        break;
                    case "Registered":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Verified":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Upload":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");
                        break;
                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                }
            }


        }


        private async Task PrepareData()
        {

            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            if (!status.StatusName.Equals("Verified"))
            {
                switch (status.StatusName)
                {


                    case "Payment":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                        break;
                    case "PaymentProcess":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                        break;
                    case "Registered":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Verified":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}");
                        break;
                    case "Upload":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}");
                        break;
                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                }
            }

            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            await PopulateCategories();
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
                    NocDetailsDto.NumberOfUnits = int.Parse(HttpUtility.HtmlEncode(details.NoOfUnits));
                    NocDetailsDto.UnitsName = HttpUtility.HtmlEncode(details.UnitDetails);
                    NocDetailsDto.BuildingName = HttpUtility.HtmlEncode(details.BuildingName);
                    NocDetailsDto.ContractFromDate = details.FromDate;
                    NocDetailsDto.ContractEndDate = details.ToDate;
                    NocDetailsDto.NOCFor = HttpUtility.HtmlEncode(details.NOCFor);
                    EditContextEjariDetails = new(NocDetailsDto);
                    ValidationMessageStoreEjariDetails = new ValidationMessageStore(EditContextEjariDetails);
                    EditContextEjariDetails.OnValidationRequested += EditContextEjariDetails_OnValidationRequested;

                }
            }

            else
            {
                NocDetailsDto.ContractFromDate = DateTime.Now;
                NocDetailsDto.ContractEndDate = DateTime.Now;
            }


        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private async Task PopulateCategories()
        {
            var categorieTypes = await NOCService.GetCategoryTypesAsync();
            Categories = (from c in categorieTypes
                          select new SelectListItem() { Text = c.CategoryName, Value = c.CategoryId.ToString() }).ToList();
        }
    }
}
