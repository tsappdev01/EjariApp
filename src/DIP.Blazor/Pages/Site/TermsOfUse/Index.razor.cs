using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;

namespace DIP.Blazor.Pages.Site.TermsOfUse
{
    public partial class Index
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPressReleasesAppService PressReleasesAppService { get; set; }

        private IReadOnlyList<PressReleaseFrontEnd> PressReleaseList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int PressreleasesTotalCount { get; set; } = 0;
        private int PressreleasesTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 9;
        private GetPressReleasesInput Filter { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public Index()
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
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("terms-of-use");

  
        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

    }
}
