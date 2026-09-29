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
using DIP.PageInfos;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
namespace DIP.Blazor.Pages.Managements.PagesInfos
{
    public partial class PageInfos
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<PageInfoDto> PageInfoList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePageInfo { get; set; }
        private bool CanEditPageInfo { get; set; }
        private bool CanDeletePageInfo { get; set; }
        private PageInfoCreateDto NewPageInfo { get; set; }
        private Validations NewPageInfoValidations { get; set; } = new();
        private PageInfoUpdateDto EditingPageInfo { get; set; }
        private Validations EditingPageInfoValidations { get; set; } = new();
        private Guid EditingPageInfoId { get; set; }
        private Modal CreatePageInfoModal { get; set; } = new();
        private Modal EditPageInfoModal { get; set; } = new();
        private GetPageInfosInput Filter { get; set; }
        private DataGridEntityActionsColumn<PageInfoDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "pageInfo-create-tab";
        protected string SelectedEditTab = "pageInfo-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public PageInfos()
        {
            NewPageInfo = new PageInfoCreateDto();
            EditingPageInfo = new PageInfoUpdateDto();
            Filter = new GetPageInfosInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PageInfoList = new List<PageInfoDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PageInfos"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPageInfo"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/page-infos/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.PageInfos.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePageInfo = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.PageInfos.Create);
            CanEditPageInfo = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PageInfos.Edit);
            CanDeletePageInfo = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PageInfos.Delete);
        }

        private async Task GetPageInfosAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PageInfosAppService.GetListAsync(Filter);
            PageInfoList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPageInfosAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await PageInfosAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/page-infos/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PageInfoDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPageInfosAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePageInfoModalAsync()
        {
            NewPageInfo = new PageInfoCreateDto{
                
                
            };
            await NewPageInfoValidations.ClearAll();
            await CreatePageInfoModal.Show();
        }

        private async Task CloseCreatePageInfoModalAsync()
        {
            NewPageInfo = new PageInfoCreateDto{
                
                
            };
            await CreatePageInfoModal.Hide();
        }

        private async Task OpenEditPageInfoModalAsync(PageInfoDto input)
        {
            var pageInfo = await PageInfosAppService.GetAsync(input.Id);
            
            EditingPageInfoId = pageInfo.Id;
            EditingPageInfo = ObjectMapper.Map<PageInfoDto, PageInfoUpdateDto>(pageInfo);
            await EditingPageInfoValidations.ClearAll();
            await EditPageInfoModal.Show();
        }

        private async Task DeletePageInfoAsync(PageInfoDto input)
        {
            await PageInfosAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetPageInfosAsync();
        }

        private async Task CreatePageInfoAsync()
        {
            try
            {
                if (await NewPageInfoValidations.ValidateAll() == false)
                {
                    return;
                }

                await PageInfosAppService.CreateAsync(NewPageInfo);
                await GetPageInfosAsync();
                await CloseCreatePageInfoModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPageInfoModalAsync()
        {
            await EditPageInfoModal.Hide();
        }

        private async Task UpdatePageInfoAsync()
        {
            try
            {
                if (await EditingPageInfoValidations.ValidateAll() == false)
                {
                    return;
                }

                await PageInfosAppService.UpdateAsync(EditingPageInfoId, EditingPageInfo);
                await GetPageInfosAsync();
                await EditPageInfoModal.Hide();                
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

        protected Task EditAsync(PageInfoDto input)
        {
            NavigationManager.NavigateTo($"/admin/page-infos/edit/{input.Id}");
            return Task.CompletedTask;
        }
    }
}
