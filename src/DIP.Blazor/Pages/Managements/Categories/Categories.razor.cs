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
using DIP.Categories;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.Categories
{
    public partial class Categories
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<CategoryDto> CategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCategory { get; set; }
        private bool CanEditCategory { get; set; }
        private bool CanDeleteCategory { get; set; }
        private CategoryCreateDto NewCategory { get; set; }
        private Validations NewCategoryValidations { get; set; } = new();
        private CategoryUpdateDto EditingCategory { get; set; }
        private Validations EditingCategoryValidations { get; set; } = new();
        private Guid EditingCategoryId { get; set; }
        private Modal CreateCategoryModal { get; set; } = new();
        private Modal EditCategoryModal { get; set; } = new();
        private GetCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<CategoryDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "category-create-tab";
        protected string SelectedEditTab = "category-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public Categories()
        {
            NewCategory = new CategoryCreateDto();
            EditingCategory = new CategoryUpdateDto();
            Filter = new GetCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CategoryList = new List<CategoryDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Categories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCategory"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/categories/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.Categories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCategory = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Categories.Create);
            CanEditCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Categories.Edit);
            CanDeleteCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Categories.Delete);
        }

        private async Task GetCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CategoriesAppService.GetListAsync(Filter);
            CategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await CategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/categories/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CategoryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCategoryModalAsync()
        {
            NewCategory = new CategoryCreateDto{
                
                
            };
            await NewCategoryValidations.ClearAll();
            await CreateCategoryModal.Show();
        }

        private async Task CloseCreateCategoryModalAsync()
        {
            NewCategory = new CategoryCreateDto{
                
                
            };
            await CreateCategoryModal.Hide();
        }

        private async Task OpenEditCategoryModalAsync(CategoryDto input)
        {
            var category = await CategoriesAppService.GetAsync(input.Id);
            
            EditingCategoryId = category.Id;
            EditingCategory = ObjectMapper.Map<CategoryDto, CategoryUpdateDto>(category);
            await EditingCategoryValidations.ClearAll();
            await EditCategoryModal.Show();
        }

        private async Task DeleteCategoryAsync(CategoryDto input)
        {
            await CategoriesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetCategoriesAsync();
        }

        private async Task CreateCategoryAsync()
        {
            try
            {
                if (await NewCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.CreateAsync(NewCategory);
                await GetCategoriesAsync();
                await CloseCreateCategoryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCategoryModalAsync()
        {
            await EditCategoryModal.Hide();
        }

        private async Task UpdateCategoryAsync()
        {
            try
            {
                if (await EditingCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.UpdateAsync(EditingCategoryId, EditingCategory);
                await GetCategoriesAsync();
                await EditCategoryModal.Hide();                
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
        protected Task EditAsync(CategoryDto input)
        {
            NavigationManager.NavigateTo($"/admin/categories/edit/{input.Id}");
            return Task.CompletedTask;
        }

    }
}
