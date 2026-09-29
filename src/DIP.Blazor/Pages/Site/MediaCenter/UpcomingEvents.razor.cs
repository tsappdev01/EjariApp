using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.LastEventss;
using System.Collections.Generic;
using DIP.PageInfos;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class UpcomingEvents
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public ILastEventssAppService LastEventsAppService { get; set; }

        private IReadOnlyList<LastEventsFrontEnd> LastEventsList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int LastEventsTotalCount { get; set; } = 0;
        private int LastEventsTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 9;
        private GetLastEventssInput Filter { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public UpcomingEvents()
        {
            Filter = new GetLastEventssInput
            {
                MaxResultCount = PageSize,
                SkipCount = 0,
                Sorting = string.Empty
            };
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("upcoming-events");

            Filter.SkipCount = 0;
            Filter.MaxResultCount = PageSize;
            Filter.IsActive = true;
            Filter.StartDateMin = DateTime.Now;
            var res = await LastEventsAppService.GetViewListAsync(Filter);
            LastEventsList = res.Items;
            CurrentPage = 0;
            LastEventsTotalCount = Convert.ToInt32(res.TotalCount);
            LastEventsTotalPage = LastEventsTotalCount / PageSize;
            if (LastEventsTotalCount % PageSize != 0)
                LastEventsTotalPage += 1;
        }
        private async Task UpdateLastEvents(int page)
        {
            if (page >= 0 && page < LastEventsTotalPage)
            {
                CurrentPage = page;
                Filter.SkipCount = CurrentPage * PageSize;
                Filter.MaxResultCount = PageSize;
                Filter.IsActive = true;
                Filter.StartDateMin = DateTime.Now;
                Filter.Sorting = "StartDate";
                var res = await LastEventsAppService.GetViewListAsync(Filter);
                LastEventsList = res.Items;
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("go_to_top");
        }

    }
}
