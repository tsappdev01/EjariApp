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
using DIP.SiteSettings;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class SiteSettings
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<SiteSettingDto> SiteSettingList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSiteSetting { get; set; }
        private bool CanEditSiteSetting { get; set; }
        private bool CanDeleteSiteSetting { get; set; }
        private SiteSettingCreateDto NewSiteSetting { get; set; }
        private Validations NewSiteSettingValidations { get; set; } = new();
        private SiteSettingUpdateDto EditingSiteSetting { get; set; }
        private Validations EditingSiteSettingValidations { get; set; } = new();
        private Guid EditingSiteSettingId { get; set; }
        private Modal CreateSiteSettingModal { get; set; } = new();
        private Modal EditSiteSettingModal { get; set; } = new();
        private GetSiteSettingsInput Filter { get; set; }
        private DataGridEntityActionsColumn<SiteSettingDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "siteSetting-create-tab";
        protected string SelectedEditTab = "siteSetting-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public SiteSettings()
        {
            NewSiteSetting = new SiteSettingCreateDto();
            EditingSiteSetting = new SiteSettingUpdateDto();
            Filter = new GetSiteSettingsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SiteSettingList = new List<SiteSettingDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SiteSettings"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSiteSetting"], async () =>
            {
                await OpenCreateSiteSettingModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.SiteSettings.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSiteSetting = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.SiteSettings.Create);
            CanEditSiteSetting = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SiteSettings.Edit);
            CanDeleteSiteSetting = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SiteSettings.Delete);
        }

        private async Task GetSiteSettingsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SiteSettingsAppService.GetListAsync(Filter);
            SiteSettingList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSiteSettingsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await SiteSettingsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/site-settings/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SiteSettingDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSiteSettingsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSiteSettingModalAsync()
        {
            NewSiteSetting = new SiteSettingCreateDto{
                
                
            };
            await NewSiteSettingValidations.ClearAll();
            await CreateSiteSettingModal.Show();
        }

        private async Task CloseCreateSiteSettingModalAsync()
        {
            NewSiteSetting = new SiteSettingCreateDto{
                
                
            };
            await CreateSiteSettingModal.Hide();
        }

        private async Task OpenEditSiteSettingModalAsync(SiteSettingDto input)
        {
            var siteSetting = await SiteSettingsAppService.GetAsync(input.Id);
            
            EditingSiteSettingId = siteSetting.Id;
            EditingSiteSetting = ObjectMapper.Map<SiteSettingDto, SiteSettingUpdateDto>(siteSetting);
            await EditingSiteSettingValidations.ClearAll();
            await EditSiteSettingModal.Show();
        }

        private async Task DeleteSiteSettingAsync(SiteSettingDto input)
        {
            await SiteSettingsAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetSiteSettingsAsync();
        }

        private async Task CreateSiteSettingAsync()
        {
            try
            {
                if (await NewSiteSettingValidations.ValidateAll() == false)
                {
                    return;
                }

                await SiteSettingsAppService.CreateAsync(NewSiteSetting);
                await GetSiteSettingsAsync();
                await CloseCreateSiteSettingModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSiteSettingModalAsync()
        {
            await EditSiteSettingModal.Hide();
        }

        private async Task UpdateSiteSettingAsync()
        {
            try
            {
                if (await EditingSiteSettingValidations.ValidateAll() == false)
                {
                    return;
                }

                await SiteSettingsAppService.UpdateAsync(EditingSiteSettingId, EditingSiteSetting);
                await GetSiteSettingsAsync();
                await EditSiteSettingModal.Hide();                
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
