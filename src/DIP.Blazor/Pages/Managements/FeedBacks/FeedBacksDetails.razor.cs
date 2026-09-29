using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.FeedBacks;
using DIP.Permissions;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.FeedBacks
{
    public partial class FeedBacksDetails
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private bool CanDeleteFeedBack { get; set; }

        private FeedBackDto FeedBackDto { get; set; }
        private Validations DetailFeedBackValidations { get; set; } = new();
        private Guid DetailFeedBackId { get; set; }

        public FeedBacksDetails()
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
                    DetailFeedBackId = Guid.Parse(Id);
                    FeedBackDto = await FeedBacksAppService.GetAsync(DetailFeedBackId);
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:FeedBacks"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Details"]));
            return ValueTask.CompletedTask;
        }


        private async Task SetPermissionsAsync()
        {
            CanDeleteFeedBack = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.FeedBacks.Delete);
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
     

 
    }
}
