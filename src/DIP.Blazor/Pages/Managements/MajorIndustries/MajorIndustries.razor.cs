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
using DIP.MajorIndustries;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.MajorIndustries
{
    public partial class MajorIndustries
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<MajorIndustryWithNavigationPropertiesDto> MajorIndustryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMajorIndustry { get; set; }
        private bool CanEditMajorIndustry { get; set; }
        private bool CanDeleteMajorIndustry { get; set; }
        private MajorIndustryCreateDto NewMajorIndustry { get; set; }
        private Validations NewMajorIndustryValidations { get; set; } = new();
        private MajorIndustryUpdateDto EditingMajorIndustry { get; set; }
        private Validations EditingMajorIndustryValidations { get; set; } = new();
        private Guid EditingMajorIndustryId { get; set; }
        private Modal CreateMajorIndustryModal { get; set; } = new();
        private Modal EditMajorIndustryModal { get; set; } = new();
        private GetMajorIndustriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<MajorIndustryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "majorIndustry-create-tab";
        protected string SelectedEditTab = "majorIndustry-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> ZonesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public MajorIndustries()
        {
            NewMajorIndustry = new MajorIndustryCreateDto();
            EditingMajorIndustry = new MajorIndustryUpdateDto();
            Filter = new GetMajorIndustriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MajorIndustryList = new List<MajorIndustryWithNavigationPropertiesDto>();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:MajorIndustries"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMajorIndustry"], async () =>
            {
                NavigationManager.NavigateTo($"/admin/major-industries/create/");
            }, IconName.Add, requiredPolicyName: DIPPermissions.MajorIndustries.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMajorIndustry = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.MajorIndustries.Create);
            CanEditMajorIndustry = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.MajorIndustries.Edit);
            CanDeleteMajorIndustry = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.MajorIndustries.Delete);
        }

        private async Task GetMajorIndustriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MajorIndustriesAppService.GetListAsync(Filter);
            MajorIndustryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMajorIndustriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await MajorIndustriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/major-industries/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MajorIndustryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMajorIndustriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMajorIndustryModalAsync()
        {
            NewMajorIndustry = new MajorIndustryCreateDto{
                
                ZoneId = ZonesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewMajorIndustryValidations.ClearAll();
            await CreateMajorIndustryModal.Show();
        }

        private async Task CloseCreateMajorIndustryModalAsync()
        {
            NewMajorIndustry = new MajorIndustryCreateDto{
                
                ZoneId = ZonesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateMajorIndustryModal.Hide();
        }

        private async Task OpenEditMajorIndustryModalAsync(MajorIndustryWithNavigationPropertiesDto input)
        {
            var majorIndustry = await MajorIndustriesAppService.GetWithNavigationPropertiesAsync(input.MajorIndustry.Id);
            
            EditingMajorIndustryId = majorIndustry.MajorIndustry.Id;
            EditingMajorIndustry = ObjectMapper.Map<MajorIndustryDto, MajorIndustryUpdateDto>(majorIndustry.MajorIndustry);
            await EditingMajorIndustryValidations.ClearAll();
            await EditMajorIndustryModal.Show();
        }

        private async Task DeleteMajorIndustryAsync(MajorIndustryWithNavigationPropertiesDto input)
        {
            await MajorIndustriesAppService.DeleteAsync(input.MajorIndustry.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);


            await GetMajorIndustriesAsync();
        }

        private async Task CreateMajorIndustryAsync()
        {
            try
            {
                if (await NewMajorIndustryValidations.ValidateAll() == false)
                {
                    return;
                }

                await MajorIndustriesAppService.CreateAsync(NewMajorIndustry);
                await GetMajorIndustriesAsync();
                await CloseCreateMajorIndustryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMajorIndustryModalAsync()
        {
            await EditMajorIndustryModal.Hide();
        }

        private async Task UpdateMajorIndustryAsync()
        {
            try
            {
                if (await EditingMajorIndustryValidations.ValidateAll() == false)
                {
                    return;
                }

                await MajorIndustriesAppService.UpdateAsync(EditingMajorIndustryId, EditingMajorIndustry);
                await GetMajorIndustriesAsync();
                await EditMajorIndustryModal.Hide();                
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
            ZonesCollection = (await MajorIndustriesAppService.GetZoneLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        protected Task EditAsync(MajorIndustryWithNavigationPropertiesDto input)
        {
            NavigationManager.NavigateTo($"/admin/major-industries/edit/{input.MajorIndustry.Id}");
            return Task.CompletedTask;
        }
    }
}
