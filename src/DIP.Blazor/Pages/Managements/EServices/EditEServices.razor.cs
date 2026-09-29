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
using DIP.EServices;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using System.IO;
using Volo.Abp;
using DIP.EServices;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using DIP.Helper;
using DIP.Amenities;
using DIP.LastEventss;

namespace DIP.Blazor.Pages.Managements.EServices
{
    public partial class EditEServices
    {

        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private bool CanCreateEService { get; set; }
        private bool CanEditEService { get; set; }
        private bool CanDeleteEService { get; set; }
        private EServiceCreateDto NewEService { get; set; }
        private Validations NewEServiceValidations { get; set; } = new();
        private EServiceUpdateDto EditingEService { get; set; }
        private Validations EditingEServiceValidations { get; set; } = new();
        private Guid EditingEServiceId { get; set; }
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<EServiceContainer> EServiceContainer { get; set; }
        public string EServiceImage { get; set; } = "";
        public byte[] EServiceImageContent { get; set; }
        public bool EServiceImageNewUpload { get; set; } = false;

        public string EServiceHeaderImage { get; set; } = "";
        public byte[] EServiceHeaderImageContent { get; set; }
        public bool EServiceHeaderImageNewUpload { get; set; } = false;


        public EditEServices()
        {
            NewEService = new EServiceCreateDto();
            EditingEService = new EServiceUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingEServiceId = Guid.Parse(Id);
                    var eService = await EServicesAppService.GetAsync(EditingEServiceId);
                    EditingEService = ObjectMapper.Map<EServiceDto, EServiceUpdateDto>(eService); if (EditingEService != null)
                    {
                        EServiceImage = EditingEService?.Image;
                        EServiceHeaderImage = EditingEService?.HeaderImage;
                    }
                    await EditingEServiceValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EServices"],
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
                NewEService.TitleEn = value;
                NewEService.Slug = Slug.GenerateSlug(value);
            }
            else
            {
                EditingEService.TitleEn = value;
                EditingEService.Slug = Slug.GenerateSlug(value);
            }
        }
   
        private async Task CreateEServiceAsync()
        {
            try
            {
                if (await NewEServiceValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!EServiceImage.IsNullOrEmpty() && !EServiceImageContent.IsNullOrEmpty())
                {
                    await EServiceContainer.SaveAsync(EServiceImage, EServiceImageContent);
                    NewEService.Image = EServiceImage;
                }
                if (!EServiceHeaderImage.IsNullOrEmpty() && !EServiceHeaderImageContent.IsNullOrEmpty())
                {
                    await EServiceContainer.SaveAsync(EServiceHeaderImage, EServiceHeaderImageContent);
                    NewEService.HeaderImage = EServiceHeaderImage;
                }
                await EServicesAppService.CreateAsync(NewEService);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateEServiceAsync()
        {
            try
            {
                if (await EditingEServiceValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!EServiceImage.IsNullOrEmpty() && !EServiceImageContent.IsNullOrEmpty() && EServiceImage != EditingEService.Image)
                {
                    if (!EditingEService.Image.IsNullOrEmpty())
                        await EServiceContainer.DeleteAsync(EditingEService.Image);
                    await EServiceContainer.SaveAsync(EServiceImage, EServiceImageContent);
                    EditingEService.Image = EServiceImage;
                }
                else
                {
                    if (EServiceImage.IsNullOrEmpty() && !EditingEService.Image.IsNullOrEmpty())
                    {
                        await EServiceContainer.DeleteAsync(EditingEService.Image);
                        EditingEService.Image = null;
                    }
                }
                if (!EServiceHeaderImage.IsNullOrEmpty() && !EServiceHeaderImageContent.IsNullOrEmpty() && EServiceHeaderImage != EditingEService.HeaderImage)
                {
                    if (!EditingEService.HeaderImage.IsNullOrEmpty())
                        await EServiceContainer.DeleteAsync(EditingEService.HeaderImage);
                    await EServiceContainer.SaveAsync(EServiceHeaderImage, EServiceHeaderImageContent);
                    EditingEService.HeaderImage = EServiceHeaderImage;
                }
                else
                {
                    if (EServiceHeaderImage.IsNullOrEmpty() && !EditingEService.HeaderImage.IsNullOrEmpty())
                    {
                        await EServiceContainer.DeleteAsync(EditingEService.HeaderImage);
                        EditingEService.HeaderImage = null;
                    }
                }
                await EServicesAppService.UpdateAsync(EditingEServiceId, EditingEService);
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
            NewEService.DescriptionEn = value;
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewEService.DescriptionAr = value;
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingEService.DescriptionEn = value;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingEService.DescriptionAr = value;
        }
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    EServiceImageContent = await result.GetAllBytesAsync();
                    EServiceImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    EServiceImageNewUpload = true;
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
                    EServiceImage = null;
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
                    EServiceHeaderImageContent = await result.GetAllBytesAsync();
                    EServiceHeaderImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    EServiceHeaderImageNewUpload = true;
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
                    EServiceHeaderImage = null;
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
                EServiceImage = null;
            else
                EServiceHeaderImage = null;
        }

        private async Task SetNewAsync()
        {
            GetEServicesInput getEServicesInput = new GetEServicesInput();
            getEServicesInput.MaxResultCount = 1;
            PagedResultDto<EServiceDto> eServices = (await EServicesAppService.GetListAsync(getEServicesInput));
            if (eServices != null)
                NewEService.Order = Convert.ToInt32(eServices.TotalCount);
            else
                NewEService.Order = 0;
            NewEService.IsActive = true;
        }
    }
}
