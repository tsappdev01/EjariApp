using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.InquiryForms;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using DIP.InquiryForms;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using Excubo.Generators.Blazor.ExperimentalDoNotUseYet;

namespace DIP.Blazor.Pages.Managements.InquiryForms
{
    public partial class InquiryFormsDetails
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }



        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private bool CanDeleteInquiryForm { get; set; }

        private InquiryFormDto InquiryFormDto { get; set; }
        private Guid DetailInquiryFormId { get; set; }

        public InquiryFormsDetails()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    DetailInquiryFormId = Guid.Parse(Id);
                    InquiryFormDto = await InquiryFormsAppService.GetAsync(DetailInquiryFormId);
                }
                catch (Exception ex)
                {

                    //await uiMessageService.Error("Error in get data");
                    Logger.LogError(ex, "Error in get data");
                    //NavigationManager.NavigateTo("/page-informations");
                    NavigationManager.NavigateTo($"/{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
                    //{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}
                    await HandleErrorAsync(ex);
                }
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:InquiryForms"],
              url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Details"]));
            return ValueTask.CompletedTask;
        }


        private async Task SetPermissionsAsync()
        {
            CanDeleteInquiryForm = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.InquiryForms.Delete);
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
     

 
    }
}
