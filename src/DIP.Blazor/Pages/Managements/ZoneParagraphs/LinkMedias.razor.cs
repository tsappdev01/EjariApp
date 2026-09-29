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
using DIP.Helper;
using Excubo.Generators.Blazor.ExperimentalDoNotUseYet;
using static DIP.Permissions.DIPPermissions;

namespace DIP.Blazor.Pages.Managements.ZoneParagraphs
{
    public partial class LinkMedias
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

        private string SelectedTab { get; set; }
        public LinkMedias()
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
                    getMediasInput.SkipCount= 0;
                    getMediasInput.MaxResultCount = 1000;
                    getMediasInput.ZoneParagraphId = EditingId;

                    PagedResultDto<MediaWithNavigationPropertiesDto> mediaWithNavigationPropertiesDtos = await MediasAppService.GetListAsync(getMediasInput);
                    if (mediaWithNavigationPropertiesDtos != null && mediaWithNavigationPropertiesDtos.TotalCount > 0)
                    {
                        MediaDtos = mediaWithNavigationPropertiesDtos.Items.Select(m => m.Media).ToList();
                        SelectedTab = "1";
                    }

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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ZoneParagraphs"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
 
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["LinkMedias"]));
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
            data.Order = MediaDtos.Count + 1;
            data.TitleEn = $"Media {MediaDtos.Count+1}" ;

            SelectedTab = (MediaDtos.Count + 1).ToString();
            MediaDtos.Add(data);
        }
        protected void MediaDeleted(MediaDto input)
        {
            MediaDtos.Remove(input);
            if(!MediaDtos.IsNullOrEmpty() && SelectedTab != null && SelectedTab.Trim() != "1")
                SelectedTab =  (Int32.Parse(SelectedTab) - 1).ToString();
            else
                if(SelectedTab != null && SelectedTab.Trim() == "1")
                    SelectedTab = "1";
        }

        private async Task UpdateZoneparagraphMediasAsync()
        {
            try
            {
                if (await ValidateMedia() == false)
                {
                    return;
                }

                await MediasAppService.UpdateZoneparagraphMediasAsync(EditingId, MediaDtos);
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task<bool> ValidateMedia()
        {
            foreach (var (item, index) in MediaDtos.WithIndex())
            {
                if(item.File.IsNullOrEmpty())
                {
                    if(!item.TitleEn.IsNullOrEmpty())
                        await uiMessageService.Error( $"{L["MediaFileValidation"]} ({item.TitleEn})");
                    else
                        await uiMessageService.Error($"{L["MediaFileValidation"]} (Media {index + 1})");
                    return false;
                }
            }
            return true;
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

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedTab = name;
        }
    }
}
