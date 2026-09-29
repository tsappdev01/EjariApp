using DIP.SiteSettings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.LeptonX.Shared;

namespace DIP.Blazor.Components.Layout
{
    public partial class FooterLayout
    {
        [Inject]
        protected IAbpUtilsService UtilsService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IOptions<LeptonXThemeOptions> Options { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISiteSettingsAppService SiteSettingsAppService { get; set; }
        public SiteSettingFrontEnd SiteSettingFrontEnd { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (SiteSettingFrontEnd == null)
            {
                SiteSettingFrontEnd = await SiteSettingsAppService.GetFrontAsync();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
        }
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
        private string GetBodyClassName()
        {
            return "lpx-theme-" + Options.Value.DefaultStyle;
        }
    }
}
