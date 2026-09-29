using Blazorise;
using DIP.Amenities;
using DIP.EServices;
using DIP.PageInfos;
using DIP.SiteSettings;
using DIP.Zones;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.LeptonX.Shared;

namespace DIP.Blazor.Components.Layout
{
    public partial class CustomLayout
    {
     



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


            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);


            //if (SiteSettingFrontEnd == null)
            //    SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();

            //if (PageInfoHeaderAboutUs.IsNullOrEmpty())
            //{
            //    List<string> slugs = new List<string>()
            //    {
            //        "dip-profile", "dubai-investments","timeline"
            //    };
            //    PageInfoHeaderAboutUs = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            //}
            //if (PageInfoHeaderEService.IsNullOrEmpty())
            //{
            //    List<string> slugs = new List<string>()
            //    {

            //    };
            //    PageInfoHeaderEService = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            //}
            //if (ZonesList.IsNullOrEmpty())
            //{
            //    GetZonesInput getZonesInput = new GetZonesInput
            //    {
            //        MaxResultCount = 1000,
            //        SkipCount = 0,
            //        IsActive = true,
            //        Sorting = "Order"
            //    };
            //    ZonesList = await ZonesAppService.GetListFrontEndAsync(getZonesInput);
            //}
            //if (AmenitiesList.IsNullOrEmpty())
            //{
            //    GetAmenitiesInput getAmenitiesInput = new GetAmenitiesInput
            //    {
            //        MaxResultCount = 1000,
            //        SkipCount = 0,
            //        IsActive = true,
            //        Sorting = "Order"
            //    };
            //    AmenitiesList = await AmenitiesAppService.GetListFrontEndAsync(getAmenitiesInput);
            //}
            //if (PageInfoHeaderMediaCenter.IsNullOrEmpty())
            //{
            //    List<string> slugs = new List<string>()
            //    {
            //        "company-news","upcoming-events"
            //    };
            //    PageInfoHeaderMediaCenter = await PageInfosAppService.GetPageInfoLookupBySlugsAsync(slugs);
            //}

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if(firstRender)
                {
                    await JSRuntime.InvokeVoidAsync("dip_arrows", null);

                }
                else
                {
                    
                    //await JSRuntime.InvokeVoidAsync("dip_arrows", null);

                }

                await JSRuntime.InvokeVoidAsync("dip_chat_box", null);


                //await JSRuntime.InvokeVoidAsync("click_menu", null);

            }
            catch (Exception ex)
            {

            }
        }

        private async Task Navigat(string url)
        {




            NavigationManager.NavigateTo($"/{CultureInfo.CurrentCulture.Name}/{url}", forceLoad: false);
     
        }
        public string SetCulture()
        {
            //var t = HttpUtility.UrlEncode(NavigationManager.ToBaseRelativePath(NavigationManager.Uri));
            //var tt = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            //var ttt = NavigationManager.Uri;
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
