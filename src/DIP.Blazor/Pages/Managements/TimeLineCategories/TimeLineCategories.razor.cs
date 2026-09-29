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
using DIP.Zones;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.TimeLineCategories
{
    public partial class TimeLineCategories
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<TimeLineCategoryDto> TimeLineCategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateTimeLineCategory { get; set; }
        private bool CanEditTimeLineCategory { get; set; }
        private bool CanDeleteTimeLineCategory { get; set; }
        private GetTimeLineCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<TimeLineCategoryDto> EntityActionsColumn { get; set; } = new();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public TimeLineCategories()
        {
            Filter = new GetTimeLineCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            TimeLineCategoryList = new List<TimeLineCategoryDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:TimeLineCategories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewTimeLineCategory"], async () =>
            {
                // await OpenCreateTimeLineCategoryModalAsync();
                NavigationManager.NavigateTo($"/admin/time-line-categories/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.TimeLineCategories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTimeLineCategory = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.TimeLineCategories.Create);
            CanEditTimeLineCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.TimeLineCategories.Edit);
            CanDeleteTimeLineCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.TimeLineCategories.Delete);
        }

        private async Task GetTimeLineCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await TimeLineCategoriesAppService.GetListAsync(Filter);
            TimeLineCategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetTimeLineCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await TimeLineCategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/time-line-categories/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TimeLineCategoryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetTimeLineCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteTimeLineCategoryAsync(TimeLineCategoryDto input)
        {
            await TimeLineCategoriesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetTimeLineCategoriesAsync();
        }
        protected Task EditAsync(TimeLineCategoryDto input)
        {
            NavigationManager.NavigateTo($"/admin/time-line-categories/edit/{input.Id}");
            return Task.CompletedTask;
        }

    }
}
