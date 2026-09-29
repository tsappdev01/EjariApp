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
using DIP.SliderHomePages;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
namespace DIP.Blazor.Pages.Managements.SliderHomePages  {
    public partial class SliderHomePages
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<SliderHomePageDto> SliderHomePageList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSliderHomePage { get; set; }
        private bool CanEditSliderHomePage { get; set; }
        private bool CanDeleteSliderHomePage { get; set; }
        private SliderHomePageCreateDto NewSliderHomePage { get; set; }
        private Validations NewSliderHomePageValidations { get; set; } = new();
        private SliderHomePageUpdateDto EditingSliderHomePage { get; set; }
        private Validations EditingSliderHomePageValidations { get; set; } = new();
        private Guid EditingSliderHomePageId { get; set; }
        private Modal CreateSliderHomePageModal { get; set; } = new();
        private Modal EditSliderHomePageModal { get; set; } = new();
        private GetSliderHomePagesInput Filter { get; set; }
        private DataGridEntityActionsColumn<SliderHomePageDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "sliderHomePage-create-tab";
        protected string SelectedEditTab = "sliderHomePage-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public SliderHomePages()
        {
            NewSliderHomePage = new SliderHomePageCreateDto();
            EditingSliderHomePage = new SliderHomePageUpdateDto();
            Filter = new GetSliderHomePagesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SliderHomePageList = new List<SliderHomePageDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SliderHomePages"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSliderHomePage"], async () =>
            {
                // await OpenCreateSliderHomePageModalAsync();
                NavigationManager.NavigateTo($"/admin/slider-home-pages/create/");

            }, IconName.Add, requiredPolicyName: DIPPermissions.SliderHomePages.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSliderHomePage = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.SliderHomePages.Create);
            CanEditSliderHomePage = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SliderHomePages.Edit);
            CanDeleteSliderHomePage = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SliderHomePages.Delete);
        }

        private async Task GetSliderHomePagesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SliderHomePagesAppService.GetListAsync(Filter);
            SliderHomePageList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSliderHomePagesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await SliderHomePagesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/slider-home-pages/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SliderHomePageDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSliderHomePagesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSliderHomePageModalAsync()
        {
            NewSliderHomePage = new SliderHomePageCreateDto{
                
                
            };
            await NewSliderHomePageValidations.ClearAll();
            await CreateSliderHomePageModal.Show();
        }

        private async Task CloseCreateSliderHomePageModalAsync()
        {
            NewSliderHomePage = new SliderHomePageCreateDto{
                
                
            };
            await CreateSliderHomePageModal.Hide();
        }
        protected Task EditAsync(SliderHomePageDto input)
        {
            NavigationManager.NavigateTo($"/admin/slider-home-pages/edit/{input.Id}");
            return Task.CompletedTask;
        }
        //private async Task OpenEditSliderHomePageModalAsync(SliderHomePageDto input)
        //{
        //    var sliderHomePage = await SliderHomePagesAppService.GetAsync(input.Id);

        //    EditingSliderHomePageId = sliderHomePage.Id;
        //    EditingSliderHomePage = ObjectMapper.Map<SliderHomePageDto, SliderHomePageUpdateDto>(sliderHomePage);
        //    await EditingSliderHomePageValidations.ClearAll();
        //    await EditSliderHomePageModal.Show();
        //}

        private async Task DeleteSliderHomePageAsync(SliderHomePageDto input)
        {
            await SliderHomePagesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetSliderHomePagesAsync();
        }

        private async Task CreateSliderHomePageAsync()
        {
            try
            {
                if (await NewSliderHomePageValidations.ValidateAll() == false)
                {
                    return;
                }

                await SliderHomePagesAppService.CreateAsync(NewSliderHomePage);
                await GetSliderHomePagesAsync();
                await CloseCreateSliderHomePageModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSliderHomePageModalAsync()
        {
            await EditSliderHomePageModal.Hide();
        }

        private async Task UpdateSliderHomePageAsync()
        {
            try
            {
                if (await EditingSliderHomePageValidations.ValidateAll() == false)
                {
                    return;
                }

                await SliderHomePagesAppService.UpdateAsync(EditingSliderHomePageId, EditingSliderHomePage);
                await GetSliderHomePagesAsync();
                await EditSliderHomePageModal.Hide();                
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
