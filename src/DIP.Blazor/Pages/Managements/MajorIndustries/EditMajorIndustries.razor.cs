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
using DIP.MajorIndustries;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.ObjectMapping;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using System.IO;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using DIP.Amenities;
using Blazorise.Extensions;

namespace DIP.Blazor.Pages.Managements.MajorIndustries
{
    public partial class EditMajorIndustries
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private MajorIndustryCreateDto NewMajorIndustry { get; set; }
        private Validations NewMajorIndustryValidations { get; set; } = new();
        private MajorIndustryUpdateDto EditingMajorIndustry { get; set; }
        private Validations EditingMajorIndustryValidations { get; set; } = new();
        private Guid EditingMajorIndustryId { get; set; }
        private Modal CreateMajorIndustryModal { get; set; } = new();
        private Modal EditMajorIndustryModal { get; set; } = new();
        private GetMajorIndustriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<MajorIndustryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private IReadOnlyList<LookupDto<Guid>> ZonesCollection { get; set; } = new List<LookupDto<Guid>>();


        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<MajorIndustryContainer> MajorIndustryContainer { get; set; }
        public string MajorIndustryImage { get; set; } = "";
        public byte[] MajorIndustryImageContent { get; set; }
        public bool MajorIndustryImageNewUpload { get; set; } = false;

        private bool ImageValidationError { get; set; } = false;
        public EditMajorIndustries()
        {
            NewMajorIndustry = new MajorIndustryCreateDto();
            EditingMajorIndustry = new MajorIndustryUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetZoneCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingMajorIndustryId = Guid.Parse(Id);
                    var majorIndustry = await MajorIndustriesAppService.GetAsync(EditingMajorIndustryId);
                    EditingMajorIndustry = ObjectMapper.Map<MajorIndustryDto, MajorIndustryUpdateDto>(majorIndustry);
                    if (EditingMajorIndustry != null)
                    {
                        MajorIndustryImage = EditingMajorIndustry?.Image;
                    }
                    await EditingMajorIndustryValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:MajorIndustries"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }
        private async Task CreateMajorIndustryAsync()
        {
            try
            {
                if (await NewMajorIndustryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (MajorIndustryImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    return;
                }
                if (!MajorIndustryImage.IsNullOrEmpty() && !MajorIndustryImageContent.IsNullOrEmpty())
                {
                    await MajorIndustryContainer.SaveAsync(MajorIndustryImage, MajorIndustryImageContent);
                    NewMajorIndustry.Image = MajorIndustryImage;
                }
                await MajorIndustriesAppService.CreateAsync(NewMajorIndustry);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }


        private async Task UpdateMajorIndustryAsync()
        {
            try
            {
                if (await EditingMajorIndustryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (MajorIndustryImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    return;
                }
                if (!MajorIndustryImage.IsNullOrEmpty() && !MajorIndustryImageContent.IsNullOrEmpty() && MajorIndustryImage != EditingMajorIndustry.Image)
                {
                    if (!EditingMajorIndustry.Image.IsNullOrEmpty())
                        await MajorIndustryContainer.DeleteAsync(EditingMajorIndustry.Image);
                    await MajorIndustryContainer.SaveAsync(MajorIndustryImage, MajorIndustryImageContent);
                    EditingMajorIndustry.Image = MajorIndustryImage;
                }
                else
                {
                    if (MajorIndustryImage.IsNullOrEmpty() && !EditingMajorIndustry.Image.IsNullOrEmpty())
                    {
                        await MajorIndustryContainer.DeleteAsync(EditingMajorIndustry.Image);
                        EditingMajorIndustry.Image = null;
                    }
                }
                await MajorIndustriesAppService.UpdateAsync(EditingMajorIndustryId, EditingMajorIndustry);
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


        private async Task GetZoneCollectionLookupAsync(string? newValue = null)
        {
            ZonesCollection = (await MajorIndustriesAppService.GetZoneLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
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
                    MajorIndustryImageContent = await result.GetAllBytesAsync();
                    MajorIndustryImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    MajorIndustryImageNewUpload = true;

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
                    MajorIndustryImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task RemoveMedia( )
        {

            MajorIndustryImage = null;
           
        }

        private async Task SetNewAsync()
        {
            GetMajorIndustriesInput getMajorIndustriesInput = new GetMajorIndustriesInput();
            getMajorIndustriesInput.MaxResultCount = 1;
            PagedResultDto<MajorIndustryWithNavigationPropertiesDto> majorIndustries = (await MajorIndustriesAppService.GetListAsync(getMajorIndustriesInput));
            if (majorIndustries != null)
                NewMajorIndustry.Order = Convert.ToInt32(majorIndustries.TotalCount);
            else
                NewMajorIndustry.Order = 0;
            NewMajorIndustry.IsActive = true;
            if (!ZonesCollection.IsNullOrEmpty())
                NewMajorIndustry.ZoneId = ZonesCollection.FirstOrDefault().Id;
        }

    }
}
