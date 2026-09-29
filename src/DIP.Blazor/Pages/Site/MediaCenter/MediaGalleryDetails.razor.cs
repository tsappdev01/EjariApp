using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DIP.MediaGalleries;
using DIP.Amenities;

namespace DIP.Blazor.Pages.Site.MediaCenter
{
    public partial class MediaGalleryDetails
    {
        [Parameter]
        public string Lang { get; set; }        
        
        [Parameter]
        public string Slug { get; set; }

        [Inject]
        public IMediaGalleriesAppService MediaGalleriesAppService { get; set; }
        private MediaGalleryFrontEnd MediaGalleryFrontEnd { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public MediaGalleryDetails()
        {
        }

        protected override async Task OnInitializedAsync()
        {
           MediaGalleryFrontEnd = await MediaGalleriesAppService.GetWithDetailsFrontEndAsync(Slug);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //// await JS.InvokeVoidAsync("hideProgress");
        }
        protected override async Task OnParametersSetAsync()
        {
            MediaGalleryFrontEnd = await MediaGalleriesAppService.GetWithDetailsFrontEndAsync(Slug);

        }
    }
}
