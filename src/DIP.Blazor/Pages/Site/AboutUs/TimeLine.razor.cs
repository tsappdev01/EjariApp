using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;
using DIP.TimeLineCategories;

namespace DIP.Blazor.Pages.Site.AboutUs
{
    public partial class TimeLine
    {
        [Parameter]
        public string Lang { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        public PageInfoFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public ITimeLineCategoriesAppService TimeLineCategoriesAppService { get; set; }

        private List<TimeLineCategoryFrontEnd> TimeLineCategoryList { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public TimeLine()
        {
           
        }

        protected override async Task OnInitializedAsync()
        {
            PageInfoFrontEnd = await PageInfosAppService.GetBySlugAsync("timeline");

            TimeLineCategoryList = await TimeLineCategoriesAppService.GetListWithDetailsFrontEndAsync();
        }
    }
}
