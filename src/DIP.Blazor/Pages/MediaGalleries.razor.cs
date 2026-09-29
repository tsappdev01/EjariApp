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
using DIP.MediaGalleries;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class MediaGalleries
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<MediaGalleryDto> MediaGalleryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMediaGallery { get; set; }
        private bool CanEditMediaGallery { get; set; }
        private bool CanDeleteMediaGallery { get; set; }
        private MediaGalleryCreateDto NewMediaGallery { get; set; }
        private Validations NewMediaGalleryValidations { get; set; } = new();
        private MediaGalleryUpdateDto EditingMediaGallery { get; set; }
        private Validations EditingMediaGalleryValidations { get; set; } = new();
        private Guid EditingMediaGalleryId { get; set; }
        private Modal CreateMediaGalleryModal { get; set; } = new();
        private Modal EditMediaGalleryModal { get; set; } = new();
        private GetMediaGalleriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<MediaGalleryDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "mediaGallery-create-tab";
        protected string SelectedEditTab = "mediaGallery-edit-tab";
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public MediaGalleries()
        {
            NewMediaGallery = new MediaGalleryCreateDto();
            EditingMediaGallery = new MediaGalleryUpdateDto();
            Filter = new GetMediaGalleriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MediaGalleryList = new List<MediaGalleryDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:MediaGalleries"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMediaGallery"], async () =>
            {
                await OpenCreateMediaGalleryModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.MediaGalleries.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMediaGallery = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.MediaGalleries.Create);
            CanEditMediaGallery = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.MediaGalleries.Edit);
            CanDeleteMediaGallery = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.MediaGalleries.Delete);
        }

        private async Task GetMediaGalleriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MediaGalleriesAppService.GetListAsync(Filter);
            MediaGalleryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMediaGalleriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await MediaGalleriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/media-galleries/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MediaGalleryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMediaGalleriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMediaGalleryModalAsync()
        {
            NewMediaGallery = new MediaGalleryCreateDto{
                
                
            };
            await NewMediaGalleryValidations.ClearAll();
            await CreateMediaGalleryModal.Show();
        }

        private async Task CloseCreateMediaGalleryModalAsync()
        {
            NewMediaGallery = new MediaGalleryCreateDto{
                
                
            };
            await CreateMediaGalleryModal.Hide();
        }

        private async Task OpenEditMediaGalleryModalAsync(MediaGalleryDto input)
        {
            var mediaGallery = await MediaGalleriesAppService.GetAsync(input.Id);
            
            EditingMediaGalleryId = mediaGallery.Id;
            EditingMediaGallery = ObjectMapper.Map<MediaGalleryDto, MediaGalleryUpdateDto>(mediaGallery);
            await EditingMediaGalleryValidations.ClearAll();
            await EditMediaGalleryModal.Show();
        }

        private async Task DeleteMediaGalleryAsync(MediaGalleryDto input)
        {
            await MediaGalleriesAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetMediaGalleriesAsync();
        }

        private async Task CreateMediaGalleryAsync()
        {
            try
            {
                if (await NewMediaGalleryValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediaGalleriesAppService.CreateAsync(NewMediaGallery);
                await GetMediaGalleriesAsync();
                await CloseCreateMediaGalleryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMediaGalleryModalAsync()
        {
            await EditMediaGalleryModal.Hide();
        }

        private async Task UpdateMediaGalleryAsync()
        {
            try
            {
                if (await EditingMediaGalleryValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediaGalleriesAppService.UpdateAsync(EditingMediaGalleryId, EditingMediaGallery);
                await GetMediaGalleriesAsync();
                await EditMediaGalleryModal.Hide();                
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
