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
using DIP.SliderHomePages;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.LanguageManagement;
using Volo.Abp.BlobStoring;
using System.IO;
using Volo.Abp;
using Volo.Abp.Guids;
using DIP.Amenities;

namespace DIP.Blazor.Pages.Managements.SliderHomePages
{
    public partial class EditSliderHomePages
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private SliderHomePageCreateDto NewSliderHomePage { get; set; }
        private SliderHomePageUpdateDto EditingSliderHomePage { get; set; }
        private Validations NewSliderHomePageValidations { get; set; } = new();

        private Validations EditingSliderHomePageValidations { get; set; } = new();
        private DataGridEntityActionsColumn<SliderHomePageDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private Guid EditingSliderHomePagesId { get; set; }

        public IBlobContainer<SliderHomeContainer> SliderHomeContainer { get; set; }
        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        public string SliderImage { get; set; } = "";
        public byte[] SliderImageContent { get; set; }
        public bool SliderImageNewUpload { get; set; } = false;
   
        private bool ImageOrYoutubeError { get; set; } = false;
        public EditSliderHomePages()
        {
            NewSliderHomePage = new SliderHomePageCreateDto();
            EditingSliderHomePage = new SliderHomePageUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingSliderHomePagesId = Guid.Parse(Id);
                    var sliderHomePage = await SliderHomePagesAppService.GetAsync(EditingSliderHomePagesId);
                    EditingSliderHomePage = ObjectMapper.Map<SliderHomePageDto, SliderHomePageUpdateDto>(sliderHomePage);
                    if (EditingSliderHomePage != null)
                    {
                        SliderImage = EditingSliderHomePage?.Image;
                    }
                    await EditingSliderHomePageValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SliderHomePages"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }


        private async Task CreateSliderHomePageAsync()
        {
            try
            {

                if (await NewSliderHomePageValidations.ValidateAll() == false)
                {
                    return;
                }
                if (SliderImage.IsNullOrEmpty() && NewSliderHomePage != null && NewSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                {
                    StateHasChanged();
                    ImageOrYoutubeError = true;
                    return;
                }
                if (!SliderImage.IsNullOrEmpty() && !SliderImageContent.IsNullOrEmpty())
                {
                    await SliderHomeContainer.SaveAsync(SliderImage, SliderImageContent);
                    NewSliderHomePage.Image = SliderImage;
                }
                await SliderHomePagesAppService.CreateAsync(NewSliderHomePage);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/",  NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateSliderHomePageAsync()
        {
            try
            {
                if (await EditingSliderHomePageValidations.ValidateAll() == false)
                {
                    return;
                }
                if (SliderImage.IsNullOrEmpty() && EditingSliderHomePage != null && EditingSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                {
                    StateHasChanged();
                    ImageOrYoutubeError = true;
                    return;
                }
                if (!SliderImage.IsNullOrEmpty() && !SliderImageContent.IsNullOrEmpty() && SliderImage != EditingSliderHomePage.Image)
                {
                    if (!EditingSliderHomePage.Image.IsNullOrEmpty())
                        await SliderHomeContainer.DeleteAsync(EditingSliderHomePage.Image);
                    await SliderHomeContainer.SaveAsync(SliderImage, SliderImageContent);
                    EditingSliderHomePage.Image = SliderImage;
                }
                else
                {
                    if (SliderImage.IsNullOrEmpty() && !EditingSliderHomePage.Image.IsNullOrEmpty())
                    {
                        await SliderHomeContainer.DeleteAsync(EditingSliderHomePage.Image);
                        EditingSliderHomePage.Image = null;
                    }
                }
                await SliderHomePagesAppService.UpdateAsync(EditingSliderHomePagesId, EditingSliderHomePage);
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
            EditingSliderHomePage.DescriptionEn = value;
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            EditingSliderHomePage.DescriptionAr= value;
        }

        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingSliderHomePage.DescriptionEn = value;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingSliderHomePage.DescriptionAr = value;
        }
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    SliderImageContent = await result.GetAllBytesAsync();
                    SliderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    SliderImageNewUpload = true;
                }
                if (Id.IsNullOrEmpty())
                {
                    if (SliderImage == null && NewSliderHomePage != null && NewSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                }
                else
                {
                    if (SliderImage == null && EditingSliderHomePage != null && EditingSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                    else
                        ImageOrYoutubeError = false;
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
                    SliderImage = null;
                }
                if (Id.IsNullOrEmpty())
                {
                    if (SliderImage == null && NewSliderHomePage != null && NewSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                }
                else
                {
                    if (SliderImage == null && EditingSliderHomePage != null && EditingSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                    else
                        ImageOrYoutubeError = false;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }  

        private async Task RemoveMedia()
        {
                SliderImage = null;
                if (Id.IsNullOrEmpty())
                {
                    if (SliderImage == null && NewSliderHomePage != null && NewSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                }
                else
                {
                    if (SliderImage == null && EditingSliderHomePage != null && EditingSliderHomePage.YoutubeUrl.IsNullOrEmpty())
                        ImageOrYoutubeError = true;
                    else
                        ImageOrYoutubeError = false;
                }

        }

        private async Task SetNewAsync()
        {
            GetSliderHomePagesInput getSliderHomePagesInput = new GetSliderHomePagesInput();
            getSliderHomePagesInput.MaxResultCount = 1;
            PagedResultDto<SliderHomePageDto> sliderHomePages = (await SliderHomePagesAppService.GetListAsync(getSliderHomePagesInput));
            if (sliderHomePages != null)
                NewSliderHomePage.Order = Convert.ToInt32(sliderHomePages.TotalCount) + 1;
            else
                NewSliderHomePage.Order = 1;
            NewSliderHomePage.IsActive = true;
        }

        private void OnYoutubeUrlChanged(string value)
        {
            if (Id.IsNullOrEmpty())
                NewSliderHomePage.TitleEn = value;
            else
                EditingSliderHomePage.TitleEn = value;

            if (value.IsNullOrEmpty() && SliderImage == null)
                ImageOrYoutubeError = true;
            else
                ImageOrYoutubeError = false;
        }
    }
}
