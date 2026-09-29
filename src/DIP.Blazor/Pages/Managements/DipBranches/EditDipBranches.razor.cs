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
using DIP.DipBranches;
using DIP.Permissions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.DipBranches
{
    public partial class EditDipBranches
    {


        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<DipBranchDto> DipBranchList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDipBranch { get; set; }
        private bool CanEditDipBranch { get; set; }
        private bool CanDeleteDipBranch { get; set; }
        private DipBranchCreateDto NewDipBranch { get; set; }
        private Validations NewDipBranchValidations { get; set; } = new();
        private DipBranchUpdateDto EditingDipBranch { get; set; }
        private Validations EditingDipBranchValidations { get; set; } = new();
        private Guid EditingDipBranchId { get; set; }
        private Modal CreateDipBranchModal { get; set; } = new();
        private Modal EditDipBranchModal { get; set; } = new();
        private GetDipBranchesInput Filter { get; set; }
        private DataGridEntityActionsColumn<DipBranchDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
   
        public EditDipBranches()
        {
            NewDipBranch = new DipBranchCreateDto();
            EditingDipBranch = new DipBranchUpdateDto();
            Filter = new GetDipBranchesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DipBranchList = new List<DipBranchDto>();
        }

        protected override async Task OnInitializedAsync()
        {
          
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();

            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingDipBranchId = Guid.Parse(Id);
                    var dipBranch= await DipBranchesAppService.GetAsync(EditingDipBranchId);
                    EditingDipBranch = ObjectMapper.Map<DipBranchDto, DipBranchUpdateDto>(dipBranch);

                    await EditingDipBranchValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:DipBranches"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewDipBranch"], async () =>
            {
                await OpenCreateDipBranchModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.DipBranches.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDipBranch = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.DipBranches.Create);
            CanEditDipBranch = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipBranches.Edit);
            CanDeleteDipBranch = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipBranches.Delete);
        }

        private async Task GetDipBranchesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DipBranchesAppService.GetListAsync(Filter);
            DipBranchList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDipBranchesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await DipBranchesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dip-branches/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DipBranchDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDipBranchesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDipBranchModalAsync()
        {
            NewDipBranch = new DipBranchCreateDto{
                
                
            };
            await NewDipBranchValidations.ClearAll();
            await CreateDipBranchModal.Show();
        }

        private async Task CloseCreateDipBranchModalAsync()
        {
            NewDipBranch = new DipBranchCreateDto{
                
                
            };
            await CreateDipBranchModal.Hide();
        }

        private async Task OpenEditDipBranchModalAsync(DipBranchDto input)
        {
            var dipBranch = await DipBranchesAppService.GetAsync(input.Id);
            
            EditingDipBranchId = dipBranch.Id;
            EditingDipBranch = ObjectMapper.Map<DipBranchDto, DipBranchUpdateDto>(dipBranch);
            await EditingDipBranchValidations.ClearAll();
            await EditDipBranchModal.Show();
        }

      

        private async Task CreateDipBranchAsync()
        {
            try
            {
                if (await NewDipBranchValidations.ValidateAll() == false)
                {
                    return;
                }

                await DipBranchesAppService.CreateAsync(NewDipBranch);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDipBranchModalAsync()
        {
            await EditDipBranchModal.Hide();
        }

        private async Task UpdateDipBranchAsync()
        {
            try
            {
                if (await EditingDipBranchValidations.ValidateAll() == false)
                {
                    return;
                }

                await DipBranchesAppService.UpdateAsync(EditingDipBranchId, EditingDipBranch);
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
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
        private async Task SetNewAsync()
        {
            GetDipBranchesInput getDipBranchesInput = new GetDipBranchesInput();
            getDipBranchesInput.MaxResultCount = 1;
            PagedResultDto<DipBranchDto> dipBranches = (await DipBranchesAppService.GetListAsync(getDipBranchesInput));
            if (dipBranches != null)
                NewDipBranch.Order = Convert.ToInt32(dipBranches.TotalCount);
            else
                NewDipBranch.Order = 0;
            NewDipBranch.IsActive = true;
        }

    }
}
