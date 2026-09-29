using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Amenities;
using DIP.PageInfos;
using DIP.Zones;
using System.Collections.Generic;

namespace DIP.Blazor.Pages.Site.Amenities
{
    public partial class Index
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IAmenitiesAppService AmenitiesAppService { get; set; }

        public List<AmenityFrontEnd> AmenitiesList { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public Index()
        {
        }

        protected override async Task OnInitializedAsync()
        {

            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("dip-amenities");
            GetAmenitiesInput getAmenitiesInput = new GetAmenitiesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            AmenitiesList = await AmenitiesAppService.GetListFrontEndAsync(getAmenitiesInput);
        }
   
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           //// await JS.InvokeVoidAsync("hideProgress");
        }

    }
}
