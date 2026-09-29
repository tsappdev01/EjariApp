using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.Zones;
using DIP.PageInfos;
using System.Collections.Generic;
using DIP.DipBranches;
using DIP.SiteSettings;
using DIP.LastEventss;
using System.Linq;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class EventsDetails
    {
        [Parameter]
        public string Lang { get; set; }        
        
        [Parameter]
        public string Slug { get; set; }

        [Inject]
        public ILastEventssAppService LastEventsAppService { get; set; }
        private LastEventsFrontEnd LastEventsFrontEnd { get; set; }

        private List<LastEventsFrontEnd> RelatedEventsFrontEnd { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public EventsDetails()
        {
        }

        protected override async Task OnInitializedAsync()
        {
           LastEventsFrontEnd = await LastEventsAppService.GetBySlugAsync(Slug);
            GetLastEventssInput getLastEventssInput = new GetLastEventssInput()
            {
                SkipCount = 0,
                MaxResultCount = 4,
                IsActive= true,
                StartDateMin = LastEventsFrontEnd.StartDate,
                StartDateMax = LastEventsFrontEnd.StartDate.Value.AddYears(1),
                Sorting = "StartDate"
            };
            RelatedEventsFrontEnd = await LastEventsAppService.GetListFrontEndAsync(getLastEventssInput);
            RelatedEventsFrontEnd = RelatedEventsFrontEnd.Where(t => t.Slug != Slug).ToList();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //// await JS.InvokeVoidAsync("hideProgress");
        }
        protected override async Task OnParametersSetAsync()
        {
            LastEventsFrontEnd = await LastEventsAppService.GetBySlugAsync(Slug);
            GetLastEventssInput getLastEventssInput = new GetLastEventssInput()
            {
                SkipCount = 0,
                MaxResultCount =4,
                IsActive = true,
                StartDateMin = LastEventsFrontEnd.StartDate,
                StartDateMax = LastEventsFrontEnd.StartDate.Value.AddYears(1),
                Sorting = "StartDate"
            };
            RelatedEventsFrontEnd = await LastEventsAppService.GetListFrontEndAsync(getLastEventssInput);
            RelatedEventsFrontEnd = RelatedEventsFrontEnd.Where(t => t.Slug != Slug).ToList();

        }
    }
}
