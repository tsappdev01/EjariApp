using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;
using DIP.Amenities;
using DIP.SiteSettings;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.LeptonX.Shared;
using DIP.EServices;

namespace DIP.Blazor.Pages.Site.SiteMap
{
    public partial class Index
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }
        [Inject]
        protected IAbpUtilsService UtilsService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IOptions<LeptonXThemeOptions> Options { get; set; }

     

        [Inject]
        public IZonesAppService ZonesAppService { get; set; }
        [Inject]
        public IAmenitiesAppService AmenitiesAppService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISiteSettingsAppService SiteSettingsAppService { get; set; }
        public SiteSettingFrontEnd SiteSettingFrontEnd { get; set; }
        public List<PageInfoHeaderFrontEnd> PageInfoHeaderAboutUs { get; set; }
        public List<PageInfoHeaderFrontEnd> PageInfoHeaderEService { get; set; }

        public List<ZoneFrontEnd> ZonesList { get; set; }
        public List<AmenityFrontEnd> AmenitiesList { get; set; }

        public List<PageInfoHeaderFrontEnd> PageInfoHeaderMediaCenter { get; set; }
        public PageInfoHeaderFrontEnd PageInfoHeaderDirectory { get; set; }
        public PageInfoHeaderFrontEnd PageInfoHeaderContactUs { get; set; }

        public List<EServiceFrontEnd> EServiceList { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        protected override async Task OnInitializedAsync()
        {

            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("sitemap");

            if (SiteSettingFrontEnd == null)
                SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();

            if (PageInfoHeaderAboutUs.IsNullOrEmpty())
            {
                List<string> slugs = new List<string>()
                {
                    "our-profile", "dubai-investments","our-values","timeline"
                };
                PageInfoHeaderAboutUs = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            }
            if (PageInfoHeaderEService.IsNullOrEmpty())
            {
                List<string> slugs = new List<string>()
                {
                    "e-forms" , "noc-ejari" , "payment-services" , "ready-built-facilities"
                };
                PageInfoHeaderEService = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            }
            if (ZonesList.IsNullOrEmpty())
            {
                GetZonesInput getZonesInput = new GetZonesInput
                {
                    MaxResultCount = 1000,
                    SkipCount = 0,
                    IsActive = true,
                    Sorting = "Order"
                };
                ZonesList = await ZonesAppService.GetListFrontEndAsync(getZonesInput);
            }
            if (AmenitiesList.IsNullOrEmpty())
            {
                GetAmenitiesInput getAmenitiesInput = new GetAmenitiesInput
                {
                    MaxResultCount = 1000,
                    SkipCount = 0,
                    IsActive = true,
                    Sorting = "Order"
                };
                AmenitiesList = await AmenitiesAppService.GetListFrontEndAsync(getAmenitiesInput);
            }
            if (PageInfoHeaderMediaCenter.IsNullOrEmpty())
            {
                List<string> slugs = new List<string>()
                {
                    "press-releases","upcoming-events","gallery"
                };
                PageInfoHeaderMediaCenter = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            }
            if (PageInfoHeaderDirectory == null)
            {
                List<string> slugs = new List<string>()
                {
                    "directory"
                };
                List<PageInfoHeaderFrontEnd> pageInfoHeaderFrontEnds = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
                if (!pageInfoHeaderFrontEnds.IsNullOrEmpty())
                    PageInfoHeaderDirectory = pageInfoHeaderFrontEnds[0];
            }
            if (PageInfoHeaderContactUs == null)
            {
                List<string> slugs = new List<string>()
                {
                    "contact-us"
                };
                List<PageInfoHeaderFrontEnd> pageInfoHeaderFrontEnds = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
                if (!pageInfoHeaderFrontEnds.IsNullOrEmpty())
                    PageInfoHeaderContactUs = pageInfoHeaderFrontEnds[0];
            }
            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

        }

        private async Task Navigat(string url)
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                CultureInfo.CurrentCulture = new CultureInfo("ar");

            }
            else

                CultureInfo.CurrentCulture = new CultureInfo("en");


            // Session.SetString("url", url);

            //NavigationManager.NavigateTo($"api/app/cultures/", true);

            NavigationManager.NavigateTo(url, true);
        }
        private string GetBodyClassName()
        {
            return "lpx-theme-" + Options.Value.DefaultStyle;
        }


        protected override async Task OnParametersSetAsync()
        {
            SetCulture();
        }

        private string SetCulture()
        {

            var link = "";
            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                if (NavigationManager.ToBaseRelativePath(NavigationManager.Uri).IsNullOrEmpty())
                {
                    link = "ar";

                }
                else
                {
                    link = (NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ReplaceFirst("en/", "ar/"));

                }


            }
            else
            {
                if (NavigationManager.ToBaseRelativePath(NavigationManager.Uri).IsNullOrEmpty() || NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Equals("ar"))
                {
                    link = "";

                }
                else
                {
                    link = (NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ReplaceFirst("ar/", "en/"));

                }

            }
            link = "/" + link;


            return link;

        }
    }
}
