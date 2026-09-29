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
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using DIP.EFormServiceSubCategories;

namespace DIP.Blazor.Pages.Managements.EFormServices
{
    public partial class EditEFormServices
    {

        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


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
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";

        public EditEFormServices    ()
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
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingEFormServiceId = Guid.Parse(Id);
                    var eFormService = await EFormServicesAppService.GetAsync(EditingEFormServiceId);
                    EditingEFormService = ObjectMapper.Map<EFormServiceDto, EFormServiceUpdateDto>(eFormService);

                    await EditingEFormServiceValidations.ClearAll();
                }
                catch (Exception ex)
                {

                    //await uiMessageService.Error("Error in get data");
                    Logger.LogError(ex, "Error in get data");
                    //NavigationManager.NavigateTo("/page-informations");
                    NavigationManager.NavigateTo($"/{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
                    //{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}
                    await HandleErrorAsync(ex);
                }
            }
            else
                await SetNewAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EFormServices"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        //protected virtual ValueTask SetToolbarItemsAsync()
        //{
        //    Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
        //    Toolbar.AddButton(L["NewEFormService"], async () =>
        //    {
        //        await OpenCreateEFormServiceModalAsync();
        //    }, IconName.Add, requiredPolicyName: DIPPermissions.EFormServices.Create);

        //    return ValueTask.CompletedTask;
        //}

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

    

        private async Task CreateEFormServiceAsync()
        {
            try
            {
                if (await NewEFormServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await EFormServicesAppService.CreateAsync(NewEFormService);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
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
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
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
        private async Task SetNewAsync()
        {
            GetEFormServicesInput getEFormServicesInput = new GetEFormServicesInput();
            getEFormServicesInput.MaxResultCount = 1;
            PagedResultDto<EFormServiceDto> pagedResultDto = (await EFormServicesAppService.GetListAsync(getEFormServicesInput));
            if (pagedResultDto != null)
                NewEFormService.Order = Convert.ToInt32(pagedResultDto.TotalCount) + 1;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
    }
}
