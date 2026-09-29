using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.SliderHomePages;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace DIP.Blazor.Pages
{
    public partial class SliderHomePages
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
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
        private SliderHomePageDto? SelectedSliderHomePage;
        
        
        
        
        
        
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
            await SetPermissionsAsync();
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SliderHomePages"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSliderHomePage"], async () =>
            {
                await OpenCreateSliderHomePageModalAsync();
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

        private async Task DownloadAsExcelAsync()
        {
            var token = (await SliderHomePagesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/slider-home-pages/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&TitleAr={HttpUtility.UrlEncode(Filter.TitleAr)}&TitleEn={HttpUtility.UrlEncode(Filter.TitleEn)}&DescriptionAr={HttpUtility.UrlEncode(Filter.DescriptionAr)}&DescriptionEn={HttpUtility.UrlEncode(Filter.DescriptionEn)}&ButtonTitleAr={HttpUtility.UrlEncode(Filter.ButtonTitleAr)}&ButtonTitleEn={HttpUtility.UrlEncode(Filter.ButtonTitleEn)}&ButtonUrlEn={HttpUtility.UrlEncode(Filter.ButtonUrlEn)}&ButtonUrlAr={HttpUtility.UrlEncode(Filter.ButtonUrlAr)}&Image={HttpUtility.UrlEncode(Filter.Image)}&YoutubeUrl={HttpUtility.UrlEncode(Filter.YoutubeUrl)}&IsActive={Filter.IsActive}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}", forceLoad: true);
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

            SelectedCreateTab = "sliderHomePage-create-tab";
            
            
            await NewSliderHomePageValidations.ClearAll();
            await CreateSliderHomePageModal.Show();
        }

        private async Task CloseCreateSliderHomePageModalAsync()
        {
            NewSliderHomePage = new SliderHomePageCreateDto{
                
                
            };
            await CreateSliderHomePageModal.Hide();
        }

        private async Task OpenEditSliderHomePageModalAsync(SliderHomePageDto input)
        {
            SelectedEditTab = "sliderHomePage-edit-tab";
            
            
            var sliderHomePage = await SliderHomePagesAppService.GetAsync(input.Id);
            
            EditingSliderHomePageId = sliderHomePage.Id;
            EditingSliderHomePage = ObjectMapper.Map<SliderHomePageDto, SliderHomePageUpdateDto>(sliderHomePage);
            
            await EditingSliderHomePageValidations.ClearAll();
            await EditSliderHomePageModal.Show();
        }

        private async Task DeleteSliderHomePageAsync(SliderHomePageDto input)
        {
            await SliderHomePagesAppService.DeleteAsync(input.Id);
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









        protected virtual async Task OnTitleArChangedAsync(string? titleAr)
        {
            Filter.TitleAr = titleAr;
            await SearchAsync();
        }
        protected virtual async Task OnTitleEnChangedAsync(string? titleEn)
        {
            Filter.TitleEn = titleEn;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionArChangedAsync(string? descriptionAr)
        {
            Filter.DescriptionAr = descriptionAr;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionEnChangedAsync(string? descriptionEn)
        {
            Filter.DescriptionEn = descriptionEn;
            await SearchAsync();
        }
        protected virtual async Task OnButtonTitleArChangedAsync(string? buttonTitleAr)
        {
            Filter.ButtonTitleAr = buttonTitleAr;
            await SearchAsync();
        }
        protected virtual async Task OnButtonTitleEnChangedAsync(string? buttonTitleEn)
        {
            Filter.ButtonTitleEn = buttonTitleEn;
            await SearchAsync();
        }
        protected virtual async Task OnButtonUrlEnChangedAsync(string? buttonUrlEn)
        {
            Filter.ButtonUrlEn = buttonUrlEn;
            await SearchAsync();
        }
        protected virtual async Task OnButtonUrlArChangedAsync(string? buttonUrlAr)
        {
            Filter.ButtonUrlAr = buttonUrlAr;
            await SearchAsync();
        }
        protected virtual async Task OnImageChangedAsync(string? image)
        {
            Filter.Image = image;
            await SearchAsync();
        }
        protected virtual async Task OnYoutubeUrlChangedAsync(string? youtubeUrl)
        {
            Filter.YoutubeUrl = youtubeUrl;
            await SearchAsync();
        }
        protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
        {
            Filter.IsActive = isActive;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMinChangedAsync(int? orderMin)
        {
            Filter.OrderMin = orderMin;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMaxChangedAsync(int? orderMax)
        {
            Filter.OrderMax = orderMax;
            await SearchAsync();
        }
        







    }
}
