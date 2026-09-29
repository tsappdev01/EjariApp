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
using DIP.PageInfos;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using DIP.PageInfos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using DIP.Helper;
using DIP.EServices;

namespace DIP.Blazor.Pages.Managements.PagesInfos
{
    public partial class EditPageInfos
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private PageInfoCreateDto NewPageInfo { get; set; }
        private Validations NewPageInfoValidations { get; set; } = new();
        private PageInfoUpdateDto EditingPageInfo { get; set; }
        private Validations EditingPageInfoValidations { get; set; } = new();
        private Guid EditingPageInfoId { get; set; }

        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<PageInfoContainer> PageInfoContainer { get; set; }
        public string PageInfoImage { get; set; } = "";
        public byte[] PageInfoImageContent { get; set; }
        public bool PageInfoImageNewUpload { get; set; } = false;

        public string PageInfoHeaderImage { get; set; } = "";
        public byte[] PageInfoHeaderImageContent { get; set; }
        public bool PageInfoHeaderImageNewUpload { get; set; } = false;

        private bool HeaderImageValidationError { get; set; } = false;

        public EditPageInfos()
        {
            NewPageInfo = new PageInfoCreateDto();
            EditingPageInfo = new PageInfoUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingPageInfoId = Guid.Parse(Id);
                    var pageInfo = await PageInfosAppService.GetAsync(EditingPageInfoId);
                    EditingPageInfo = ObjectMapper.Map<PageInfoDto, PageInfoUpdateDto>(pageInfo);
                    if (EditingPageInfo != null)
                    {
                        PageInfoImage = EditingPageInfo?.Image;
                        PageInfoHeaderImage = EditingPageInfo?.HeaderImage;
                    }
                    await EditingPageInfoValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PageInfos"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        private async Task CreatePageInfoAsync()
        {
            try
            {
                if (await NewPageInfoValidations.ValidateAll() == false)
                {
                    return;
                }
                if (PageInfoHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    return;
                }
                if (!PageInfoImage.IsNullOrEmpty() && !PageInfoImageContent.IsNullOrEmpty())
                {
                    await PageInfoContainer.SaveAsync(PageInfoImage, PageInfoImageContent);
                    NewPageInfo.Image = PageInfoImage;
                }
                if (!PageInfoHeaderImage.IsNullOrEmpty() && !PageInfoHeaderImageContent.IsNullOrEmpty())
                {
                    await PageInfoContainer.SaveAsync(PageInfoHeaderImage, PageInfoHeaderImageContent);
                    NewPageInfo.HeaderImage = PageInfoHeaderImage;
                }
                await PageInfosAppService.CreateAsync(NewPageInfo);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task UpdatePageInfoAsync()
        {
            try
            {
                if (await EditingPageInfoValidations.ValidateAll() == false)
                {
                    return;
                }
                if (PageInfoHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    return;
                }
                if (!PageInfoImage.IsNullOrEmpty() && !PageInfoImageContent.IsNullOrEmpty() && PageInfoImage != EditingPageInfo.Image)
                {
                    if (!EditingPageInfo.Image.IsNullOrEmpty())
                        await PageInfoContainer.DeleteAsync(EditingPageInfo.Image);
                    await PageInfoContainer.SaveAsync(PageInfoImage, PageInfoImageContent);
                    EditingPageInfo.Image = PageInfoImage;
                }
                else
                {
                    if (PageInfoImage.IsNullOrEmpty() && !EditingPageInfo.Image.IsNullOrEmpty())
                    {
                        await PageInfoContainer.DeleteAsync(EditingPageInfo.Image);
                        EditingPageInfo.Image = null;
                    }
                }
                if (!PageInfoHeaderImage.IsNullOrEmpty() && !PageInfoHeaderImageContent.IsNullOrEmpty() && PageInfoHeaderImage != EditingPageInfo.HeaderImage)
                {
                    if (!EditingPageInfo.HeaderImage.IsNullOrEmpty())
                        await PageInfoContainer.DeleteAsync(EditingPageInfo.HeaderImage);
                    await PageInfoContainer.SaveAsync(PageInfoHeaderImage, PageInfoHeaderImageContent);
                    EditingPageInfo.HeaderImage = PageInfoHeaderImage;
                }
                else
                {
                    if (PageInfoHeaderImage.IsNullOrEmpty() && !EditingPageInfo.HeaderImage.IsNullOrEmpty())
                    {
                        await PageInfoContainer.DeleteAsync(EditingPageInfo.HeaderImage);
                        EditingPageInfo.HeaderImage = null;
                    }
                }
                await PageInfosAppService.UpdateAsync(EditingPageInfoId, EditingPageInfo);
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
            NewPageInfo.DescriptionEn = value;
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewPageInfo.DescriptionAr = value;
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingPageInfo.DescriptionEn = value;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingPageInfo.DescriptionAr = value;
        }
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    PageInfoImageContent = await result.GetAllBytesAsync();
                    PageInfoImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    PageInfoImageNewUpload = true;
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
                    PageInfoImage = null;
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
                    PageInfoHeaderImageContent = await result.GetAllBytesAsync();
                    PageInfoHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    PageInfoHeaderImageNewUpload = true;

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
                    PageInfoHeaderImage = null;
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
                PageInfoImage = null;
            else
                PageInfoHeaderImage = null;
        }

        private void OnTitleEnChanged(string value)
        {
            if (Id.IsNullOrEmpty())
            {
                NewPageInfo.TitleEn = value;
                NewPageInfo.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingPageInfo.TitleEn = value;
                EditingPageInfo.Slug = Slug.GenerateSlug(value);
            }
        }

        private async Task SetNewAsync()
        {
            GetPageInfosInput getPageInfosInput = new GetPageInfosInput();
            getPageInfosInput.MaxResultCount = 1;
            PagedResultDto<PageInfoDto> pageInfos = (await PageInfosAppService.GetListAsync(getPageInfosInput));
            if (pageInfos != null)
                NewPageInfo.Order = Convert.ToInt32(pageInfos.TotalCount);
            else
                NewPageInfo.Order = 0;
            NewPageInfo.IsActive = true;
        }
    }
}
