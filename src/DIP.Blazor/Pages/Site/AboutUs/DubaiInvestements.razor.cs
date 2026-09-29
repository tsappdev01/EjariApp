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
    public partial class DubaiInvestements
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPressReleasesAppService PressReleasesAppService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public DubaiInvestements()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            using (CultureHelper.Use(Lang))
            {
                PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("dubai-investments");
            }
        }

    }
}
