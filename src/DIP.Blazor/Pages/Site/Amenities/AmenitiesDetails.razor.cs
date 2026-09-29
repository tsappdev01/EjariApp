using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Amenities;
using DIP.PressReleases;

namespace DIP.Blazor.Pages.Site.Amenities
{
    public partial class AmenitiesDetails
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }

        [Inject]
        public IAmenitiesAppService AmenitiesAppService { get; set; }

        private AmenityFrontEnd AmenityFrontEnd { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        public AmenitiesDetails()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            AmenityFrontEnd = await AmenitiesAppService.GetWithDetailsFrontEndAsync(Slug);
            var t = AmenityFrontEnd;
        }
   
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           //// await JS.InvokeVoidAsync("hideProgress");
        }
        protected override async Task OnParametersSetAsync()
        {
            AmenityFrontEnd = await AmenitiesAppService.GetWithDetailsFrontEndAsync(Slug);
            var t = AmenityFrontEnd;
        }
    }
}
