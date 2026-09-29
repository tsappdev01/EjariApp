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
using DIP.Amenities;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using DIP.Amenities;
using DIP.Helper;
using System.IO;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.BlobStoring;
using DIP.Zones;

namespace DIP.Blazor.Pages.Managements.Amenities
{
    public partial class EditAmenities
    {

        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private AmenityCreateDto NewAmenity { get; set; }
        private Validations NewAmenityValidations { get; set; } = new();
        private AmenityUpdateDto EditingAmenity { get; set; }
        private Validations EditingAmenityValidations { get; set; } = new();
        private Guid EditingAmenityId { get; set; }

        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        [Inject]
        public IBlobContainer<AmenityContainer> AmenityContainer { get; set; }
        public string AmenityImage { get; set; } = "";
        public byte[] AmenityImageContent { get; set; }
        public bool AmenityImageNewUpload { get; set; } = false;
        public string AmenityHeaderImage { get; set; } = "";
        public byte[] AmenityHeaderImageContent { get; set; }
        public bool AmenityHeaderImageNewUpload { get; set; } = false;
        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        private bool ImageValidationError { get; set; } = false;
        private bool HeaderImageValidationError { get; set; } = false;

        public EditAmenities()
        {
            NewAmenity = new AmenityCreateDto();
            EditingAmenity = new AmenityUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingAmenityId = Guid.Parse(Id);
                    var amenitiesPage = await AmenitiesAppService.GetAsync(EditingAmenityId);
                    EditingAmenity = ObjectMapper.Map<AmenityDto, AmenityUpdateDto>(amenitiesPage);
                    if (EditingAmenity != null)
                    {
                        AmenityImage = EditingAmenity?.Image;
                        AmenityHeaderImage = EditingAmenity?.HeaderImage;
                    }
                    await EditingAmenityValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Amenities"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}" ));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }
        private void OnTitleEnChanged(string value)
        {
            if(Id.IsNullOrEmpty())
            {
                NewAmenity.TitleEn = value;
                NewAmenity.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingAmenity.TitleEn = value;
                EditingAmenity.Slug = Slug.GenerateSlug(value);
            }
        }
        private async Task CreateAmenityAsync()
        {
            try
            {
                bool isValid = true;
                if (await NewAmenityValidations.ValidateAll() == false)
                    isValid = false;
                if (AmenityImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (AmenityHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!AmenityImage.IsNullOrEmpty() && !AmenityImageContent.IsNullOrEmpty())
                {
                    await AmenityContainer.SaveAsync(AmenityImage, AmenityImageContent);
                    NewAmenity.Image = AmenityImage;
                }
                if (!AmenityHeaderImage.IsNullOrEmpty() && !AmenityHeaderImageContent.IsNullOrEmpty())
                {
                    await AmenityContainer.SaveAsync(AmenityHeaderImage, AmenityHeaderImageContent);
                    NewAmenity.HeaderImage = AmenityHeaderImage;
                }
                await AmenitiesAppService.CreateAsync(NewAmenity);

                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateAmenityAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingAmenityValidations.ValidateAll() == false)
                    isValid = false;
                if (AmenityImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }
                if (AmenityHeaderImage.IsNullOrEmpty())
                {
                    HeaderImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;
                if (!AmenityImage.IsNullOrEmpty() && !AmenityImageContent.IsNullOrEmpty() && AmenityImage != EditingAmenity.Image)
                {
                    if (!EditingAmenity.Image.IsNullOrEmpty())
                        await AmenityContainer.DeleteAsync(EditingAmenity.Image);
                    await AmenityContainer.SaveAsync(AmenityImage, AmenityImageContent);
                    EditingAmenity.Image = AmenityImage;
                }
                else
                {
                    if (AmenityImage.IsNullOrEmpty() && !EditingAmenity.Image.IsNullOrEmpty())
                    {
                        await AmenityContainer.DeleteAsync(EditingAmenity.Image);
                        EditingAmenity.Image = null;
                    }
                }
                if (!AmenityHeaderImage.IsNullOrEmpty() && !AmenityHeaderImageContent.IsNullOrEmpty() && AmenityHeaderImage != EditingAmenity.HeaderImage)
                {
                    if (!EditingAmenity.HeaderImage.IsNullOrEmpty())
                        await AmenityContainer.DeleteAsync(EditingAmenity.HeaderImage);
                    await AmenityContainer.SaveAsync(AmenityHeaderImage, AmenityHeaderImageContent);
                    EditingAmenity.HeaderImage = AmenityHeaderImage;
                }
                else
                {
                    if (AmenityHeaderImage.IsNullOrEmpty() && !EditingAmenity.HeaderImage.IsNullOrEmpty())
                    {
                        await AmenityContainer.DeleteAsync(EditingAmenity.HeaderImage);
                        EditingAmenity.HeaderImage = null;
                    }
                }
                await AmenitiesAppService.UpdateAsync(EditingAmenityId, EditingAmenity);
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
            {
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
        }
        private async Task SetNewAsync()
        {
            GetAmenitiesInput getAmenitiesInput = new GetAmenitiesInput();
            getAmenitiesInput.MaxResultCount = 1;
            PagedResultDto<AmenityDto> amenities = (await AmenitiesAppService.GetListAsync(getAmenitiesInput));
            if (amenities != null)
                NewAmenity.Order = Convert.ToInt32(amenities.TotalCount);
            else
                NewAmenity.Order = 0;
            
            NewAmenity.IsActive = true;
        }

        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    AmenityImageContent = await result.GetAllBytesAsync();
                    //  ZoneImage = $"{Path.GetFileNameWithoutExtension(e.File.Name)}_{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    AmenityImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    AmenityImageNewUpload = true;
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
                    AmenityImage = null;
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
                    AmenityHeaderImageContent = await result.GetAllBytesAsync();
                   // AmenityHeaderImage = $"{Path.GetFileNameWithoutExtension(e.File.Name)}_{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    AmenityHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    AmenityHeaderImageNewUpload = true;

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
                    AmenityHeaderImage = null;
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
                AmenityImage = null;
            else
                AmenityHeaderImage = null;
        }
    }
}
