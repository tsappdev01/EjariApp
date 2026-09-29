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
using DIP.TimeLines;
using DIP.Permissions;
using DIP.Shared;
using DIP.ZoneParagraphs;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.TimeLines
{
    public partial class TimeLines
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private IReadOnlyList<TimeLineWithNavigationPropertiesDto> TimeLineList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateTimeLine { get; set; }
        private bool CanEditTimeLine { get; set; }
        private bool CanDeleteTimeLine { get; set; }
        private GetTimeLinesInput Filter { get; set; }
        private DataGridEntityActionsColumn<TimeLineWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        private IReadOnlyList<LookupDto<Guid>> TimeLineCategoriesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public TimeLines()
        {
            Filter = new GetTimeLinesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            TimeLineList = new List<TimeLineWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetTimeLineCategoryCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:TimeLines"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

            Toolbar.AddButton(L["NewTimeLine"], async () =>
            {
                //  await OpenCreateTimeLineModalAsync();
                NavigationManager.NavigateTo($"/admin/time-lines/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.TimeLines.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTimeLine = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.TimeLines.Create);
            CanEditTimeLine = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.TimeLines.Edit);
            CanDeleteTimeLine = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.TimeLines.Delete);
        }

        private async Task GetTimeLinesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await TimeLinesAppService.GetListAsync(Filter);
            TimeLineList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetTimeLinesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await TimeLinesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/time-lines/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TimeLineWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetTimeLinesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteTimeLineAsync(TimeLineWithNavigationPropertiesDto input)
        {
            await TimeLinesAppService.DeleteAsync(input.TimeLine.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);


            await GetTimeLinesAsync();
        }

        private async Task GetTimeLineCategoryCollectionLookupAsync(string? newValue = null)
        {
            TimeLineCategoriesCollection = (await TimeLinesAppService.GetTimeLineCategoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        protected Task EditAsync(TimeLineWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/time-lines/edit/{input.TimeLine.Id}");
            return Task.CompletedTask;
        }

    }
}
