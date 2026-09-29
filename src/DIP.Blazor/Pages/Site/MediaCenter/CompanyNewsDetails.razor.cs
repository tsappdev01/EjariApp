using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using System.Globalization;
using DIP.PressReleases;
using System.Collections.Generic;
using DIP.PageInfos;
using DIP.LastEventss;
using System.Linq;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class CompanyNewsDetails
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Slug { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }

        private PressReleaseFrontEnd PressReleaseFrontEnd { get; set; }
        private List<PressReleaseFrontEnd> RelatedPressReleasesFrontEnd { get; set; }

        [Inject]
        public IPressReleasesAppService PressReleasesAppService { get; set; }

        private IReadOnlyList<PressReleaseFrontEnd> PressReleaseList { get; set; }
        private int CurrentPage { get; set; } = 0;
        private int PressreleasesTotalCount { get; set; } = 0;
        private int PressreleasesTotalPage { get; set; } = 0;
        private int PageSize { get; set; } = 9;
        private GetPressReleasesInput Filter { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        public CompanyNewsDetails()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            PressReleaseFrontEnd = await PressReleasesAppService.GetBySlugAsync(Slug);
            GetPressReleasesInput getPressReleasesInput = new GetPressReleasesInput()
            {
                SkipCount = 0,
                MaxResultCount = 4,
                IsActive = true,
                DateMin = PressReleaseFrontEnd.Date,
                DateMax = PressReleaseFrontEnd.Date.Value.AddYears(1),
                Sorting = "Date"
            };
            RelatedPressReleasesFrontEnd = await PressReleasesAppService.GetListFrontEndAsync(getPressReleasesInput);
            RelatedPressReleasesFrontEnd = RelatedPressReleasesFrontEnd.Where(t => t.Slug != Slug).ToList();

        }

        protected override async Task OnParametersSetAsync()
        {
            PressReleaseFrontEnd = await PressReleasesAppService.GetBySlugAsync(Slug);
            GetPressReleasesInput getPressReleasesInput = new GetPressReleasesInput()
            {
                SkipCount = 0,
                MaxResultCount = 4,
                IsActive = true,
                DateMin = PressReleaseFrontEnd.Date,
                DateMax = PressReleaseFrontEnd.Date.Value.AddYears(1),
                Sorting = "Date"
            };
            RelatedPressReleasesFrontEnd = await PressReleasesAppService.GetListFrontEndAsync(getPressReleasesInput);
            RelatedPressReleasesFrontEnd = RelatedPressReleasesFrontEnd.Where(t=> t.Slug != Slug).ToList();
        }
    }
}
