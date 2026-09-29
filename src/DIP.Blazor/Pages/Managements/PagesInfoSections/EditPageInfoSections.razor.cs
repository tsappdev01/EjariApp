using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.PageInfoSections;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;
using DIP.Helper;
using DIP.Shared;
using Blazorise.Extensions;

namespace DIP.Blazor.Pages.Managements.PagesInfoSections
{
    public partial class EditPageInfoSections
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private PageInfoSectionCreateDto NewPageInfoSection { get; set; }
        private Validations NewPageInfoSectionValidations { get; set; } = new();
        private PageInfoSectionUpdateDto EditingPageInfoSection { get; set; }
        private Validations EditingPageInfoSectionValidations { get; set; } = new();
        private Guid EditingPageInfoSectionId { get; set; }

        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<PageInfoSectionContainer> PageInfoSectionContainer { get; set; }
        public string PageInfoSectionImage { get; set; } = "";
        public byte[] PageInfoSectionImageContent { get; set; }
        public bool PageInfoSectionImageNewUpload { get; set; } = false;

        private IReadOnlyList<LookupDto<Guid>> PageInfosCollection { get; set; } = new List<LookupDto<Guid>>();

        private bool CreateCalled { get; set; } = false;
        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;
        private bool ImageValidationError { get; set; } = false;

        public EditPageInfoSections()
        {
            NewPageInfoSection = new PageInfoSectionCreateDto();
            EditingPageInfoSection = new PageInfoSectionUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetPageInfoCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingPageInfoSectionId = Guid.Parse(Id);
                    var pageInfo = await PageInfoSectionsAppService.GetAsync(EditingPageInfoSectionId);
                    EditingPageInfoSection = ObjectMapper.Map<PageInfoSectionDto, PageInfoSectionUpdateDto>(pageInfo);
                    if (EditingPageInfoSection != null)
                    {
                        PageInfoSectionImage = EditingPageInfoSection?.PageSectionMedia;
                    }
                    await EditingPageInfoSectionValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PageInfoSections"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        private async Task CreatePageInfoSectionAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewPageInfoSectionValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewPageInfoSection.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewPageInfoSection.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (PageInfoSectionImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!PageInfoSectionImage.IsNullOrEmpty() && !PageInfoSectionImageContent.IsNullOrEmpty())
                {
                    await PageInfoSectionContainer.SaveAsync(PageInfoSectionImage, PageInfoSectionImageContent);
                    NewPageInfoSection.PageSectionMedia = PageInfoSectionImage;
                }
                await PageInfoSectionsAppService.CreateAsync(NewPageInfoSection);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task UpdatePageInfoSectionAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingPageInfoSectionValidations.ValidateAll() == false)
                    isValid = false;
                if (HtmlParser.GetCleanedText(EditingPageInfoSection.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }

                if (HtmlParser.GetCleanedText(EditingPageInfoSection.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }
                if (PageInfoSectionImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (!isValid)
                    return;

                if (!PageInfoSectionImage.IsNullOrEmpty() && !PageInfoSectionImageContent.IsNullOrEmpty() && PageInfoSectionImage != EditingPageInfoSection.PageSectionMedia)
                {
                    if (!EditingPageInfoSection.PageSectionMedia.IsNullOrEmpty())
                        await PageInfoSectionContainer.DeleteAsync(EditingPageInfoSection.PageSectionMedia);
                    await PageInfoSectionContainer.SaveAsync(PageInfoSectionImage, PageInfoSectionImageContent);
                    EditingPageInfoSection.PageSectionMedia = PageInfoSectionImage;
                }
                else
                {
                    if (PageInfoSectionImage.IsNullOrEmpty() && !EditingPageInfoSection.PageSectionMedia.IsNullOrEmpty())
                    {
                        await PageInfoSectionContainer.DeleteAsync(EditingPageInfoSection.PageSectionMedia);
                        EditingPageInfoSection.PageSectionMedia = null;
                    }
                }
                await PageInfoSectionsAppService.UpdateAsync(EditingPageInfoSectionId, EditingPageInfoSection);
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
            NewPageInfoSection.DescriptionEn = value;

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
            NewPageInfoSection.DescriptionAr = value;

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
            EditingPageInfoSection.DescriptionEn = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionEnValidationError = false;
            else
                DescriptionEnValidationError = true;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingPageInfoSection.DescriptionAr = value;
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
                    PageInfoSectionImageContent = await result.GetAllBytesAsync();
                    PageInfoSectionImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    PageInfoSectionImageNewUpload = true;

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
                    PageInfoSectionImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task RemoveMedia()
        {
                PageInfoSectionImage = null;
        }


        private async Task GetPageInfoCollectionLookupAsync(string? newValue = null)
        {
            PageInfosCollection = (await PageInfoSectionsAppService.GetPageInfoLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task SetNewAsync()
        {
            GetPageInfoSectionsInput getPageInfoSectionsInput = new GetPageInfoSectionsInput();
            getPageInfoSectionsInput.MaxResultCount = 1;
            PagedResultDto<PageInfoSectionWithNavigationPropertiesDto> pageInfos = (await PageInfoSectionsAppService.GetListAsync(getPageInfoSectionsInput));
            if (pageInfos != null)
                NewPageInfoSection.Order = Convert.ToInt32(pageInfos.TotalCount);
            else
                NewPageInfoSection.Order = 0;
            NewPageInfoSection.IsActive = true;
            if (!PageInfosCollection.IsNullOrEmpty())
                NewPageInfoSection.PageInfoId = PageInfosCollection.Select(i => i.Id).FirstOrDefault();
        }
    }
}
