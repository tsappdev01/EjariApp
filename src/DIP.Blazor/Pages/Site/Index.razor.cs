using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System;
using Blazorise;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System.Globalization;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using DIP.PressReleases;
using DIP.DipFacts;
using DIP.EServices;
using k8s.Models;
using DIP.Zones;
using DIP.LastEventss;
using DIP.PageInfos;
using DIP.SliderHomePages;
using DIP.Helper;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DIP.SiteSettings;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Volo.Abp.OpenIddict;

namespace DIP.Blazor.Pages.Site
{
    public partial class Index
    {

        [Parameter]
        public string lang { get; set; }
        public string Lang { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }
        public PageInfoFrontEnd PageInfoHomeFrontEnd { get; set; }

        [Inject]
        public ISliderHomePagesAppService SliderHomePagesAppService { get; set; }
        public List<SliderHomePageFrontEnd> SliderHomePageList { get; set; }

        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        [Inject]
        public IZonesAppService ZonesAppService { get; set; }

        public List<ZoneFrontEnd> ZonesList { get; set; }

        [Inject]
        public IDipFactsAppService DipFactsAppService { get; set; }

        public List<DipFactFrontEnd> DipFactList { get; set; }

        [Inject]
        public IPressReleasesAppService PressReleasesAppService { get; set; }

        public List<PressReleaseFrontEnd> PressReleaseList { get; set; }

        [Inject]
        public ILastEventssAppService LastEventssAppService { get; set; }

        public List<LastEventsFrontEnd> LastEventList { get; set; }
        [Inject]
        public ISiteSettingsAppService SiteSettingsAppService { get; set; }

        public SiteSettingFrontEnd SiteSettingFrontEnd { get; set; }
        [Inject]
        IDistributedCache Cache { get; set; }
        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {
         //   await Cache.SetStringAsync("Morshed", "MorshedCache",
         //  new DistributedCacheEntryOptions
         //  {
         //      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
         //  });
         //   var num = 1234;
         //   await Cache.SetStringAsync("MorshedNumber", num.ToString(),
         //new DistributedCacheEntryOptions
         //{
         //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
         //});


            
         //   await Cache.SetStringAsync("MorshedTest", JsonConvert.SerializeObject("MorshedTest"),
         //  new DistributedCacheEntryOptions
         //  {
         //      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
         //  });


            Lang = CultureInfo.CurrentCulture.Name;

            PageInfoHomeFrontEnd = await PageInfosAppService.GetBySlugAsync("home");
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("our-profile");

            //slider home page
            GetSliderHomePagesInput getSliderHomePagesInput = new GetSliderHomePagesInput()
            {
                SkipCount = 0,
                MaxResultCount = 1000,
                IsActive = true,
                Sorting = "Order"
            };
            SliderHomePageList = await SliderHomePagesAppService.GetListFrontEndAsync(getSliderHomePagesInput);
            
            //eservices
            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);
            //zones
            GetZonesInput getZonesInput = new GetZonesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeature = true,
                IsActive = true,
                Sorting = "Order"
            };
            ZonesList = await ZonesAppService.GetListFrontEndAsync(getZonesInput);
            //Dip Facts
            GetDipFactsInput getDipFactsInput = new GetDipFactsInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            DipFactList = await DipFactsAppService.GetListFrontEndAsync(getDipFactsInput);

            // PressReleases
            GetPressReleasesInput getPressReleasesInput = new GetPressReleasesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeatured = true,
                IsActive = true,
                Sorting = "Order"
            };
            PressReleaseList = await PressReleasesAppService.GetListFrontEndAsync(getPressReleasesInput);
            // LastEvents
            GetLastEventssInput getLastEventssInput = new GetLastEventssInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeatured = true,
                IsActive = true,
                Sorting = "Order"
            };
            LastEventList = await LastEventssAppService.GetListFrontEndAsync(getLastEventssInput);

            if (SiteSettingFrontEnd == null)
            {
                SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();
            }
        }
        private async Task Navigat(string url)
        {
            // Session.SetString("url", url);
            NavigationManager.NavigateTo(url);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {

               
                await JS.InvokeVoidAsync("dip_swiper_with_tab", null);
                await JS.InvokeVoidAsync("dip_modal_popup", null);

                await JS.InvokeVoidAsync("dip_theme_owl_carousel", null);

                await JS.InvokeVoidAsync("dip_preloader", null);
                await JS.InvokeVoidAsync("dip_swiper_options", null);
                await JS.InvokeVoidAsync("Splitting", null);
                await JS.InvokeVoidAsync("dip_light_box", null);


            }
            catch (Exception ex)
            {

            }

        }

        public string GetPlayList(string youTubeLink)
        {
            var playList = youTubeLink.Split("/").LastOrDefault();

            return playList;
        }
        protected override async Task OnParametersSetAsync()
        {
            //PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("home");
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("our-profile");

            //slider home page
            GetSliderHomePagesInput getSliderHomePagesInput = new GetSliderHomePagesInput()
            {
                SkipCount = 0,
                MaxResultCount = 1000,
                IsActive = true,
                Sorting = "Order"
            };
            SliderHomePageList = await SliderHomePagesAppService.GetListFrontEndAsync(getSliderHomePagesInput);
            Logger.LogError(SliderHomePageList.Count+"");
            //eservices
            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);
            //zones
            GetZonesInput getZonesInput = new GetZonesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeature = true,
                IsActive = true,
                Sorting = "Order"
            };
            ZonesList = await ZonesAppService.GetListFrontEndAsync(getZonesInput);
            //Dip Facts
            GetDipFactsInput getDipFactsInput = new GetDipFactsInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            DipFactList = await DipFactsAppService.GetListFrontEndAsync(getDipFactsInput);

            // PressReleases
            GetPressReleasesInput getPressReleasesInput = new GetPressReleasesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeatured = true,
                IsActive = true,
                Sorting = "Order"
            };
            PressReleaseList = await PressReleasesAppService.GetListFrontEndAsync(getPressReleasesInput);
            // LastEvents
            GetLastEventssInput getLastEventssInput = new GetLastEventssInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsFeatured = true,
                IsActive = true,
                Sorting = "Order",
                StartDateMin = DateTime.Now,
                
            };
            LastEventList = await LastEventssAppService.GetListFrontEndAsync(getLastEventssInput);
        }

    }
}
