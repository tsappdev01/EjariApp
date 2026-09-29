using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;
using Microsoft.JSInterop;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class CompanyNews
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPressReleasesAppService PressReleasesAppService { get; set; }
        [Inject]
        IJSRuntime JS { get; set; }
        private IReadOnlyList<PressReleaseFrontEnd> PressReleaseList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int PressreleasesTotalCount { get; set; } = 0;
        private int PressreleasesTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 9;
        private GetPressReleasesInput Filter { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public CompanyNews()
        {
            Filter = new GetPressReleasesInput
            {
                MaxResultCount = PageSize,
                SkipCount = 0,
                Sorting = string.Empty
            };
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("press-releases");

            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            var res = await PressReleasesAppService.GetViewListAsync(Filter);
            PressReleaseList = res.Items;
            CurrentPage = 0;
            PressreleasesTotalCount = Convert.ToInt32(res.TotalCount);
            PressreleasesTotalPage = PressreleasesTotalCount / PageSize;
            if (PressreleasesTotalCount % PageSize != 0)
                PressreleasesTotalPage += 1;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("go_to_top");
        }

        private async Task UpdatePressreleases(int page)
        {
            if (page >= 0 && page < PressreleasesTotalPage)
            {
                CurrentPage = page;
                Filter.SkipCount = CurrentPage * PageSize;
                Filter.MaxResultCount = PageSize;
                Filter.IsActive = true;
                Filter.Sorting = "Date";

                var res = await PressReleasesAppService.GetViewListAsync(Filter);
                PressReleaseList = res.Items;
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }

    }
}
