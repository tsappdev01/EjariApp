using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;
using DIP.EFormServices;
using DIP.EServices;
using System.Linq;

namespace DIP.Blazor.Pages.Site.EServices
{
    public partial class EForms
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IEFormServicesAppService EFormsAppService { get; set; }

        public List<EFormServiceFrontEnd> EFormServicesList { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public EForms()
        {
        }

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

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("e-forms"));
            }
            GetEFormServicesInput getEFormServicesInput = new GetEFormServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EFormServicesList = await EFormsAppService.GetListFrontEndAsync(getEFormServicesInput);
        }
   
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await JS.InvokeVoidAsync("Splitting", null);
            //await JS.InvokeVoidAsync("dip_light_box", null);

        }

    }
}
