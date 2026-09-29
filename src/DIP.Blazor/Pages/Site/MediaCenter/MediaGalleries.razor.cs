using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.MediaGalleries;
using System.Collections.Generic;
using DIP.PageInfos;
using DIP.MediaGalleries;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class MediaGalleries
    {
        [Inject]
        IJSRuntime JS { get; set; }
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IMediaGalleriesAppService MediaGalleriesAppService { get; set; }

        private IReadOnlyList<MediaGalleryFrontEnd> MediaGalleryList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int MediaGalleriesTotalCount { get; set; } = 0;
        private int MediaGalleriesTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 9;
        private GetMediaGalleriesInput Filter { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public MediaGalleries()
        {
            Filter = new GetMediaGalleriesInput()
            {
                MaxResultCount = PageSize,
                SkipCount = 0,
                Sorting = string.Empty
            };
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("gallery");

            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            var res = await MediaGalleriesAppService.GetListFrontEndAsync(Filter);
            MediaGalleryList = res.Items;
            CurrentPage = 0;
            MediaGalleriesTotalCount = Convert.ToInt32(res.TotalCount);
            MediaGalleriesTotalPage = MediaGalleriesTotalCount / PageSize;
            if (MediaGalleriesTotalCount % PageSize != 0)
                MediaGalleriesTotalPage += 1;
        }
        private async Task UpdateMediaGalleries(int page)
        {
            if (page >= 0 && page < MediaGalleriesTotalPage)
            {
                CurrentPage = page;
                Filter.SkipCount = CurrentPage * PageSize;
                Filter.MaxResultCount = PageSize;
                Filter.IsActive = true;
                Filter.Sorting = "Order";
                var res = await MediaGalleriesAppService.GetListFrontEndAsync(Filter);
                MediaGalleryList = res.Items;
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("go_to_top");
        }

    }
}
