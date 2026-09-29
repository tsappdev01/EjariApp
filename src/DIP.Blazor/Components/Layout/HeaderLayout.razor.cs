using DIP.Amenities;
using DIP.EServices;
using DIP.PageInfos;
using DIP.Shared;
using DIP.SiteSettings;
using DIP.Zones;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.LeptonX.Shared;

namespace DIP.Blazor.Components.Layout
{
    public partial class HeaderLayout
    {

        [Parameter]
        public string Link { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        protected IAbpUtilsService UtilsService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IOptions<LeptonXThemeOptions> Options { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

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
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }

        protected override async Task OnInitializedAsync()
        {
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
            //if (PageInfoHeaderEService.IsNullOrEmpty())
            //{
            //    List<string> slugs = new List<string>()
            //    {
            //        "e-forms" , "Ejari" , "Online/PaymentServices" , "ready-built-facilities"
            //    };
            //    PageInfoHeaderEService = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            //}
            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

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
            if(PageInfoHeaderDirectory == null)
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


        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if(firstRender)
                {



                    

                    var module = await JS.InvokeAsync<IJSObjectReference>(
                                  "import", "assets/js/dip-extension.js");

                }
                else
                {
                    //await JS.InvokeVoidAsync("click_menu", null);
                }





            }
            catch (Exception ex)
            {

            }
        }

        private async Task Navigat(string url)
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                CultureInfo.CurrentCulture = new CultureInfo("ar");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ar");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ar");

            }
            else
            {
                CultureInfo.CurrentCulture = new CultureInfo("en");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");
            }



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
