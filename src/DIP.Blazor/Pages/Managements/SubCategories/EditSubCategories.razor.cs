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
using DIP.SubCategories;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using Blazorise.Extensions;

namespace DIP.Blazor.Pages.Managements.SubCategories
{
    public partial class EditSubCategories
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
  
        private SubCategoryCreateDto NewSubCategory { get; set; }
        private Validations NewSubCategoryValidations { get; set; } = new();
        private SubCategoryUpdateDto EditingSubCategory { get; set; }
        private Validations EditingSubCategoryValidations { get; set; } = new();
        private Guid EditingSubCategoryId { get; set; }
        private DataGridEntityActionsColumn<SubCategoryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private IReadOnlyList<LookupDto<Guid>> CategoriesCollection { get; set; } = new List<LookupDto<Guid>>();

        public EditSubCategories()
        {
            NewSubCategory = new SubCategoryCreateDto();
            EditingSubCategory = new SubCategoryUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetCategoryCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingSubCategoryId = Guid.Parse(Id);
                    var subCategory = await SubCategoriesAppService.GetAsync(EditingSubCategoryId);
                    EditingSubCategory = ObjectMapper.Map<SubCategoryDto, SubCategoryUpdateDto>(subCategory);

                    await EditingSubCategoryValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SubCategories"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }
        private async Task CreateSubCategoryAsync()
        {
            try
            {
                if (await NewSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await SubCategoriesAppService.CreateAsync(NewSubCategory);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateSubCategoryAsync()
        {
            try
            {
                if (await EditingSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await SubCategoriesAppService.UpdateAsync(EditingSubCategoryId, EditingSubCategory);
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
        

        private async Task GetCategoryCollectionLookupAsync(string? newValue = null)
        {
            CategoriesCollection = (await SubCategoriesAppService.GetCategoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }

        private async Task SetNewAsync()
        {
            GetSubCategoriesInput getSubCategoriesInput = new GetSubCategoriesInput();
            getSubCategoriesInput.MaxResultCount = 1;
            PagedResultDto<SubCategoryWithNavigationPropertiesDto> pagedResultDto = (await SubCategoriesAppService.GetListAsync(getSubCategoriesInput));
            if (pagedResultDto != null)
                NewSubCategory.Order = Convert.ToInt32(pagedResultDto.TotalCount) + 1;
            else
                NewSubCategory.Order = 1;
            NewSubCategory.IsActive = true;
            if (!CategoriesCollection.IsNullOrEmpty())
                NewSubCategory.CategoryId = CategoriesCollection.FirstOrDefault().Id;
        }
    }
}
