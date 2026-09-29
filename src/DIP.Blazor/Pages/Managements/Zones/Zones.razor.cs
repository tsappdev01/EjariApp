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
using DIP.Zones;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.Zones
{
    public partial class Zones
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<ZoneDto> ZoneList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateZone { get; set; }
        private bool CanEditZone { get; set; }
        private bool CanDeleteZone { get; set; }
        private GetZonesInput Filter { get; set; }
        private DataGridEntityActionsColumn<ZoneDto> EntityActionsColumn { get; set; } = new();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public Zones()
        {
            Filter = new GetZonesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ZoneList = new List<ZoneDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Zones"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewZone"], async () =>
            {
                //  await OpenCreateZoneModalAsync();
                NavigationManager.NavigateTo($"/admin/zones/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.Zones.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateZone = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Zones.Create);
            CanEditZone = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Zones.Edit);
            CanDeleteZone = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Zones.Delete);
        }

        private async Task GetZonesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ZonesAppService.GetListAsync(Filter);
            ZoneList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetZonesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await ZonesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/zones/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ZoneDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetZonesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteZoneAsync(ZoneDto input)
        {
            await ZonesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetZonesAsync();
        }

        protected Task EditAsync(ZoneDto input)
        {
            NavigationManager.NavigateTo($"/admin/zones/edit/{input.Id}");
            return Task.CompletedTask;
        }
    }
}
