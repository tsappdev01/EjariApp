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
using DIP.Amenities;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.Amenities
{
    public partial class Amenities
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<AmenityDto> AmenityList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateAmenity { get; set; }
        private bool CanEditAmenity { get; set; }
        private bool CanDeleteAmenity { get; set; }
        private AmenityCreateDto NewAmenity { get; set; }
        private Validations NewAmenityValidations { get; set; } = new();
        private AmenityUpdateDto EditingAmenity { get; set; }
        private Validations EditingAmenityValidations { get; set; } = new();
        private Guid EditingAmenityId { get; set; }
        private Modal CreateAmenityModal { get; set; } = new();
        private Modal EditAmenityModal { get; set; } = new();
        private GetAmenitiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<AmenityDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "amenity-create-tab";
        protected string SelectedEditTab = "amenity-edit-tab";

        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public Amenities()
        {
            NewAmenity = new AmenityCreateDto();
            EditingAmenity = new AmenityUpdateDto();
            Filter = new GetAmenitiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            AmenityList = new List<AmenityDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Amenities"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewAmenity"], async () =>
            {
                // await OpenCreateAmenityModalAsync();
                NavigationManager.NavigateTo($"/admin/amenities/create/");

            }, IconName.Add, requiredPolicyName: DIPPermissions.Amenities.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAmenity = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Amenities.Create);
            CanEditAmenity = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Amenities.Edit);
            CanDeleteAmenity = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Amenities.Delete);
        }

        private async Task GetAmenitiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AmenitiesAppService.GetListAsync(Filter);
            AmenityList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await AmenitiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/amenities/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AmenityDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateAmenityModalAsync()
        {
            NewAmenity = new AmenityCreateDto{
                
                
            };
            await NewAmenityValidations.ClearAll();
            await CreateAmenityModal.Show();
        }

        private async Task CloseCreateAmenityModalAsync()
        {
            NewAmenity = new AmenityCreateDto{
                
                
            };
            await CreateAmenityModal.Hide();
        }

        private async Task OpenEditAmenityModalAsync(AmenityDto input)
        {
            var amenity = await AmenitiesAppService.GetAsync(input.Id);
            
            EditingAmenityId = amenity.Id;
            EditingAmenity = ObjectMapper.Map<AmenityDto, AmenityUpdateDto>(amenity);
            await EditingAmenityValidations.ClearAll();
            await EditAmenityModal.Show();
        }

        private async Task DeleteAmenityAsync(AmenityDto input)
        {
            await AmenitiesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetAmenitiesAsync();
        }

        private async Task CreateAmenityAsync()
        {
            try
            {
                if (await NewAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await AmenitiesAppService.CreateAsync(NewAmenity);
                await GetAmenitiesAsync();
                await CloseCreateAmenityModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditAmenityModalAsync()
        {
            await EditAmenityModal.Hide();
        }

        private async Task UpdateAmenityAsync()
        {
            try
            {
                if (await EditingAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await AmenitiesAppService.UpdateAsync(EditingAmenityId, EditingAmenity);
                await GetAmenitiesAsync();
                await EditAmenityModal.Hide();                
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
        protected Task EditAsync(AmenityDto input)
        {
            NavigationManager.NavigateTo($"/admin/amenities/edit/{input.Id}");
            return Task.CompletedTask;
        }

    }
}
