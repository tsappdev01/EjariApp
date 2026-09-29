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
using DIP.DipFacts;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace DIP.Blazor.Pages.Managements.DipFacts
{
    public partial class DipFacts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<DipFactDto> DipFactList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDipFact { get; set; }
        private bool CanEditDipFact { get; set; }
        private bool CanDeleteDipFact { get; set; }
        private DipFactCreateDto NewDipFact { get; set; }
        private Validations NewDipFactValidations { get; set; } = new();
        private DipFactUpdateDto EditingDipFact { get; set; }
        private Validations EditingDipFactValidations { get; set; } = new();
        private Guid EditingDipFactId { get; set; }
        private Modal CreateDipFactModal { get; set; } = new();
        private Modal EditDipFactModal { get; set; } = new();
        private GetDipFactsInput Filter { get; set; }
        private DataGridEntityActionsColumn<DipFactDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "dipFact-create-tab";
        protected string SelectedEditTab = "dipFact-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public DipFacts()
        {
            NewDipFact = new DipFactCreateDto();
            EditingDipFact = new DipFactUpdateDto();
            Filter = new GetDipFactsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DipFactList = new List<DipFactDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:DipFacts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            //Toolbar.AddButton(L["NewDipFact"], async () =>
            //{
            //    NavigationManager.NavigateTo($"/admin/dip-facts/create/");
            //}, IconName.Add, requiredPolicyName: DIPPermissions.DipFacts.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateDipFact = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.DipFacts.Create);
            CanEditDipFact = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipFacts.Edit);
            CanDeleteDipFact = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipFacts.Delete);
        }

        private async Task GetDipFactsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DipFactsAppService.GetListAsync(Filter);
            DipFactList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDipFactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await DipFactsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dip-facts/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DipFactDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDipFactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDipFactModalAsync()
        {
            NewDipFact = new DipFactCreateDto{
                
                
            };
            await NewDipFactValidations.ClearAll();
            await CreateDipFactModal.Show();
        }

        private async Task CloseCreateDipFactModalAsync()
        {
            NewDipFact = new DipFactCreateDto{
                
                
            };
            await CreateDipFactModal.Hide();
        }

        private async Task OpenEditDipFactModalAsync(DipFactDto input)
        {
            var dipFact = await DipFactsAppService.GetAsync(input.Id);
            
            EditingDipFactId = dipFact.Id;
            EditingDipFact = ObjectMapper.Map<DipFactDto, DipFactUpdateDto>(dipFact);
            await EditingDipFactValidations.ClearAll();
            await EditDipFactModal.Show();
        }

     

     

        private async Task CloseEditDipFactModalAsync()
        {
            await EditDipFactModal.Hide();
        }

        private async Task UpdateDipFactAsync()
        {
            try
            {
                if (await EditingDipFactValidations.ValidateAll() == false)
                {
                    return;
                }

                await DipFactsAppService.UpdateAsync(EditingDipFactId, EditingDipFact);
                await GetDipFactsAsync();
                await EditDipFactModal.Hide();                
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

        protected Task EditAsync(DipFactDto input)
        {
            NavigationManager.NavigateTo($"/admin/dip-facts/edit/{input.Id}");
            return Task.CompletedTask;
        }
    }
}
