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
using DIP.EServices;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.EServices
{
    public partial class EServices
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<EServiceDto> EServiceList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEService { get; set; }
        private bool CanEditEService { get; set; }
        private bool CanDeleteEService { get; set; }
        private EServiceCreateDto NewEService { get; set; }
        private Validations NewEServiceValidations { get; set; } = new();
        private EServiceUpdateDto EditingEService { get; set; }
        private Validations EditingEServiceValidations { get; set; } = new();
        private Guid EditingEServiceId { get; set; }
        private Modal CreateEServiceModal { get; set; } = new();
        private Modal EditEServiceModal { get; set; } = new();
        private GetEServicesInput Filter { get; set; }
        private DataGridEntityActionsColumn<EServiceDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "eService-create-tab";
        protected string SelectedEditTab = "eService-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public EServices()
        {
            NewEService = new EServiceCreateDto();
            EditingEService = new EServiceUpdateDto();
            Filter = new GetEServicesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EServiceList = new List<EServiceDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EServices"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewEService"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/e-services/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.EServices.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEService = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.EServices.Create);
            CanEditEService = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EServices.Edit);
            CanDeleteEService = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EServices.Delete);
        }

        private async Task GetEServicesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EServicesAppService.GetListAsync(Filter);
            EServiceList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await EServicesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/e-services/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EServiceDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEServiceModalAsync()
        {
            NewEService = new EServiceCreateDto{
                
                
            };
            await NewEServiceValidations.ClearAll();
            await CreateEServiceModal.Show();
        }

        private async Task CloseCreateEServiceModalAsync()
        {
            NewEService = new EServiceCreateDto{
                
                
            };
            await CreateEServiceModal.Hide();
        }

        private async Task OpenEditEServiceModalAsync(EServiceDto input)
        {
            var eService = await EServicesAppService.GetAsync(input.Id);
            
            EditingEServiceId = eService.Id;
            EditingEService = ObjectMapper.Map<EServiceDto, EServiceUpdateDto>(eService);
            await EditingEServiceValidations.ClearAll();
            await EditEServiceModal.Show();
        }

        private async Task DeleteEServiceAsync(EServiceDto input)
        {
            await EServicesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetEServicesAsync();
        }

        private async Task CreateEServiceAsync()
        {
            try
            {
                if (await NewEServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await EServicesAppService.CreateAsync(NewEService);
                await GetEServicesAsync();
                await CloseCreateEServiceModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEServiceModalAsync()
        {
            await EditEServiceModal.Hide();
        }

        private async Task UpdateEServiceAsync()
        {
            try
            {
                if (await EditingEServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await EServicesAppService.UpdateAsync(EditingEServiceId, EditingEService);
                await GetEServicesAsync();
                await EditEServiceModal.Hide();                
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
        protected Task EditAsync(EServiceDto input)
        {
            NavigationManager.NavigateTo($"/admin/e-services/edit/{input.Id}");
            return Task.CompletedTask;
        }

    }
}
