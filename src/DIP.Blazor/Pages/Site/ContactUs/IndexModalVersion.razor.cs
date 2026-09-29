using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;
using DIP.DipBranches;
using DIP.SiteSettings;
using Blazorise;
using DIP.ContactForms;
using DIP.InquiryForms;
using DIP.FeedBacks;
using DIP.Categories;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Site.ContactUs
{
    public partial class IndexModalVersion
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public ISiteSettingsAppService SiteSettingsAppService { get; set; }
        public SiteSettingFrontEnd SiteSettingFrontEnd { get; set; }

        [Inject]
        public IDipBranchesAppService DipBranchesAppService { get; set; }

        public List<DipBranchFront> DipBranchesList { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        private ContactFormCreateDto NewContactForm { get; set; }
        private Validations NewContactFormValidations { get; set; } = new();
        private bool DisplayAlert { get; set; } = false;
        private bool DisplayValidation { get; set; } = false;
        private bool DisableButton { get; set; } = false;


        private InquiryFormCreateDto NewInquiryForm { get; set; }
        private Validations NewInquiryFormValidations { get; set; } = new();
        private bool DisplayAlertInquiry { get; set; } = false;
        private bool DisplayValidationInquiry { get; set; } = false;
        private bool DisableButtonInquiry { get; set; } = false;

        private FeedBackCreateDto NewFeedBack { get; set; }
        private Validations NewFeedBackValidations { get; set; } = new();
        private bool DisplayAlertFeedBack { get; set; } = false;
        private bool DisplayValidationFeedBack { get; set; } = false;
        private bool DisableButtonFeedBack { get; set; } = false;

        private Modal CreateCategoryModal { get; set; } = new();

        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        private async Task OpenCreateCategoryModalAsync()
        {
            NewContactForm = new ContactFormCreateDto
            {


            };
            await NewContactFormValidations.ClearAll();
            await CreateCategoryModal.Show();
        }
        private async Task CloseCreateCategoryModalAsync()
        {
            NewContactForm = new ContactFormCreateDto
            {


            };
            await CreateCategoryModal.Hide();
        }
        public IndexModalVersion()
        {
            NewContactForm = new ContactFormCreateDto();
            NewInquiryForm = new InquiryFormCreateDto();
            NewFeedBack = new FeedBackCreateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("contact-us");
            SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();
            GetDipBranchesInput getDipBranchesInput = new GetDipBranchesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            DipBranchesList = await DipBranchesAppService.GetListFrontEndAsync(getDipBranchesInput);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("dip_modal_popup", null);
        }

        private async Task CreateContactFormAsync()
        {
            try
            {

                DisableButton = true;

                if (await NewContactFormValidations.ValidateAll() == false)
                {
                    DisplayValidation = true;
                    StateHasChanged();

                    // make sure the alert shows for a bit
                    await Task.Delay(1000);
                    DisplayValidation = false;
                    DisableButton = false;
                    return;
                }

                // NewStory.ProgramStudied = (ProgramStudied)(SelectedProgramStudied.Value);

                await ContactFormsAppService.CreateAsync(NewContactForm);
                     await CloseCreateCategoryModalAsync();
                //      await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
                //NewContactForm = new ContactFormCreateDto
                //{


                //};
                //NewContactForm = new ContactFormCreateDto();
                //if (NewContactFormValidations != null)
                //    await NewContactFormValidations.ClearAll();
                //DisplayAlert = true;
                //StateHasChanged();

                //// make sure the alert shows for a bit
                //await Task.Delay(1000);
                //DisplayAlert = false;
                //DisableButton = false;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private async Task CreateInquiryFormAsync()
        {
            try
            {

                DisableButtonInquiry = true;

                if (await NewInquiryFormValidations.ValidateAll() == false)
                {
                    DisplayValidationInquiry = true;
                    StateHasChanged();

                    // make sure the alert shows for a bit
                    await Task.Delay(1000);
                    DisplayValidationInquiry = false;
                    DisableButtonInquiry = false;
                    return;
                }

                // NewStory.ProgramStudied = (ProgramStudied)(SelectedProgramStudied.Value);

                await InquiryFormsAppService.CreateAsync(NewInquiryForm);
                NewInquiryForm = new InquiryFormCreateDto();
                if (NewInquiryFormValidations != null)
                    NewInquiryFormValidations.ClearAll();
                DisplayAlertInquiry = true;
                StateHasChanged();

                // make sure the alert shows for a bit
                await Task.Delay(1000);
                DisplayAlertInquiry = false;
                DisableButtonInquiry = false;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private async Task CreateFeedBackAsync()
        {
            try
            {

                DisableButtonFeedBack = true;

                if (await NewFeedBackValidations.ValidateAll() == false)
                {
                    DisplayValidationFeedBack = true;
                    StateHasChanged();

                    // make sure the alert shows for a bit
                    await Task.Delay(1000);
                    DisplayValidationFeedBack = false;
                    DisableButtonFeedBack = false;
                    return;
                }

                // NewStory.ProgramStudied = (ProgramStudied)(SelectedProgramStudied.Value);

                await FeedBacksAppService.CreateAsync(NewFeedBack);
                NewFeedBack = new FeedBackCreateDto();
                if (NewFeedBackValidations != null)
                    await NewFeedBackValidations.ClearAll();
                DisplayAlertFeedBack = true;
                StateHasChanged();

                // make sure the alert shows for a bit
                await Task.Delay(1000);
                DisplayAlertFeedBack = false;
                DisableButtonFeedBack = false;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

    }
}
