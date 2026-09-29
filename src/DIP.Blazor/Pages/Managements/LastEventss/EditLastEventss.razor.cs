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
using DIP.LastEventss;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using DIP.LastEventss;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using DIP.Helper;
using DIP.Amenities;

namespace DIP.Blazor.Pages.Managements.LastEventss
{
    public partial class EditLastEventss
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private LastEventsCreateDto NewLastEvents { get; set; }
        private Validations NewLastEventsValidations { get; set; } = new();
        private LastEventsUpdateDto EditingLastEvents { get; set; }
        private Validations EditingLastEventsValidations { get; set; } = new();
        private Guid EditingLastEventsId { get; set; }
        private Modal CreateLastEventsModal { get; set; } = new();
        private Modal EditLastEventsModal { get; set; } = new();

        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<LastEventsContainer> LastEventsContainer { get; set; }
        public string LastEventsImage { get; set; } = "";
        public byte[] LastEventsImageContent { get; set; }
        public bool LastEventsImageNewUpload { get; set; } = false;

        public string LastEventsHeaderImage { get; set; } = "";
        public byte[] LastEventsHeaderImageContent { get; set; }
        public bool LastEventsHeaderImageNewUpload { get; set; } = false;

        private bool CreateCalled { get; set; } = false;
        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;
        private bool ImageValidationError { get; set; } = false;
        private bool HeaderImageValidationError { get; set; } = false;
        public EditLastEventss()
        {
            NewLastEvents = new LastEventsCreateDto();
            EditingLastEvents = new LastEventsUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingLastEventsId = Guid.Parse(Id);
                    var lastEvents = await LastEventssAppService.GetAsync(EditingLastEventsId);
                    EditingLastEvents = ObjectMapper.Map<LastEventsDto, LastEventsUpdateDto>(lastEvents);
                    if (EditingLastEvents != null)
                    {
                        LastEventsImage = EditingLastEvents?.Image;
                        LastEventsHeaderImage = EditingLastEvents?.HeaderImage;
                    }
                    await EditingLastEventsValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:LastEventss"],
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
                NewLastEvents.TitleEn = value;
                NewLastEvents.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingLastEvents.TitleEn = value;
                EditingLastEvents.Slug = Slug.GenerateSlug(value);
            }
        }


        private async Task CreateLastEventsAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewLastEventsValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewLastEvents.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewLastEvents.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (LastEventsImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (LastEventsHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!LastEventsImage.IsNullOrEmpty() && !LastEventsImageContent.IsNullOrEmpty())
                {
                    await LastEventsContainer.SaveAsync(LastEventsImage, LastEventsImageContent);
                    NewLastEvents.Image = LastEventsImage;
                }
                if (!LastEventsHeaderImage.IsNullOrEmpty() && !LastEventsHeaderImageContent.IsNullOrEmpty())
                {
                    await LastEventsContainer.SaveAsync(LastEventsHeaderImage, LastEventsHeaderImageContent);
                    NewLastEvents.HeaderImage = LastEventsHeaderImage;
                }
                await LastEventssAppService.CreateAsync(NewLastEvents);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task UpdateLastEventsAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingLastEventsValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(EditingLastEvents.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }

                 if (HtmlParser.GetCleanedText(EditingLastEvents.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (LastEventsImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (LastEventsHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!LastEventsImage.IsNullOrEmpty() && !LastEventsImageContent.IsNullOrEmpty() && LastEventsImage != EditingLastEvents.Image)
                {
                    if (!EditingLastEvents.Image.IsNullOrEmpty())
                        await LastEventsContainer.DeleteAsync(EditingLastEvents.Image);
                    await LastEventsContainer.SaveAsync(LastEventsImage, LastEventsImageContent);
                    EditingLastEvents.Image = LastEventsImage;
                }
                else
                {
                    if (LastEventsImage.IsNullOrEmpty() && !EditingLastEvents.Image.IsNullOrEmpty())
                    {
                        await LastEventsContainer.DeleteAsync(EditingLastEvents.Image);
                        EditingLastEvents.Image = null;
                    }
                }
                if (!LastEventsHeaderImage.IsNullOrEmpty() && !LastEventsHeaderImageContent.IsNullOrEmpty() && LastEventsHeaderImage != EditingLastEvents.HeaderImage)
                {
                    if (!EditingLastEvents.HeaderImage.IsNullOrEmpty())
                        await LastEventsContainer.DeleteAsync(EditingLastEvents.HeaderImage);
                    await LastEventsContainer.SaveAsync(LastEventsHeaderImage, LastEventsHeaderImageContent);
                    EditingLastEvents.HeaderImage = LastEventsHeaderImage;
                }
                else
                {
                    if (LastEventsHeaderImage.IsNullOrEmpty() && !EditingLastEvents.HeaderImage.IsNullOrEmpty())
                    {
                        await LastEventsContainer.DeleteAsync(EditingLastEvents.HeaderImage);
                        EditingLastEvents.HeaderImage = null;
                    }
                }
                await LastEventssAppService.UpdateAsync(EditingLastEventsId, EditingLastEvents);
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
        public async Task CreatingDescriptionEnOnContentChanged(string value)
        {
            NewLastEvents.DescriptionEn = value;
            if (CreateCalled)
            {
                if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                    DescriptionEnValidationError = false;
                else
                    DescriptionEnValidationError = true;
            }
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewLastEvents.DescriptionAr = value;
            if (CreateCalled)
            {
                if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                    DescriptionArValidationError = false;
                else
                    DescriptionArValidationError = true;
            }
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingLastEvents.DescriptionEn = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionEnValidationError = false;
            else
                DescriptionEnValidationError = true;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingLastEvents.DescriptionAr = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionArValidationError = false;
            else
                DescriptionArValidationError = true;
        }
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    LastEventsImageContent = await result.GetAllBytesAsync();
                    LastEventsImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    LastEventsImageNewUpload = true;

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
                    LastEventsImage = null;
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
                    LastEventsHeaderImageContent = await result.GetAllBytesAsync();
                    LastEventsHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    LastEventsHeaderImageNewUpload = true;

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
                    LastEventsHeaderImage = null;
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
                LastEventsImage = null;
            else
                LastEventsHeaderImage = null;
        }
        private async Task SetNewAsync()
        {
            GetLastEventssInput getLastEventssInput = new GetLastEventssInput();
            getLastEventssInput.MaxResultCount = 1;
            PagedResultDto<LastEventsDto> lastEvents = (await LastEventssAppService.GetListAsync(getLastEventssInput));
            if (lastEvents != null)
                NewLastEvents.Order = Convert.ToInt32(lastEvents.TotalCount);
            else
                NewLastEvents.Order = 0;
            NewLastEvents.StartDate = DateTime.Now;
            NewLastEvents.EndDate = DateTime.Now;
            NewLastEvents.IsActive = true;
        }
    }
}
