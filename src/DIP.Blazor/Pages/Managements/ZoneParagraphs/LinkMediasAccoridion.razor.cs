using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Guids;
using Volo.Abp.Application.Dtos;
using DIP.Medias;
using DIP.ZoneParagraphs;
using Microsoft.Extensions.Logging;
using DIP.Permissions;
using Excubo.Generators.Blazor.ExperimentalDoNotUseYet;

namespace DIP.Blazor.Pages.Managements.ZoneParagraphs
{
    public partial class LinkMediasAccoridion
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private bool CanEditZoneParagraph { get; set; }

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }

        private Guid EditingId { get; set; }
        private Validations EditingZoneParagraphValidations { get; set; } = new();
        
        [Inject]
        public IZoneParagraphsAppService ZoneParagraphsAppService { get; set; }

        [Inject]
        public IMediasAppService MediasAppService { get; set; }
        
        private List<MediaDto> MediaDtos { get; set; }
        private ZoneParagraphDto ZoneParagraphDto { get; set; }
        public LinkMediasAccoridion()
        {
           
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();

            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingId = Guid.Parse(Id);

                    ZoneParagraphDto = await ZoneParagraphsAppService.GetAsync(EditingId);

                    GetMediasInput getMediasInput = new GetMediasInput();
                    getMediasInput.SkipCount= 1;
                    getMediasInput.MaxResultCount = 1000;
                    getMediasInput.ZoneParagraphId = EditingId;

                    PagedResultDto<MediaWithNavigationPropertiesDto> mediaWithNavigationPropertiesDtos = await MediasAppService.GetListAsync(getMediasInput);
                    if (mediaWithNavigationPropertiesDtos != null && mediaWithNavigationPropertiesDtos.TotalCount > 0)
                        MediaDtos = mediaWithNavigationPropertiesDtos.Items.Select(m => m.Media).ToList();

                   // await SetNewAsync();
                }
                catch (Exception ex)
                {
                    //await uiMessageService.Error("Error in get data");
                    Logger.LogError(ex, "Error in get data");
                    //NavigationManager.NavigateTo("/page-informations");
                    NavigationManager.NavigateTo($"/{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}");
                    //{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}
                    await HandleErrorAsync(ex);
                }
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:MediaGalleries"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
 
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }
        private async Task SetPermissionsAsync()
        {
            CanEditZoneParagraph = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.ZoneParagraphs.Edit);

            if (!CanEditZoneParagraph)
                throw new UnauthorizedAccessException();

        }

        protected async Task addNewMediaAsync()
        {
            var data = new MediaDto();
            data.Id = GuidGenerator.Create();
            if (!Id.IsNullOrEmpty())
                data.ZoneParagraphId = Guid.Parse(Id);
            data.IsActive = true;
            if (MediaDtos.IsNullOrEmpty())
                MediaDtos = new List<MediaDto>();
            data.Order = MediaDtos.Count;
            MediaDtos.Add(data);
        }
        protected void MediaDelated(MediaDto input)
        {
            MediaDtos.Remove(input);
        }

        private async Task UpdateMediaGalleryMediasAsync()
        {
            try
            {
                //var academicCourseList = await MediaGalleriesAppService.UpdateMediasAsync(EditingMediaGalleryId, EditingMediaGallery);
                //await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                //NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
            {
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
        }

        //private async Task SetNewAsync()
        //{
        //    GetMediaGalleriesInput getMediaGalleriesInput = new GetMediaGalleriesInput();
        //    getMediaGalleriesInput.MaxResultCount = 1;
        //    getMediaGalleriesInput.IsActive = true;
        //    getMediaGalleriesInput.IsForPagesOnly = true;

        //    PagedResultDto<MediaGalleryDto> pagedResult = await MediaGalleriesAppService.GetListAsync(getMediaGalleriesInput);
        //    if (pagedResult != null)
        //    {
        //        EditingMediaGallery.Order = Convert.ToInt32(pagedResult.TotalCount);
        //    }
        //}


    }
}
