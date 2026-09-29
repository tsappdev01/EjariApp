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
using DIP.Medias;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class Medias
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<MediaWithNavigationPropertiesDto> MediaList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMedia { get; set; }
        private bool CanEditMedia { get; set; }
        private bool CanDeleteMedia { get; set; }
        private MediaCreateDto NewMedia { get; set; }
        private Validations NewMediaValidations { get; set; } = new();
        private MediaUpdateDto EditingMedia { get; set; }
        private Validations EditingMediaValidations { get; set; } = new();
        private Guid EditingMediaId { get; set; }
        private Modal CreateMediaModal { get; set; } = new();
        private Modal EditMediaModal { get; set; } = new();
        private GetMediasInput Filter { get; set; }
        private DataGridEntityActionsColumn<MediaWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "media-create-tab";
        protected string SelectedEditTab = "media-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> ZoneParagraphsCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> AmenityParagraphsCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> MediaGalleriesCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public Medias()
        {
            NewMedia = new MediaCreateDto();
            EditingMedia = new MediaUpdateDto();
            Filter = new GetMediasInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MediaList = new List<MediaWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetZoneParagraphCollectionLookupAsync();


            await GetMediaGalleryCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Medias"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMedia"], async () =>
            {
                await OpenCreateMediaModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.Medias.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMedia = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Medias.Create);
            CanEditMedia = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Medias.Edit);
            CanDeleteMedia = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Medias.Delete);
        }

        private async Task GetMediasAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MediasAppService.GetListAsync(Filter);
            MediaList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await MediasAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/medias/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MediaWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMediaModalAsync()
        {
            NewMedia = new MediaCreateDto{
                
                
            };
            await NewMediaValidations.ClearAll();
            await CreateMediaModal.Show();
        }

        private async Task CloseCreateMediaModalAsync()
        {
            NewMedia = new MediaCreateDto{
                
                
            };
            await CreateMediaModal.Hide();
        }

        private async Task OpenEditMediaModalAsync(MediaWithNavigationPropertiesDto input)
        {
            var media = await MediasAppService.GetWithNavigationPropertiesAsync(input.Media.Id);
            
            EditingMediaId = media.Media.Id;
            EditingMedia = ObjectMapper.Map<MediaDto, MediaUpdateDto>(media.Media);
            await EditingMediaValidations.ClearAll();
            await EditMediaModal.Show();
        }

        private async Task DeleteMediaAsync(MediaWithNavigationPropertiesDto input)
        {
            await MediasAppService.DeleteAsync(input.Media.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetMediasAsync();
        }

        private async Task CreateMediaAsync()
        {
            try
            {
                if (await NewMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediasAppService.CreateAsync(NewMedia);
                await GetMediasAsync();
                await CloseCreateMediaModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMediaModalAsync()
        {
            await EditMediaModal.Hide();
        }

        private async Task UpdateMediaAsync()
        {
            try
            {
                if (await EditingMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediasAppService.UpdateAsync(EditingMediaId, EditingMedia);
                await GetMediasAsync();
                await EditMediaModal.Hide();                
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
        

        private async Task GetZoneParagraphCollectionLookupAsync(string? newValue = null)
        {
            ZoneParagraphsCollection = (await MediasAppService.GetZoneParagraphLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetAmenityParagraphCollectionLookupAsync(string? newValue = null)
        {
            AmenityParagraphsCollection = (await MediasAppService.GetAmenityParagraphLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetMediaGalleryCollectionLookupAsync(string? newValue = null)
        {
            MediaGalleriesCollection = (await MediasAppService.GetMediaGalleryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
