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
using DIP.EFormServices;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.EFormServices
{
    public partial class EFormServices
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<EFormServiceDto> EFormServiceList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEFormService { get; set; }
        private bool CanEditEFormService { get; set; }
        private bool CanDeleteEFormService { get; set; }
        private EFormServiceCreateDto NewEFormService { get; set; }
        private Validations NewEFormServiceValidations { get; set; } = new();
        private EFormServiceUpdateDto EditingEFormService { get; set; }
        private Validations EditingEFormServiceValidations { get; set; } = new();
        private Guid EditingEFormServiceId { get; set; }
        private Modal CreateEFormServiceModal { get; set; } = new();
        private Modal EditEFormServiceModal { get; set; } = new();
        private GetEFormServicesInput Filter { get; set; }
        private DataGridEntityActionsColumn<EFormServiceDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "eFormService-create-tab";
        protected string SelectedEditTab = "eFormService-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public EFormServices()
        {
            NewEFormService = new EFormServiceCreateDto();
            EditingEFormService = new EFormServiceUpdateDto();
            Filter = new GetEFormServicesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EFormServiceList = new List<EFormServiceDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EFormServices"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewEFormService"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/e-form-services/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.EFormServices.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEFormService = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.EFormServices.Create);
            CanEditEFormService = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServices.Edit);
            CanDeleteEFormService = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServices.Delete);
        }

        private async Task GetEFormServicesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EFormServicesAppService.GetListAsync(Filter);
            EFormServiceList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEFormServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await EFormServicesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/e-form-services/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EFormServiceDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEFormServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEFormServiceModalAsync()
        {
            NewEFormService = new EFormServiceCreateDto{
                
                
            };
            await NewEFormServiceValidations.ClearAll();
            await CreateEFormServiceModal.Show();
        }

        private async Task CloseCreateEFormServiceModalAsync()
        {
            NewEFormService = new EFormServiceCreateDto{
                
                
            };
            await CreateEFormServiceModal.Hide();
        }

        private async Task OpenEditEFormServiceModalAsync(EFormServiceDto input)
        {
            var eFormService = await EFormServicesAppService.GetAsync(input.Id);
            
            EditingEFormServiceId = eFormService.Id;
            EditingEFormService = ObjectMapper.Map<EFormServiceDto, EFormServiceUpdateDto>(eFormService);
            await EditingEFormServiceValidations.ClearAll();
            await EditEFormServiceModal.Show();
        }

        private async Task DeleteEFormServiceAsync(EFormServiceDto input)
        {
            await EFormServicesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetEFormServicesAsync();
        }

        private async Task CreateEFormServiceAsync()
        {
            try
            {
                if (await NewEFormServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await EFormServicesAppService.CreateAsync(NewEFormService);
                await GetEFormServicesAsync();
                await CloseCreateEFormServiceModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEFormServiceModalAsync()
        {
            await EditEFormServiceModal.Hide();
        }

        private async Task UpdateEFormServiceAsync()
        {
            try
            {
                if (await EditingEFormServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await EFormServicesAppService.UpdateAsync(EditingEFormServiceId, EditingEFormService);
                await GetEFormServicesAsync();
                await EditEFormServiceModal.Hide();                
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
        protected Task EditAsync(EFormServiceDto input)
        {
            NavigationManager.NavigateTo($"/admin/e-form-services/edit/{input.Id}");
            return Task.CompletedTask;
        }

    }
}
