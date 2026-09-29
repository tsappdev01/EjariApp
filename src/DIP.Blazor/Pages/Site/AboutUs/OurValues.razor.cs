using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;

namespace DIP.Blazor.Pages.Site.AboutUs
{
    public partial class OurValues
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


        public PageInfoFrontEnd PageInfoFrontEndCorporate { get; set; }

        public PageInfoFrontEnd PageInfoFrontEndEnvironment { get; set; }

        public PageInfoFrontEnd PageInfoFrontEndCommunity { get; set; }

        public PageInfoFrontEnd PageInfoFrontEndIntegration { get; set; }

        public PageInfoFrontEnd PageInfoFrontEndMission { get; set; }
        public PageInfoFrontEnd PageInfoFrontEndVision { get; set; }


        public OurValues()
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
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("our-values");
            PageInfoFrontEndCorporate = await PageInfosAppService.GetBySlugAsync("corporate-social-responsibility-csr");
            PageInfoFrontEndEnvironment = await PageInfosAppService.GetBySlugAsync("environment-friendly");
            PageInfoFrontEndCommunity = await PageInfosAppService.GetBySlugAsync("community-driven");
            PageInfoFrontEndIntegration = await PageInfosAppService.GetBySlugAsync("integration");
            PageInfoFrontEndVision = await PageInfosAppService.GetBySlugAsync("our-vision");
            PageInfoFrontEndMission = await PageInfosAppService.GetBySlugAsync("our-mission");

 
          
        }
     

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

    }
}
