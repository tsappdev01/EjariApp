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
using Microsoft.AspNetCore.Components.Forms;
using DIP.EServices;
using DIP.Blazor.Shared;
using MiniExcelLibs;
using System.Net.Http;
using System.ComponentModel;
using Scriban.Syntax;
using DIP.Settings;
using Castle.Core.Smtp;
using DIP.Emails;
using System.Net.Http.Json;
using SweetAlertBlazor;

namespace DIP.Blazor.Pages.Site.ContactUs
{
    public partial class Index
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

        [Inject]
        private IEmailsAppService EmailsAppService { get; set; }
        public ContactFormCreateDto NewContactForm { get; set; }
        //private Validations NewContactFormValidations { get; set; } = new();
       // private bool DisplayAlert { get; set; } = false;

        private bool DisableButton  = false;


        private InquiryFormCreateDto NewInquiryForm { get; set; }
        //private Validations NewInquiryFormValidations { get; set; } = new();
        private bool DisableButtonInquiry { get; set; } = false;

        private FeedBackCreateDto NewFeedBack { get; set; }
        //private Validations NewFeedBackValidations { get; set; } = new();
        private bool DisableButtonFeedBack { get; set; } = false;


        [CascadingParameter]

        private EditContext? EditContextContactUs { get; set; }
        private ValidationMessageStore? ValidationMessageStoreContactUs { get; set; }

        private EditContext? EditContextInquiry { get; set; }
        private ValidationMessageStore? ValidationMessageStorInquiry { get; set; }        
        private EditContext? EditContextFeedback { get; set; }
        private ValidationMessageStore? ValidationMessageStorFeedback { get; set; }
        //Test
        //private Captcha captchaComponent { get; set; } 
        //private Captcha captchaComponentInquiry { get; set; }
        //private Captcha captchaComponentFeeddback { get; set; }

        //private GoogleReCaptachDto GoogleReCaptachDto { get; set; } = null;
        private bool isCaptchaContactValid = false;

        private bool isCaptchaInquiryValid = false;

        private bool isCaptchaFeedBackValid = false;

