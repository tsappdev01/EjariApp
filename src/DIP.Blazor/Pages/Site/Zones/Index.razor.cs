using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;

namespace DIP.Blazor.Pages.Site.Zones
{
    public partial class Index
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IZonesAppService ZonesAppService { get; set; }

        public List<ZoneFrontEnd> ZonesList { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("dip-zones");
            GetZonesInput getZonesInput = new GetZonesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            ZonesList = await ZonesAppService.GetListFrontEndAsync(getZonesInput);
        }
   
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("Splitting", null);
            await JS.InvokeVoidAsync("dip_light_box", null);

        }

    }
}
