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
using DIP.TimeLineCategories;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.LanguageManagement;
using DIP.Zones;

namespace DIP.Blazor.Pages.Managements.TimeLineCategories
{
    public partial class EditTimeLineCategories
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private TimeLineCategoryCreateDto NewTimeLineCategory { get; set; }
        private TimeLineCategoryUpdateDto EditingTimeLineCategory { get; set; }
        private Validations NewTimeLineCategoryValidations { get; set; } = new();

        private Validations EditingTimeLineCategoryValidations { get; set; } = new();
        private Guid EditingTimeLineCategoryId { get; set; }
        
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private TimeLineCategoryUpdateDto EditingTimeLineCategories { get; set; }

        public EditTimeLineCategories()
        {
            NewTimeLineCategory = new TimeLineCategoryCreateDto();
            EditingTimeLineCategory = new TimeLineCategoryUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();

            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingTimeLineCategoryId = Guid.Parse(Id);
                    var TimeLineCategory = await TimeLineCategoriesAppService.GetAsync(EditingTimeLineCategoryId);
                    EditingTimeLineCategory = ObjectMapper.Map<TimeLineCategoryDto, TimeLineCategoryUpdateDto>(TimeLineCategory);
                 
                    await EditingTimeLineCategoryValidations.ClearAll();
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
            else
                await SetNewAsync();

        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:TimeLineCategories"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));

            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        private async Task CreateTimeLineCategoryAsync()
        {
            try
            {
                if (await NewTimeLineCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                var ent=   await TimeLineCategoriesAppService.CreateAsync(NewTimeLineCategory);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateTimeLineCategoryAsync()
        {
            try
            {
                if (await EditingTimeLineCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await TimeLineCategoriesAppService.UpdateAsync(EditingTimeLineCategoryId, EditingTimeLineCategory);
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
        private async Task SetNewAsync()
        {
            GetTimeLineCategoriesInput getTimeLineCategoriesInput = new GetTimeLineCategoriesInput();
            getTimeLineCategoriesInput.MaxResultCount = 1;
            PagedResultDto<TimeLineCategoryDto> timeLineCategories = (await TimeLineCategoriesAppService.GetListAsync(getTimeLineCategoriesInput));
            if (timeLineCategories != null)
                NewTimeLineCategory.Order = Convert.ToInt32(timeLineCategories.TotalCount);
            NewTimeLineCategory.IsActive = true;
        }

    }
}
