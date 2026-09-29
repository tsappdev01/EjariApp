using DIP.Blazor.Shared;
using DIP.EServices;
using DIP.PageInfos;
using DIP.SoapServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using NUglify.JavaScript;
using Scriban.Syntax;
using StgDipService;
using SweetAlertBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.AspNetCore.Components.Alerts;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;

namespace DIP.Blazor.Pages.Site.EjariStatus
{
    public partial class EjariStatus
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string ReferenceNumber { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        #region StepProgress
        private int currentStep = 4;
        private List<StepModel> registrationSteps = new(){
            new() { StepNumber = 1, Title = "Details", IconPath = "assets/images/clipboard-list.svg" },
            new() { StepNumber = 2, Title = "Documents", IconPath = "assets/images/info-square.svg" },
            new() { StepNumber = 3, Title = "Payment", IconPath = "assets/images/credit-card.svg" },
            new() { StepNumber = 4, Title = "Certificate", IconPath = "assets/images/info-square.svg" }
        };

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion
        private EditContext? EditContextEjariDetails;
        private string encryptedReferenceNumber;

        public NocDetailsDto NocDetailsDto { get; set; }

        private ValidationMessageStore? ValidationMessageStoreEjariDetails { get; set; }
        public string ValidateTradeLicenseNo { get; set; }
        public bool hasError { get; set; }

        public ClsRegistration Info { get; private set; }
        public DateTime ContractFromDate { get; private set; }
        public DateTime ContractEndDate { get; private set; }

        public List<SelectListItem> Categories = new List<SelectListItem>();
        private NocDetailsModel nocDetailsModel;
        private bool isSubmit;

        [Inject]
        public IAlertManager _alertManager { get; set; }
        public bool IsNOC { get; private set; }
        public FeedbackViewModel FeedbackModel { get; private set; }
        public PaymentHistory[] getPaymentHistory { get; private set; }
        public ProcessingPage Status { get; private set; }

        public List<Answer> Answers { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }


        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }


        public string AdditionalComments;
        private FeedbackViewModel fbmodel;

        public EjariStatus()
        {
            // Service initialization handled by dependency injection
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


        //private async void OnSubmit()
        //{

        //    await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/Refr");

        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("dip_modal_popup", null);
        }




        private async Task PrepareData()
        {

            var isNOC = await NOCService.CheckRegisterTypeIsNOCAsync(ReferenceNumber);

            IsNOC = isNOC;


            Status = await NOCService.CheckProcessingPageAsync(ReferenceNumber.Trim());

            if (!Status.StatusName.Equals("Submitted") && !Status.StatusName.Equals("Completed"))
            {
                switch (Status.StatusName)
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
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}"); //new flow
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
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        break;
                }
            }

            bool isModalRequired = await NOCService.EOCheckFeedbackForRefNoAsync(ReferenceNumber);
            //if (isModalRequired)
            //{
            fbmodel = new FeedbackViewModel();
            fbmodel.Answers = GetAllAnswers();
            Answers = GetAllAnswers();
            fbmodel.Services = GetAllServices();
            fbmodel.IsPopup = isModalRequired;
            fbmodel.RefNo = ReferenceNumber;
            FeedbackModel = fbmodel;

            //}
            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            getPaymentHistory = await NOCService.GetPaymentHistoryAsync(ReferenceNumber);



        }

        public List<Answer> GetAllAnswers()
        {
            List<Answer> list = new List<Answer>();

            list.Add(new Answer() { ID = 2, Name = L["Good"], IsClick = false, Css = "fa fa-smile-o" });
            list.Add(new Answer() { ID = 1, Name = L["Neutral"], IsClick = false, Css = "fa fa-meh-o" });
            list.Add(new Answer() { ID = 0, Name = L["Poor"], IsClick = false, Css = "fa fa-frown-o" });


            return list;
        }

        public async Task ClickAnswer(Answer answer)
        {
            answer.IsClick = true;
            foreach (var item in Answers)
            {
                if (item.ID != answer.ID)
                {
                    item.IsClick = false;
                }
            }

        }
        public static List<Service> GetAllServices()
        {
            List<Service> list = new List<Service>();
            list.Add(new Service() { ID = 1, Name = "Login" });
            list.Add(new Service() { ID = 2, Name = "Registration" });
            list.Add(new Service() { ID = 3, Name = "Uploads" });
            list.Add(new Service() { ID = 4, Name = "Payment Process" });
            list.Add(new Service() { ID = 5, Name = "Status" });
            list.Add(new Service() { ID = 6, Name = "Payment Slip Link" });
            return list;
        }
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private async Task onSubmit()
        {
            try
            {


                var answer = Answers.SingleOrDefault(x => x.IsClick == true);


                var response = await NOCService.EOInsertFeedbackForRefNoAsync(ReferenceNumber, 
                    answer != null ? answer.Name : "Good", 
                    AdditionalComments == null ? "" : AdditionalComments, 
                    Info.CompanyName, Info.Email, "Ejari", 1);

                var swalAlertModel = new SwalModel(L["ThankYouForProvidingYourValuableFeedback"], "")
         .WithIcon(SweetAlert.Icon.Success) // Other Icons are Success, Error and Warning
         .WithButton(SweetAlert.Button.Ok())
         .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                // Show the alert
                await JS.ShowSwalAsync(swalAlertModel);


                bool isModalRequired = await NOCService.EOCheckFeedbackForRefNoAsync(ReferenceNumber);
                //if (isModalRequired)
                //{
                fbmodel = new FeedbackViewModel();
                fbmodel.Answers = GetAllAnswers();
                Answers = GetAllAnswers();
                fbmodel.Services = GetAllServices();
                fbmodel.IsPopup = isModalRequired;
                fbmodel.RefNo = ReferenceNumber;
                FeedbackModel = fbmodel;

                //}
                Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
                getPaymentHistory = await NOCService.GetPaymentHistoryAsync(ReferenceNumber);

            }
            catch (Exception ex)
            {
                var t = ex;
            }




        }
    }
}
