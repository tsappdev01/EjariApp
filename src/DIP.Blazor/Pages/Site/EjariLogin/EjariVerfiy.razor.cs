using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using DIP.EServices;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Scriban.Syntax;
using NUglify.JavaScript;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotLiquid.Util;
using System.Web;
using Blazorise;
using System.Globalization;
using DIP.PageInfos;

namespace DIP.Blazor.Pages.Site.EjariLogin
{
    public partial class EjariVerfiy
    {
        [Parameter]
        public string Lang { get; set; }


        [Parameter]
        public string Email { get; set; }

        [Parameter]
        public string Phone { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }


        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
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

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }

        }

        private string GetMobileNumber(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }
            if (mobile.Contains('-'))
            {
                var splitmobile = mobile.Split('-');
                if (splitmobile.Length >= 2)
                {
                    return splitmobile[1];
                }
            }
            return mobile.Replace("+971", string.Empty).Replace("971", string.Empty);
        }

        private string GetCountryCode(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }
            if (mobile.Contains('-'))
            {
                var splitmobile = mobile.Split("-");
                if (splitmobile.Length >= 2)
                {
                    return splitmobile[0];
                }
            }
            return "+971";
        }

        public async Task GoToLogIn()
        {
            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari");

        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
    }
}
