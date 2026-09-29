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
using DIP.ZoneParagraphs;
using DIP.Permissions;
using DIP.Shared;
using DIP.Zones;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.ZoneParagraphs
{
    public partial class ZoneParagraphs
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<ZoneParagraphWithNavigationPropertiesDto> ZoneParagraphList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateZoneParagraph { get; set; }
        private bool CanEditZoneParagraph { get; set; }
        private bool CanDeleteZoneParagraph { get; set; }
        private ZoneParagraphCreateDto NewZoneParagraph { get; set; }
        private Validations NewZoneParagraphValidations { get; set; } = new();
        private ZoneParagraphUpdateDto EditingZoneParagraph { get; set; }
        private Validations EditingZoneParagraphValidations { get; set; } = new();
        private Guid EditingZoneParagraphId { get; set; }
        private Modal CreateZoneParagraphModal { get; set; } = new();
        private Modal EditZoneParagraphModal { get; set; } = new();
        private GetZoneParagraphsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ZoneParagraphWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "zoneParagraph-create-tab";
        protected string SelectedEditTab = "zoneParagraph-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> ZonesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public ZoneParagraphs()
        {
            NewZoneParagraph = new ZoneParagraphCreateDto();
            EditingZoneParagraph = new ZoneParagraphUpdateDto();
            Filter = new GetZoneParagraphsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ZoneParagraphList = new List<ZoneParagraphWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetZoneCollectionLookupAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ZoneParagraphs"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewZoneParagraph"], async () =>
            {
                //  await OpenCreateZoneParagraphModalAsync();
                NavigationManager.NavigateTo($"/admin/zone-paragraphs/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.ZoneParagraphs.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateZoneParagraph = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.ZoneParagraphs.Create);
            CanEditZoneParagraph = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.ZoneParagraphs.Edit);
            CanDeleteZoneParagraph = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.ZoneParagraphs.Delete);
        }

        private async Task GetZoneParagraphsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ZoneParagraphsAppService.GetListAsync(Filter);
            ZoneParagraphList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetZoneParagraphsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await ZoneParagraphsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/zone-paragraphs/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ZoneParagraphWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetZoneParagraphsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateZoneParagraphModalAsync()
        {
            NewZoneParagraph = new ZoneParagraphCreateDto{
                
                
            };
            await NewZoneParagraphValidations.ClearAll();
            await CreateZoneParagraphModal.Show();
        }

        private async Task CloseCreateZoneParagraphModalAsync()
        {
            NewZoneParagraph = new ZoneParagraphCreateDto{
                
                
            };
            await CreateZoneParagraphModal.Hide();
        }

        private async Task DeleteZoneParagraphAsync(ZoneParagraphWithNavigationPropertiesDto input)
        {
            await ZoneParagraphsAppService.DeleteAsync(input.ZoneParagraph.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);


            await GetZoneParagraphsAsync();
        }

        private async Task CreateZoneParagraphAsync()
        {
            try
            {
                if (await NewZoneParagraphValidations.ValidateAll() == false)
                {
                    return;
                }

                await ZoneParagraphsAppService.CreateAsync(NewZoneParagraph);
                await GetZoneParagraphsAsync();
                await CloseCreateZoneParagraphModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditZoneParagraphModalAsync()
        {
            await EditZoneParagraphModal.Hide();
        }

        private async Task UpdateZoneParagraphAsync()
        {
            try
            {
                if (await EditingZoneParagraphValidations.ValidateAll() == false)
                {
                    return;
                }

                await ZoneParagraphsAppService.UpdateAsync(EditingZoneParagraphId, EditingZoneParagraph);
                await GetZoneParagraphsAsync();
                await EditZoneParagraphModal.Hide();                
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
        

        private async Task GetZoneCollectionLookupAsync(string? newValue = null)
        {
            ZonesCollection = (await ZoneParagraphsAppService.GetZoneLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        protected Task EditAsync(ZoneParagraphWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/zone-paragraphs/edit/{input.ZoneParagraph.Id}");
            return Task.CompletedTask;
        }

        protected Task LinkMedias(ZoneParagraphWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/zone-paragraphs/medias/{input.ZoneParagraph.Id}");
            return Task.CompletedTask;
        }

    }
}
