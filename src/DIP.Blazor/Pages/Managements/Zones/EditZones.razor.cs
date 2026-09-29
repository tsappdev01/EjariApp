using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.Zones;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.LanguageManagement;
using DIP.ZoneParagraphs;
using System.IO;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.BlobStoring;
using DIP.Helper;

namespace DIP.Blazor.Pages.Managements.Zones
{
    public partial class EditZones
    {
        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private ZoneCreateDto NewZone { get; set; }
        private ZoneUpdateDto EditingZone { get; set; }
        private Validations NewZoneValidations { get; set; } = new();

        private Validations EditingZoneValidations { get; set; } = new();
        private Guid EditingZoneId { get; set; }
        
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<ZoneContainer> ZoneContainer { get; set; }
        public string ZoneImage { get; set; } = "";
        public byte[] ZoneImageContent { get; set; }
        public bool ZoneImageNewUpload { get; set; } = false;

        public string ZoneHeaderImage { get; set; } = "";
        public byte[] ZoneHeaderImageContent { get; set; }
        public bool ZoneHeaderImageNewUpload { get; set; } = false;

        private bool CreateCalled { get; set; } = false;
        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;
        private bool ImageValidationError { get; set; } = false;
        private bool HeaderImageValidationError { get; set; } = false;

        public EditZones()
        {
            NewZone = new ZoneCreateDto();
            EditingZone = new ZoneUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingZoneId = Guid.Parse(Id);
                    var Zone = await ZonesAppService.GetAsync(EditingZoneId);
                    EditingZone = ObjectMapper.Map<ZoneDto, ZoneUpdateDto>(Zone);
                    if (EditingZone != null)
                    {
                        ZoneImage = EditingZone?.Image;
                        ZoneHeaderImage = EditingZone?.HeaderImage;
                    }
                    await EditingZoneValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Zones"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }
        private void OnTitleEnChanged(string value)
        {
            if (Id.IsNullOrEmpty())
            {
                NewZone.TitleEn = value;
                NewZone.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingZone.TitleEn = value;
                EditingZone.Slug = Slug.GenerateSlug(value);
            }
        }
        private async Task CreateZoneAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewZoneValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                //if (HtmlParser.GetCleanedText(NewZone.DescriptionEn).IsNullOrEmpty())
                //{
                //    DescriptionEnValidationError = true;
                //    isValid = false;
                //}
                //if (HtmlParser.GetCleanedText(NewZone.DescriptionAr).IsNullOrEmpty())
                //{
                //    DescriptionArValidationError = true;
                //    isValid = false;
                //}
                if (ZoneImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (ZoneHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!ZoneImage.IsNullOrEmpty() && !ZoneImageContent.IsNullOrEmpty())
                {
                    await ZoneContainer.SaveAsync(ZoneImage, ZoneImageContent);
                    NewZone.Image = ZoneImage;
                }
                if (!ZoneHeaderImage.IsNullOrEmpty() && !ZoneHeaderImageContent.IsNullOrEmpty())
                {
                    await ZoneContainer.SaveAsync(ZoneHeaderImage, ZoneHeaderImageContent);
                    NewZone.HeaderImage = ZoneHeaderImage;
                }
                var ent=   await ZonesAppService.CreateAsync(NewZone);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateZoneAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingZoneValidations.ValidateAll() == false)
                    isValid = false;
                //if (HtmlParser.GetCleanedText(EditingZone.DescriptionEn).IsNullOrEmpty())
                //{
                //    DescriptionEnValidationError = true;
                //    isValid = false;
                //}
                //if (HtmlParser.GetCleanedText(EditingZone.DescriptionAr).IsNullOrEmpty())
                //{
                //    DescriptionArValidationError = true;
                //    isValid = false;
                //}
                if (ZoneImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (ZoneHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;
  
                if (!ZoneImage.IsNullOrEmpty() && !ZoneImageContent.IsNullOrEmpty() && ZoneImage != EditingZone.Image)
                {
                    if (!EditingZone.Image.IsNullOrEmpty())
                        await ZoneContainer.DeleteAsync(EditingZone.Image);
                    await ZoneContainer.SaveAsync(ZoneImage, ZoneImageContent);
                    EditingZone.Image = ZoneImage;
                }
                else
                {
                    if (ZoneImage.IsNullOrEmpty() && !EditingZone.Image.IsNullOrEmpty())
                    {
                        await ZoneContainer.DeleteAsync(EditingZone.Image);
                        EditingZone.Image = null;
                    }
                }
                if (!ZoneHeaderImage.IsNullOrEmpty() && !ZoneHeaderImageContent.IsNullOrEmpty() && ZoneHeaderImage != EditingZone.HeaderImage)
                {
                    if (!EditingZone.HeaderImage.IsNullOrEmpty())
                        await ZoneContainer.DeleteAsync(EditingZone.HeaderImage);
                    await ZoneContainer.SaveAsync(ZoneHeaderImage, ZoneHeaderImageContent);
                    EditingZone.HeaderImage = ZoneHeaderImage;
                }
                else
                {
                    if (ZoneHeaderImage.IsNullOrEmpty() && !EditingZone.HeaderImage.IsNullOrEmpty())
                    {
                        await ZoneContainer.DeleteAsync(EditingZone.HeaderImage);
                        EditingZone.HeaderImage = null;
                    }
                }
                await ZonesAppService.UpdateAsync(EditingZoneId, EditingZone);
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

        private async Task SetNewAsync()
        {
            GetZonesInput getZonesInput = new GetZonesInput();
            getZonesInput.MaxResultCount = 1;
            PagedResultDto<ZoneDto> zones = (await ZonesAppService.GetListAsync(getZonesInput));
            if (zones != null)
                NewZone.Order = Convert.ToInt32(zones.TotalCount);
            else
                NewZone.Order = 0;

            NewZone.IsFeature = true;
            NewZone.IsActive = true;
        }

        //public async Task CreatingDescriptionEnOnContentChanged(string value)
        //{
        //    NewZone.DescriptionEn = value;
        //    if (CreateCalled)
        //    {
        //        if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
        //            DescriptionEnValidationError = false;
        //        else
        //            DescriptionEnValidationError = true;
        //    }
        //}
        //public async Task CreatingDescriptionArOnContentChanged(string value)
        //{
        //    NewZone.DescriptionAr = value;
        //    if (CreateCalled)
        //    {
        //        if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
        //            DescriptionArValidationError = false;
        //        else
        //            DescriptionArValidationError = true;
        //    }
        //}
        //public async Task EditingDescriptionEnOnContentChanged(string value)
        //{
        //    EditingZone.DescriptionEn = value;
        //    if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
        //        DescriptionEnValidationError = false;
        //    else
        //        DescriptionEnValidationError = true;
        //}
        //public async Task EditingDescriptionArOnContentChanged(string value)
        //{
        //    EditingZone.DescriptionAr = value;
        //    if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
        //        DescriptionArValidationError = false;
        //    else
        //        DescriptionArValidationError = true;
        //}
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    ZoneImageContent = await result.GetAllBytesAsync();
                  //  ZoneImage = $"{Path.GetFileNameWithoutExtension(e.File.Name)}_{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    ZoneImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    ZoneImageNewUpload = true;
                    ImageValidationError = false;
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
                    ZoneImage = null;
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
                    ZoneHeaderImageContent = await result.GetAllBytesAsync();
                 //   ZoneHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    ZoneHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    ZoneHeaderImageNewUpload = true;
                    HeaderImageValidationError = false;
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
                    ZoneHeaderImage = null;
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
                ZoneImage = null;
            else
                ZoneHeaderImage = null;
        }
    }
}
