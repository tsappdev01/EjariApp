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
using DIP.EFormServiceSubCategories;
using DIP.Permissions;
using DIP.Shared;

namespace DIP.Blazor.Pages
{
    public partial class EFormServiceSubCategories
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<EFormServiceSubCategoryWithNavigationPropertiesDto> EFormServiceSubCategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEFormServiceSubCategory { get; set; }
        private bool CanEditEFormServiceSubCategory { get; set; }
        private bool CanDeleteEFormServiceSubCategory { get; set; }
        private EFormServiceSubCategoryCreateDto NewEFormServiceSubCategory { get; set; }
        private Validations NewEFormServiceSubCategoryValidations { get; set; } = new();
        private EFormServiceSubCategoryUpdateDto EditingEFormServiceSubCategory { get; set; }
        private Validations EditingEFormServiceSubCategoryValidations { get; set; } = new();
        private Guid EditingEFormServiceSubCategoryId { get; set; }
        private Modal CreateEFormServiceSubCategoryModal { get; set; } = new();
        private Modal EditEFormServiceSubCategoryModal { get; set; } = new();
        private GetEFormServiceSubCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<EFormServiceSubCategoryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "eFormServiceSubCategory-create-tab";
        protected string SelectedEditTab = "eFormServiceSubCategory-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> EFormServicesCollection { get; set; } = new List<LookupDto<Guid>>();

        public EFormServiceSubCategories()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto();
            EditingEFormServiceSubCategory = new EFormServiceSubCategoryUpdateDto();
            Filter = new GetEFormServiceSubCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EFormServiceSubCategoryList = new List<EFormServiceSubCategoryWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EFormServiceSubCategories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewEFormServiceSubCategory"], async () =>
            {
                await OpenCreateEFormServiceSubCategoryModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.EFormServiceSubCategories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEFormServiceSubCategory = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Create);
            CanEditEFormServiceSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Edit);
            CanDeleteEFormServiceSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Delete);
        }

        private async Task GetEFormServiceSubCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EFormServiceSubCategoriesAppService.GetListAsync(Filter);
            EFormServiceSubCategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEFormServiceSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await EFormServiceSubCategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/e-form-service-sub-categories/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EFormServiceSubCategoryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEFormServiceSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEFormServiceSubCategoryModalAsync()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto{
                
                
            };
            await NewEFormServiceSubCategoryValidations.ClearAll();
            await CreateEFormServiceSubCategoryModal.Show();
        }

        private async Task CloseCreateEFormServiceSubCategoryModalAsync()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto{
                
                
            };
            await CreateEFormServiceSubCategoryModal.Hide();
        }

        private async Task OpenEditEFormServiceSubCategoryModalAsync(EFormServiceSubCategoryWithNavigationPropertiesDto input)
        {
            var eFormServiceSubCategory = await EFormServiceSubCategoriesAppService.GetWithNavigationPropertiesAsync(input.EFormServiceSubCategory.Id);
            
            EditingEFormServiceSubCategoryId = eFormServiceSubCategory.EFormServiceSubCategory.Id;
            EditingEFormServiceSubCategory = ObjectMapper.Map<EFormServiceSubCategoryDto, EFormServiceSubCategoryUpdateDto>(eFormServiceSubCategory.EFormServiceSubCategory);
            await EditingEFormServiceSubCategoryValidations.ClearAll();
            await EditEFormServiceSubCategoryModal.Show();
        }

        private async Task DeleteEFormServiceSubCategoryAsync(EFormServiceSubCategoryWithNavigationPropertiesDto input)
        {
            await EFormServiceSubCategoriesAppService.DeleteAsync(input.EFormServiceSubCategory.Id);
            await GetEFormServiceSubCategoriesAsync();
        }

        private async Task CreateEFormServiceSubCategoryAsync()
        {
            try
            {
                if (await NewEFormServiceSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await EFormServiceSubCategoriesAppService.CreateAsync(NewEFormServiceSubCategory);
                await GetEFormServiceSubCategoriesAsync();
                await CloseCreateEFormServiceSubCategoryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEFormServiceSubCategoryModalAsync()
        {
            await EditEFormServiceSubCategoryModal.Hide();
        }

        private async Task UpdateEFormServiceSubCategoryAsync()
        {
            try
            {
                if (await EditingEFormServiceSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await EFormServiceSubCategoriesAppService.UpdateAsync(EditingEFormServiceSubCategoryId, EditingEFormServiceSubCategory);
                await GetEFormServiceSubCategoriesAsync();
                await EditEFormServiceSubCategoryModal.Hide();                
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
        

        private async Task GetEFormServiceCollectionLookupAsync(string? newValue = null)
        {
            EFormServicesCollection = (await EFormServiceSubCategoriesAppService.GetEFormServiceLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
