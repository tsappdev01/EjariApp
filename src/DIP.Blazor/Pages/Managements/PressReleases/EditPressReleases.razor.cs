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
using DIP.PressReleases;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using DIP.PressReleases;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using DIP.PressReleases;
using DIP.Helper;
using DIP.PageInfos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DIP.Blazor.Pages.Managements.PressReleases
{
    public partial class EditPressReleases
    {

        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private PressReleaseCreateDto NewPressRelease { get; set; }
        private Validations NewPressReleaseValidations { get; set; } = new();
        private PressReleaseUpdateDto EditingPressRelease { get; set; }
        private Validations EditingPressReleaseValidations { get; set; } = new();
        private Guid EditingPressReleaseId { get; set; }
        private Modal CreatePressReleaseModal { get; set; } = new();
        private Modal EditPressReleaseModal { get; set; } = new();
        private GetPressReleasesInput Filter { get; set; }
        private DataGridEntityActionsColumn<PressReleaseDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";


        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<PressReleaseContainer> PressReleaseContainer { get; set; }
        public string PressReleaseImage { get; set; } = "";
        public byte[] PressReleaseImageContent { get; set; }
        public bool PressReleaseImageNewUpload { get; set; } = false;

        public string PressReleaseHeaderImage { get; set; } = "";
        public byte[] PressReleaseHeaderImageContent { get; set; }
        public bool PressReleaseHeaderImageNewUpload { get; set; } = false;

        private bool CreateCalled { get; set; } = false;
        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;
        private bool ImageValidationError { get; set; } = false;
        private bool HeaderImageValidationError { get; set; } = false;

        public EditPressReleases()
        {
            NewPressRelease = new PressReleaseCreateDto();
            EditingPressRelease = new PressReleaseUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingPressReleaseId = Guid.Parse(Id);
                    var pressRelease = await PressReleasesAppService.GetAsync(EditingPressReleaseId);
                    EditingPressRelease = ObjectMapper.Map<PressReleaseDto, PressReleaseUpdateDto>(pressRelease);
                    if (EditingPressRelease != null)
                    {
                        PressReleaseImage = EditingPressRelease?.Image;
                        PressReleaseHeaderImage = EditingPressRelease?.HeaderImage;
                    }
                    await EditingPressReleaseValidations.ClearAll();
                }
                catch (Exception ex)
                {

                    //await uiMessageService.Error("Error in get data");
                    Logger.LogError(ex, "Error in get data");
                    //NavigationManager.NavigateTo("/page-informations");
                    NavigationManager.NavigateTo($"/{System.String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
                    //{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/')?[0] ?? ""}
                    await HandleErrorAsync(ex);
                }
            }
            else
                await SetNewAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PressReleases"],
                 url: $"{System.String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
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
                NewPressRelease.TitleEn = value;
                NewPressRelease.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingPressRelease.TitleEn = value;
                EditingPressRelease.Slug = Slug.GenerateSlug(value);
            }
        }

        private async Task CreatePressReleaseAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewPressReleaseValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewPressRelease.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewPressRelease.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (PressReleaseImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (PressReleaseHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!PressReleaseImage.IsNullOrEmpty() && !PressReleaseImageContent.IsNullOrEmpty())
                {
                    await PressReleaseContainer.SaveAsync(PressReleaseImage, PressReleaseImageContent);
                    NewPressRelease.Image = PressReleaseImage;
                }
                if (!PressReleaseHeaderImage.IsNullOrEmpty() && !PressReleaseHeaderImageContent.IsNullOrEmpty())
                {
                    await PressReleaseContainer.SaveAsync(PressReleaseHeaderImage, PressReleaseHeaderImageContent);
                    NewPressRelease.HeaderImage = PressReleaseHeaderImage;
                }
                await PressReleasesAppService.CreateAsync(NewPressRelease);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{System.String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdatePressReleaseAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingPressReleaseValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(EditingPressRelease.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }

                if (HtmlParser.GetCleanedText(EditingPressRelease.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (PressReleaseImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (PressReleaseHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!PressReleaseImage.IsNullOrEmpty() && !PressReleaseImageContent.IsNullOrEmpty() && PressReleaseImage != EditingPressRelease.Image)
                {
                    if (!EditingPressRelease.Image.IsNullOrEmpty())
                        await PressReleaseContainer.DeleteAsync(EditingPressRelease.Image);
                    await PressReleaseContainer.SaveAsync(PressReleaseImage, PressReleaseImageContent);
                    EditingPressRelease.Image = PressReleaseImage;
                }
                else
                {
                    if (PressReleaseImage.IsNullOrEmpty() && !EditingPressRelease.Image.IsNullOrEmpty())
                    {
                        await PressReleaseContainer.DeleteAsync(EditingPressRelease.Image);
                        EditingPressRelease.Image = null;
                    }
                }
                if (!PressReleaseHeaderImage.IsNullOrEmpty() && !PressReleaseHeaderImageContent.IsNullOrEmpty() && PressReleaseHeaderImage != EditingPressRelease.HeaderImage)
                {
                    if (!EditingPressRelease.HeaderImage.IsNullOrEmpty())
                        await PressReleaseContainer.DeleteAsync(EditingPressRelease.HeaderImage);
                    await PressReleaseContainer.SaveAsync(PressReleaseHeaderImage, PressReleaseHeaderImageContent);
                    EditingPressRelease.HeaderImage = PressReleaseHeaderImage;
                }
                else
                {
                    if (PressReleaseHeaderImage.IsNullOrEmpty() && !EditingPressRelease.HeaderImage.IsNullOrEmpty())
                    {
                        await PressReleaseContainer.DeleteAsync(EditingPressRelease.HeaderImage);
                        EditingPressRelease.HeaderImage = null;
                    }
                }
                await PressReleasesAppService.UpdateAsync(EditingPressReleaseId, EditingPressRelease);
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{System.String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
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
                NavigationManager.NavigateTo($"{System.String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }


        public async Task CreatingDescriptionEnOnContentChanged(string value)
        {
            NewPressRelease.DescriptionEn = value;
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
            NewPressRelease.DescriptionAr = value;
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
            EditingPressRelease.DescriptionEn = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionEnValidationError = false;
            else
                DescriptionEnValidationError = true;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingPressRelease.DescriptionAr = value;
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
                    PressReleaseImageContent = await result.GetAllBytesAsync();
                    PressReleaseImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    PressReleaseImageNewUpload = true;

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
                    PressReleaseImage = null;
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
                    PressReleaseHeaderImageContent = await result.GetAllBytesAsync();
                    PressReleaseHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    PressReleaseHeaderImageNewUpload = true;

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
                    PressReleaseHeaderImage = null;
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
            {
                PressReleaseImage = null;
                ImageValidationError = true;
            }
            else
            {
                PressReleaseHeaderImage = null;
                HeaderImageValidationError = true;
            }
        }

        private async Task SetNewAsync()
        {
            GetPressReleasesInput getPressReleasesInput = new GetPressReleasesInput();
            getPressReleasesInput.MaxResultCount = 1;
            PagedResultDto<PressReleaseDto> pressReleases = (await PressReleasesAppService.GetListAsync(getPressReleasesInput));
            if (pressReleases != null)
                NewPressRelease.Order = Convert.ToInt32(pressReleases.TotalCount);
            else
                NewPressRelease.Order = 0;
            NewPressRelease.Date = DateTime.Now;
            NewPressRelease.IsActive = true;
        }
    }
}
