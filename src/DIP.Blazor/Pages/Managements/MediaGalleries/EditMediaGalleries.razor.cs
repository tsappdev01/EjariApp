using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Blazorise.Extensions;
using DIP.MediaGalleries;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using DIP.Helper;
using Volo.Abp.BlobStoring;

namespace DIP.Blazor.Pages.Managements.MediaGalleries
{
    public partial class EditMediaGalleries
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private MediaGalleryCreateDto NewMediaGallery { get; set; }
        private MediaGalleryUpdateDto EditingMediaGallery { get; set; }
        private Validations NewMediaGalleryValidations { get; set; } = new();

        private Validations EditingMediaGalleryValidations { get; set; } = new();
        private Guid EditingMediaGalleryId { get; set; }
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private MediaGalleryUpdateDto EditingMediaGallerys { get; set; }

        [Inject]
        public IBlobContainer<MediaGalleryContainer> MediaGalleryContainer { get; set; }
        public string MediaGalleryImage { get; set; } = "";
        public byte[] MediaGalleryImageContent { get; set; }
        public bool MediaGalleryImageNewUpload { get; set; } = false;

        public string MediaGalleryHeaderImage { get; set; } = "";
        public byte[] MediaGalleryHeaderImageContent { get; set; }
        public bool MediaGalleryHeaderImageNewUpload { get; set; } = false;

        public EditMediaGalleries()
        {
            NewMediaGallery = new MediaGalleryCreateDto();
            EditingMediaGallery = new MediaGalleryUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingMediaGalleryId = Guid.Parse(Id);
                    var MediaGallery = await MediaGalleriesAppService.GetAsync(EditingMediaGalleryId);
                    EditingMediaGallery = ObjectMapper.Map<MediaGalleryDto, MediaGalleryUpdateDto>(MediaGallery);
                    if (EditingMediaGallery != null)
                    {
                       MediaGalleryImage = EditingMediaGallery?.Image;
                        MediaGalleryHeaderImage = EditingMediaGallery?.HeaderImage;
                    }
                    await EditingMediaGalleryValidations.ClearAll();
                }
                catch (Exception ex)
                {

                    //await uiMessageService.Error("Error in get data");
                    Logger.LogError(ex, "Error in get data");
                    //NavigationManager.NavigateTo("/page-informations");
                    NavigationManager.NavigateTo($"/{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
                    //{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}
                    await HandleErrorAsync(ex);
                }
            }
            else
                await SetNewAsync();
   
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:MediaGalleries"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }


        private async Task CreateMediaGalleryAsync()
        {
            try
            {
                if (await NewMediaGalleryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!MediaGalleryImage.IsNullOrEmpty() && !MediaGalleryImageContent.IsNullOrEmpty())
                {
                    await MediaGalleryContainer.SaveAsync(MediaGalleryImage, MediaGalleryImageContent);
                    NewMediaGallery.Image = MediaGalleryImage;
                }
                if (!MediaGalleryHeaderImage.IsNullOrEmpty() && !MediaGalleryHeaderImageContent.IsNullOrEmpty())
                {
                    await MediaGalleryContainer.SaveAsync(MediaGalleryHeaderImage, MediaGalleryHeaderImageContent);
                    NewMediaGallery.HeaderImage = MediaGalleryHeaderImage;
                }
                var ent=   await MediaGalleriesAppService.CreateAsync(NewMediaGallery);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateMediaGalleryAsync()
        {
            try
            {
                if (await EditingMediaGalleryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!MediaGalleryImage.IsNullOrEmpty() && !MediaGalleryImageContent.IsNullOrEmpty() && MediaGalleryImage != EditingMediaGallery.Image)
                {
                    if (!EditingMediaGallery.Image.IsNullOrEmpty())
                        await MediaGalleryContainer.DeleteAsync(EditingMediaGallery.Image);
                    await MediaGalleryContainer.SaveAsync(MediaGalleryImage, MediaGalleryImageContent);
                    EditingMediaGallery.Image = MediaGalleryImage;
                }
                else
                {
                    if (MediaGalleryImage.IsNullOrEmpty() && !EditingMediaGallery.Image.IsNullOrEmpty())
                    {
                        await MediaGalleryContainer.DeleteAsync(EditingMediaGallery.Image);
                        EditingMediaGallery.Image = null;
                    }
                }
                if (!MediaGalleryHeaderImage.IsNullOrEmpty() && !MediaGalleryHeaderImageContent.IsNullOrEmpty() && MediaGalleryHeaderImage != EditingMediaGallery.HeaderImage)
                {
                    if (!EditingMediaGallery.HeaderImage.IsNullOrEmpty())
                        await MediaGalleryContainer.DeleteAsync(EditingMediaGallery.HeaderImage);
                    await MediaGalleryContainer.SaveAsync(MediaGalleryHeaderImage, MediaGalleryHeaderImageContent);
                    EditingMediaGallery.HeaderImage = MediaGalleryHeaderImage;
                }
                else
                {
                    if (MediaGalleryHeaderImage.IsNullOrEmpty() && !EditingMediaGallery.HeaderImage.IsNullOrEmpty())
                    {
                        await MediaGalleryContainer.DeleteAsync(EditingMediaGallery.HeaderImage);
                        EditingMediaGallery.HeaderImage = null;
                    }
                }
                await MediaGalleriesAppService.UpdateAsync(EditingMediaGalleryId, EditingMediaGallery);
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
 
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    MediaGalleryImageContent = await result.GetAllBytesAsync();
                    MediaGalleryImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    MediaGalleryImageNewUpload = true;
                }
            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        public async Task ImageChanged(FileChangedEventArgs e)
        {
            try
            {
                if (e.Files.Count() == 0)
                {
                    MediaGalleryImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        public async Task OnHeaderImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    MediaGalleryHeaderImageContent = await result.GetAllBytesAsync();
                    MediaGalleryHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    MediaGalleryHeaderImageNewUpload = true;
                }
            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        public async Task HeaderImageChanged(FileChangedEventArgs e)
        {
            try
            {
                if (e.Files.Count() == 0)
                {
                    MediaGalleryHeaderImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task RemoveMedia(bool isImage)
        {
            if (isImage)
                MediaGalleryImage = null;
            else
                MediaGalleryHeaderImage = null;
        }

        private void OnTitleEnChanged(string value)
        {
            if (Id.IsNullOrEmpty())
            {
                NewMediaGallery.TitleEn = value;
                NewMediaGallery.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingMediaGallery.TitleEn = value;
                EditingMediaGallery.Slug = Slug.GenerateSlug(value);
            }
        }

        private async Task SetNewAsync()
        {
            GetMediaGalleriesInput getMediaGalleriesInput = new GetMediaGalleriesInput();
            getMediaGalleriesInput.MaxResultCount = 1;
            PagedResultDto<MediaGalleryDto> MediaGallerys = (await MediaGalleriesAppService.GetListAsync(getMediaGalleriesInput));
            if (MediaGallerys != null)
                NewMediaGallery.Order = Convert.ToInt32(MediaGallerys.TotalCount);
            else
                NewMediaGallery.Order = 0;
            NewMediaGallery.IsActive= true;

        }

    }
}
