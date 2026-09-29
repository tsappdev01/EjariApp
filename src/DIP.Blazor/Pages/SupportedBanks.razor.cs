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
using DIP.SupportedBanks;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class SupportedBanks
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<SupportedBankDto> SupportedBankList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSupportedBank { get; set; }
        private bool CanEditSupportedBank { get; set; }
        private bool CanDeleteSupportedBank { get; set; }
        private SupportedBankCreateDto NewSupportedBank { get; set; }
        private Validations NewSupportedBankValidations { get; set; } = new();
        private SupportedBankUpdateDto EditingSupportedBank { get; set; }
        private Validations EditingSupportedBankValidations { get; set; } = new();
        private Guid EditingSupportedBankId { get; set; }
        private Modal CreateSupportedBankModal { get; set; } = new();
        private Modal EditSupportedBankModal { get; set; } = new();
        private GetSupportedBanksInput Filter { get; set; }
        private DataGridEntityActionsColumn<SupportedBankDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "supportedBank-create-tab";
        protected string SelectedEditTab = "supportedBank-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public SupportedBanks()
        {
            NewSupportedBank = new SupportedBankCreateDto();
            EditingSupportedBank = new SupportedBankUpdateDto();
            Filter = new GetSupportedBanksInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SupportedBankList = new List<SupportedBankDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SupportedBanks"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSupportedBank"], async () =>
            {
                await OpenCreateSupportedBankModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.SupportedBanks.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSupportedBank = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.SupportedBanks.Create);
            CanEditSupportedBank = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SupportedBanks.Edit);
            CanDeleteSupportedBank = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SupportedBanks.Delete);
        }

        private async Task GetSupportedBanksAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SupportedBanksAppService.GetListAsync(Filter);
            SupportedBankList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSupportedBanksAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await SupportedBanksAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/supported-banks/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SupportedBankDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSupportedBanksAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSupportedBankModalAsync()
        {
            NewSupportedBank = new SupportedBankCreateDto{
                
                
            };
            await NewSupportedBankValidations.ClearAll();
            await CreateSupportedBankModal.Show();
        }

        private async Task CloseCreateSupportedBankModalAsync()
        {
            NewSupportedBank = new SupportedBankCreateDto{
                
                
            };
            await CreateSupportedBankModal.Hide();
        }

        private async Task OpenEditSupportedBankModalAsync(SupportedBankDto input)
        {
            var supportedBank = await SupportedBanksAppService.GetAsync(input.Id);
            
            EditingSupportedBankId = supportedBank.Id;
            EditingSupportedBank = ObjectMapper.Map<SupportedBankDto, SupportedBankUpdateDto>(supportedBank);
            await EditingSupportedBankValidations.ClearAll();
            await EditSupportedBankModal.Show();
        }

        private async Task DeleteSupportedBankAsync(SupportedBankDto input)
        {
            await SupportedBanksAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetSupportedBanksAsync();
        }

        private async Task CreateSupportedBankAsync()
        {
            try
            {
                if (await NewSupportedBankValidations.ValidateAll() == false)
                {
                    return;
                }

                await SupportedBanksAppService.CreateAsync(NewSupportedBank);
                await GetSupportedBanksAsync();
                await CloseCreateSupportedBankModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSupportedBankModalAsync()
        {
            await EditSupportedBankModal.Hide();
        }

        private async Task UpdateSupportedBankAsync()
        {
            try
            {
                if (await EditingSupportedBankValidations.ValidateAll() == false)
                {
                    return;
                }

                await SupportedBanksAppService.UpdateAsync(EditingSupportedBankId, EditingSupportedBank);
                await GetSupportedBanksAsync();
                await EditSupportedBankModal.Hide();                
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
