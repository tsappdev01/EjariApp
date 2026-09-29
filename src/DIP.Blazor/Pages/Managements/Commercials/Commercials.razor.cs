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
using DIP.Commercials;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.Commercials
{
    public partial class Commercials
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<CommercialWithNavigationPropertiesDto> CommercialList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCommercial { get; set; }
        private bool CanEditCommercial { get; set; }
        private bool CanDeleteCommercial { get; set; }
        private CommercialCreateDto NewCommercial { get; set; }
        private Validations NewCommercialValidations { get; set; } = new();
        private CommercialUpdateDto EditingCommercial { get; set; }
        private Validations EditingCommercialValidations { get; set; } = new();
        private Guid EditingCommercialId { get; set; }
        private Modal CreateCommercialModal { get; set; } = new();
        private Modal EditCommercialModal { get; set; } = new();
        private GetCommercialsInput Filter { get; set; }
        private DataGridEntityActionsColumn<CommercialWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "commercial-create-tab";
        protected string SelectedEditTab = "commercial-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> SubCategoriesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public Commercials()
        {
            NewCommercial = new CommercialCreateDto();
            EditingCommercial = new CommercialUpdateDto();
            Filter = new GetCommercialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CommercialList = new List<CommercialWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetSubCategoryCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Commercials"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCommercial"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/commercials/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.Commercials.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCommercial = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Commercials.Create);
            CanEditCommercial = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Commercials.Edit);
            CanDeleteCommercial = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Commercials.Delete);
        }

        private async Task GetCommercialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CommercialsAppService.GetListAsync(Filter);
            CommercialList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCommercialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await CommercialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/commercials/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CommercialWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCommercialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCommercialModalAsync()
        {
            NewCommercial = new CommercialCreateDto{
                
                SubCategoryId = SubCategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewCommercialValidations.ClearAll();
            await CreateCommercialModal.Show();
        }

        private async Task CloseCreateCommercialModalAsync()
        {
            NewCommercial = new CommercialCreateDto{
                
                SubCategoryId = SubCategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateCommercialModal.Hide();
        }

        private async Task OpenEditCommercialModalAsync(CommercialWithNavigationPropertiesDto input)
        {
            var commercial = await CommercialsAppService.GetWithNavigationPropertiesAsync(input.Commercial.Id);
            
            EditingCommercialId = commercial.Commercial.Id;
            EditingCommercial = ObjectMapper.Map<CommercialDto, CommercialUpdateDto>(commercial.Commercial);
            await EditingCommercialValidations.ClearAll();
            await EditCommercialModal.Show();
        }

        private async Task DeleteCommercialAsync(CommercialWithNavigationPropertiesDto input)
        {
            await CommercialsAppService.DeleteAsync(input.Commercial.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);


            await GetCommercialsAsync();
        }

        private async Task CreateCommercialAsync()
        {
            try
            {
                if (await NewCommercialValidations.ValidateAll() == false)
                {
                    return;
                }

                await CommercialsAppService.CreateAsync(NewCommercial);
                await GetCommercialsAsync();
                await CloseCreateCommercialModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCommercialModalAsync()
        {
            await EditCommercialModal.Hide();
        }

        private async Task UpdateCommercialAsync()
        {
            try
            {
                if (await EditingCommercialValidations.ValidateAll() == false)
                {
                    return;
                }

                await CommercialsAppService.UpdateAsync(EditingCommercialId, EditingCommercial);
                await GetCommercialsAsync();
                await EditCommercialModal.Hide();                
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
        

        private async Task GetSubCategoryCollectionLookupAsync(string? newValue = null)
        {
            SubCategoriesCollection = (await CommercialsAppService.GetSubCategoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        protected Task EditAsync(CommercialWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/commercials/edit/{input.Commercial.Id}");
            return Task.CompletedTask;
        }

    }
}
