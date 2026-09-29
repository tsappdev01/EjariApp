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
using DIP.PressReleases;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class PressReleases
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<PressReleaseDto> PressReleaseList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePressRelease { get; set; }
        private bool CanEditPressRelease { get; set; }
        private bool CanDeletePressRelease { get; set; }
        private PressReleaseCreateDto NewPressRelease { get; set; }
        private Validations NewPressReleaseValidations { get; set; } = new();
        private PressReleaseUpdateDto EditingPressRelease { get; set; }
        private Validations EditingPressReleaseValidations { get; set; } = new();
        private Guid EditingPressReleaseId { get; set; }
        private Modal CreatePressReleaseModal { get; set; } = new();
        private Modal EditPressReleaseModal { get; set; } = new();
        private GetPressReleasesInput Filter { get; set; }
        private DataGridEntityActionsColumn<PressReleaseDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "pressRelease-create-tab";
        protected string SelectedEditTab = "pressRelease-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public PressReleases()
        {
            NewPressRelease = new PressReleaseCreateDto();
            EditingPressRelease = new PressReleaseUpdateDto();
            Filter = new GetPressReleasesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PressReleaseList = new List<PressReleaseDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PressReleases"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPressRelease"], async () =>
            {
                await OpenCreatePressReleaseModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.PressReleases.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePressRelease = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.PressReleases.Create);
            CanEditPressRelease = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PressReleases.Edit);
            CanDeletePressRelease = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PressReleases.Delete);
        }

        private async Task GetPressReleasesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PressReleasesAppService.GetListAsync(Filter);
            PressReleaseList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPressReleasesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await PressReleasesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/press-releases/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PressReleaseDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPressReleasesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePressReleaseModalAsync()
        {
            NewPressRelease = new PressReleaseCreateDto{
                Date = DateTime.Now,

                
            };
            await NewPressReleaseValidations.ClearAll();
            await CreatePressReleaseModal.Show();
        }

        private async Task CloseCreatePressReleaseModalAsync()
        {
            NewPressRelease = new PressReleaseCreateDto{
                Date = DateTime.Now,

                
            };
            await CreatePressReleaseModal.Hide();
        }

        private async Task OpenEditPressReleaseModalAsync(PressReleaseDto input)
        {
            var pressRelease = await PressReleasesAppService.GetAsync(input.Id);
            
            EditingPressReleaseId = pressRelease.Id;
            EditingPressRelease = ObjectMapper.Map<PressReleaseDto, PressReleaseUpdateDto>(pressRelease);
            await EditingPressReleaseValidations.ClearAll();
            await EditPressReleaseModal.Show();
        }

        private async Task DeletePressReleaseAsync(PressReleaseDto input)
        {
            await PressReleasesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetPressReleasesAsync();
        }

        private async Task CreatePressReleaseAsync()
        {
            try
            {
                if (await NewPressReleaseValidations.ValidateAll() == false)
                {
                    return;
                }

                await PressReleasesAppService.CreateAsync(NewPressRelease);
                await GetPressReleasesAsync();
                await CloseCreatePressReleaseModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPressReleaseModalAsync()
        {
            await EditPressReleaseModal.Hide();
        }

        private async Task UpdatePressReleaseAsync()
        {
            try
            {
                if (await EditingPressReleaseValidations.ValidateAll() == false)
                {
                    return;
                }

                await PressReleasesAppService.UpdateAsync(EditingPressReleaseId, EditingPressRelease);
                await GetPressReleasesAsync();
                await EditPressReleaseModal.Hide();                
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
