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
using DIP.LastEventss;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages
{
    public partial class LastEventss
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<LastEventsDto> LastEventsList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateLastEvents { get; set; }
        private bool CanEditLastEvents { get; set; }
        private bool CanDeleteLastEvents { get; set; }
        private LastEventsCreateDto NewLastEvents { get; set; }
        private Validations NewLastEventsValidations { get; set; } = new();
        private LastEventsUpdateDto EditingLastEvents { get; set; }
        private Validations EditingLastEventsValidations { get; set; } = new();
        private Guid EditingLastEventsId { get; set; }
        private Modal CreateLastEventsModal { get; set; } = new();
        private Modal EditLastEventsModal { get; set; } = new();
        private GetLastEventssInput Filter { get; set; }
        private DataGridEntityActionsColumn<LastEventsDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "lastEvents-create-tab";
        protected string SelectedEditTab = "lastEvents-edit-tab";
        
        public LastEventss()
        {
            NewLastEvents = new LastEventsCreateDto();
            EditingLastEvents = new LastEventsUpdateDto();
            Filter = new GetLastEventssInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            LastEventsList = new List<LastEventsDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:LastEventss"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewLastEvents"], async () =>
            {
                await OpenCreateLastEventsModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.LastEventss.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateLastEvents = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.LastEventss.Create);
            CanEditLastEvents = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.LastEventss.Edit);
            CanDeleteLastEvents = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.LastEventss.Delete);
        }

        private async Task GetLastEventssAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await LastEventssAppService.GetListAsync(Filter);
            LastEventsList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetLastEventssAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await LastEventssAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/last-eventss/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<LastEventsDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetLastEventssAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateLastEventsModalAsync()
        {
            NewLastEvents = new LastEventsCreateDto{
                StartDate = DateTime.Now,
EndDate = DateTime.Now,

                
            };
            await NewLastEventsValidations.ClearAll();
            await CreateLastEventsModal.Show();
        }

        private async Task CloseCreateLastEventsModalAsync()
        {
            NewLastEvents = new LastEventsCreateDto{
                StartDate = DateTime.Now,
EndDate = DateTime.Now,

                
            };
            await CreateLastEventsModal.Hide();
        }

        private async Task OpenEditLastEventsModalAsync(LastEventsDto input)
        {
            var lastEvents = await LastEventssAppService.GetAsync(input.Id);
            
            EditingLastEventsId = lastEvents.Id;
            EditingLastEvents = ObjectMapper.Map<LastEventsDto, LastEventsUpdateDto>(lastEvents);
            await EditingLastEventsValidations.ClearAll();
            await EditLastEventsModal.Show();
        }

        private async Task DeleteLastEventsAsync(LastEventsDto input)
        {
            await LastEventssAppService.DeleteAsync(input.Id);

            await GetLastEventssAsync();
        }

        private async Task CreateLastEventsAsync()
        {
            try
            {
                if (await NewLastEventsValidations.ValidateAll() == false)
                {
                    return;
                }

                await LastEventssAppService.CreateAsync(NewLastEvents);
                await GetLastEventssAsync();
                await CloseCreateLastEventsModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditLastEventsModalAsync()
        {
            await EditLastEventsModal.Hide();
        }

        private async Task UpdateLastEventsAsync()
        {
            try
            {
                if (await EditingLastEventsValidations.ValidateAll() == false)
                {
                    return;
                }

                await LastEventssAppService.UpdateAsync(EditingLastEventsId, EditingLastEvents);
                await GetLastEventssAsync();
                await EditLastEventsModal.Hide();                
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
        

    }
}
