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
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.SubCategories
{
    public partial class SubCategories
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<SubCategoryWithNavigationPropertiesDto> SubCategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSubCategory { get; set; }
        private bool CanEditSubCategory { get; set; }
        private bool CanDeleteSubCategory { get; set; }
        private SubCategoryCreateDto NewSubCategory { get; set; }
        private Validations NewSubCategoryValidations { get; set; } = new();
        private SubCategoryUpdateDto EditingSubCategory { get; set; }
        private Validations EditingSubCategoryValidations { get; set; } = new();
        private Guid EditingSubCategoryId { get; set; }
        private Modal CreateSubCategoryModal { get; set; } = new();
        private Modal EditSubCategoryModal { get; set; } = new();
        private GetSubCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<SubCategoryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "subCategory-create-tab";
        protected string SelectedEditTab = "subCategory-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> CategoriesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public SubCategories()
        {
            NewSubCategory = new SubCategoryCreateDto();
            EditingSubCategory = new SubCategoryUpdateDto();
            Filter = new GetSubCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SubCategoryList = new List<SubCategoryWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetCategoryCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SubCategories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSubCategory"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/sub-categories/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.SubCategories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSubCategory = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.SubCategories.Create);
            CanEditSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SubCategories.Edit);
            CanDeleteSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SubCategories.Delete);
        }

        private async Task GetSubCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SubCategoriesAppService.GetListAsync(Filter);
            SubCategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await SubCategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/sub-categories/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SubCategoryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSubCategoryModalAsync()
        {
            NewSubCategory = new SubCategoryCreateDto{
                
                CategoryId = CategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewSubCategoryValidations.ClearAll();
            await CreateSubCategoryModal.Show();
        }

        private async Task CloseCreateSubCategoryModalAsync()
        {
            NewSubCategory = new SubCategoryCreateDto{
                
                CategoryId = CategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateSubCategoryModal.Hide();
        }

        private async Task OpenEditSubCategoryModalAsync(SubCategoryWithNavigationPropertiesDto input)
        {
            var subCategory = await SubCategoriesAppService.GetWithNavigationPropertiesAsync(input.SubCategory.Id);
            
            EditingSubCategoryId = subCategory.SubCategory.Id;
            EditingSubCategory = ObjectMapper.Map<SubCategoryDto, SubCategoryUpdateDto>(subCategory.SubCategory);
            await EditingSubCategoryValidations.ClearAll();
            await EditSubCategoryModal.Show();
        }

        private async Task DeleteSubCategoryAsync(SubCategoryWithNavigationPropertiesDto input)
        {
            await SubCategoriesAppService.DeleteAsync(input.SubCategory.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);


            await GetSubCategoriesAsync();
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
                await GetSubCategoriesAsync();
                await CloseCreateSubCategoryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSubCategoryModalAsync()
        {
            await EditSubCategoryModal.Hide();
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
                await GetSubCategoriesAsync();
                await EditSubCategoryModal.Hide();                
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
        protected Task EditAsync(SubCategoryWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/sub-categories/edit/{input.SubCategory.Id}");
            return Task.CompletedTask;
        }
    }
}
