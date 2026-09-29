using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using DIP.DipFacts;

namespace DIP.Blazor.Pages.Site.AboutUs
{
    public partial class DipProfile
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }


        [Inject]
        public IJSRuntime JS { get; set; }

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
        [Inject]
        IDistributedCache Cache { get; set; }

        [Inject]
        ILogger<DipProfile> _logger { get; set; }



        [Inject]
        public IDipFactsAppService DipFactsAppService { get; set; }

        public List<DipFactFrontEnd> DipFactList { get; set; }


        public DipProfile()
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

    


            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("our-profile");

            //Dip Facts
            GetDipFactsInput getDipFactsInput = new GetDipFactsInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            DipFactList = await DipFactsAppService.GetListFrontEndAsync(getDipFactsInput);
        }


        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {



                await JS.InvokeVoidAsync("dip_light_box", null);

            }
            catch (Exception ex)
            {

            }

        }

    }
}