        private HttpClient HttpClient { get; set; }
        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {

            NewContactForm = new ContactFormCreateDto();
            NewInquiryForm = new InquiryFormCreateDto();
            NewFeedBack = new FeedBackCreateDto();
            EditContextContactUs = new(NewContactForm);
            EditContextInquiry = new(NewInquiryForm);
            EditContextFeedback = new(NewFeedBack);

            GetDipBranchesInput getDipBranchesInput = new GetDipBranchesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            DipBranchesList = await DipBranchesAppService.GetListFrontEndAsync(getDipBranchesInput);
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("contact-us");
           SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();




          

            //GoogleReCaptachDto = await DIPSettingAppService.GetGoogleReCaptachSettingAsync();
            ValidationMessageStoreContactUs = new ValidationMessageStore(EditContextContactUs);
            EditContextContactUs.OnValidationRequested += EditContextContactUs_OnValidationRequested;
            EditContextContactUs.OnFieldChanged += EditContext_OnFieldChanged;

            ValidationMessageStorInquiry = new ValidationMessageStore(EditContextInquiry);
            EditContextInquiry.OnValidationRequested += EditContextInquiry_OnValidationRequested;
            EditContextInquiry.OnFieldChanged += EditContextInquiry_OnFieldChanged;

            ValidationMessageStorFeedback = new ValidationMessageStore(EditContextFeedback);
            EditContextFeedback.OnValidationRequested += EditContextFeedBack_OnValidationRequested;
            EditContextFeedback.OnFieldChanged += EditContextFeedBack_OnFieldChanged;



        }
        private void EditContextContactUs_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStoreContactUs?.Clear();
        }
        private  void EditContext_OnFieldChanged(object sender,
                                     FieldChangedEventArgs e)
        {
            ValidationMessageStoreContactUs?.Clear(e.FieldIdentifier);
        }       
        private void EditContextInquiry_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStorInquiry?.Clear();

        }
        private  void EditContextInquiry_OnFieldChanged(object sender,
                                     FieldChangedEventArgs e)
        {
            ValidationMessageStorInquiry?.Clear(e.FieldIdentifier);
        }       
        private void EditContextFeedBack_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidationMessageStorFeedback?.Clear();
        }
        private  void EditContextFeedBack_OnFieldChanged(object sender,
                                     FieldChangedEventArgs e)
        {
            ValidationMessageStorFeedback?.Clear(e.FieldIdentifier);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("dip_modal_popup" , null);
        }
        //private async Task<bool> CheckCaptcha(int type)
        //{
        //    try
        //    {
        //        string reCaptchaResponse = "";
        //        if (type == 0)
        //            reCaptchaResponse = await captchaComponent.GetResponseAsync();
        //        else
        //        {
        //            if(type == 1)
        //                reCaptchaResponse = await captchaComponentInquiry.GetResponseAsync();
        //            else if(type == 2)
        //                reCaptchaResponse = await captchaComponentFeeddback.GetResponseAsync();
        //        }
        //        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        //        {
        //            { "secret", GoogleReCaptachDto.SecretKey},
        //            { "response", reCaptchaResponse}
        //        });

        //        HttpClient = new HttpClient();
        //        var response = await HttpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
               
        //        if (response.IsSuccessStatusCode)
        //        {
        //           // var jsonString = await response.Content.Rea();
        //            var verificationResponse = await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>();
        //            if (verificationResponse != null)
        //                return verificationResponse.Success;
        //            return false;
        //        }
        //        return false;
        //    }
        //    catch {
        //        return false;
        //    }
        //}
        private async Task CreateContactFormAsync()
        {
            try
            {
                DisableButton = true;
                if(isCaptchaContactValid == false)
                {
                    DisableButton = false;
                    return;
                }

                await ContactFormsAppService.CreateAsync(NewContactForm);
              //  await EmailsAppService.SendEmail("adaioob@gmail.com", NewContactForm.Subject, NewContactForm.FullName);
                NewContactForm = new ContactFormCreateDto();
                EditContextContactUs = new(NewContactForm);
                ValidationMessageStoreContactUs = new ValidationMessageStore(EditContextContactUs);
                EditContextContactUs.OnValidationRequested += EditContextContactUs_OnValidationRequested;
                //Generate options/configuration model
                var swalAlertModel = new SwalModel(L["Site:AddedSuccessfuly"],"")
                            .WithIcon(SweetAlert.Icon.Success) // Other Icons are Success, Error and Warning
                            .WithButton(SweetAlert.Button.Ok())
                            .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                // Show the alert
                await  JS.ShowSwalAsync(swalAlertModel);
                //DisplayAlert = true;
               

                //// make sure the alert shows for a bit
                //await Task.Delay(1000);
                //DisplayAlert = false;
                DisableButton = false;
                StateHasChanged();

            }
            catch (Exception ex)
            {
                DisableButton = false;
                await HandleErrorAsync(ex);
            }
        }
        private async Task CreateInquiryFormAsync()
        {
            try
            {
                DisableButtonInquiry = true;
                if (isCaptchaInquiryValid == false)
                {
                    DisableButtonInquiry = false;
                    return;
                }             

                await InquiryFormsAppService.CreateAsync(NewInquiryForm);
                //await EmailsAppService.SendEmail("adaioob@gmail.com", "", NewInquiryForm.Name);
                NewInquiryForm = new InquiryFormCreateDto();
                EditContextInquiry = new(NewInquiryForm);
                ValidationMessageStorInquiry = new ValidationMessageStore(EditContextInquiry);
                EditContextInquiry.OnValidationRequested += EditContextInquiry_OnValidationRequested;

                //Generate options/configuration model
                var swalAlertModel = new SwalModel(L["Site:AddedSuccessfuly"], "")
                            .WithIcon(SweetAlert.Icon.Success) // Other Icons are Success, Error and Warning
                            .WithButton(SweetAlert.Button.Ok())
                            .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                // Show the alert
                await JS.ShowSwalAsync(swalAlertModel);

                DisableButtonInquiry = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private async Task CreateFeedBackFormAsync()
        {
            try
            {
                DisableButtonFeedBack = true;
                if (isCaptchaFeedBackValid == false)
                {
                    DisableButtonFeedBack = false;
                    return;
                }  

                await FeedBacksAppService.CreateAsync(NewFeedBack);
                //await EmailsAppService.SendEmail("adaioob@gmail.com", "", NewFeedBack.ContactPersonName);
                NewFeedBack = new FeedBackCreateDto();
                EditContextFeedback = new(NewFeedBack);
                ValidationMessageStorFeedback = new ValidationMessageStore(EditContextFeedback);
                EditContextFeedback.OnValidationRequested += EditContextFeedBack_OnValidationRequested;

                //Generate options/configuration model
                var swalAlertModel = new SwalModel(L["Site:AddedSuccessfuly"], "")
                            .WithIcon(SweetAlert.Icon.Success) // Other Icons are Success, Error and Warning
                            .WithButton(SweetAlert.Button.Ok())
                            .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                // Show the alert
                await JS.ShowSwalAsync(swalAlertModel);
                DisableButtonFeedBack = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                DisableButtonFeedBack = false;
                await HandleErrorAsync(ex);
            }
        }

        private void OnSuccessCaptchaContact(bool isValid)
        {
            isCaptchaContactValid = isValid;    
            if (isValid) 
                {
                    NewContactForm.Captcha = "DONE";
                    if (EditContextContactUs != null)
                    {
                        // Get the FieldIdentifier with the EditContext from the field name
                        FieldIdentifier fieldIdentifier = EditContextContactUs.Field("Captcha");

                        // Validate the field when notifying change
                        EditContextContactUs.NotifyFieldChanged(fieldIdentifier);
                    }
                    //   EditContextContactUs.Validate();
                    StateHasChanged();
                }
 
        }

        //private void OnExpiredCaptchaContact()
        //{
        //    NewContactForm.Captcha =String.Empty;
        //    StateHasChanged();
        //}
        private void OnSuccessCaptchaInquiry(bool isValid)
        {
            isCaptchaInquiryValid = isValid;
            if (isValid)
            {
                NewInquiryForm.Captcha = "DONE";
                if (EditContextInquiry != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextInquiry.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextInquiry.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }
         
        }

        //private void OnExpiredCaptchaInquiry()
        //{
        //    NewInquiryForm.Captcha = string.Empty;
        //    StateHasChanged();
        //}
        private void OnSuccessCaptchaFeedback(bool isValid)
        {
            isCaptchaFeedBackValid = isValid;
            if (isValid)
            {
                NewFeedBack.Captcha = "DONE";
                if (EditContextFeedback != null)
                {
                    // Get the FieldIdentifier with the EditContext from the field name
                    FieldIdentifier fieldIdentifier = EditContextFeedback.Field("Captcha");

                    // Validate the field when notifying change
                    EditContextFeedback.NotifyFieldChanged(fieldIdentifier);
                }
                StateHasChanged();
            }
           
        }

        //private void OnExpiredCaptchaFeedback()
        //{
        //    NewFeedBack.Captcha = string.Empty;
        //    StateHasChanged();
        //}

    }
}
