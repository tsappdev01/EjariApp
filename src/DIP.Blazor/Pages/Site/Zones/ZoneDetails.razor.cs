using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;

namespace DIP.Blazor.Pages.Site.Zones
{
    public partial class ZoneDetails
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IZonesAppService ZonesAppService { get; set; }

        private ZoneFrontEnd ZoneFrontEnd { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public ZoneDetails()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            ZoneFrontEnd = await ZonesAppService.GetWithDetailsFrontEndAsync(Slug);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("dip_swiper_options", null);

            }



        }
        protected override async Task OnParametersSetAsync()
        {
            ZoneFrontEnd = await ZonesAppService.GetWithDetailsFrontEndAsync(Slug);
        }

    }
}
