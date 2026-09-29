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
using DIP.AmenityParagraphs;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.ZoneParagraphs;
using Blazorise.Extensions;
//test
namespace DIP.Blazor.Pages.Managements.AmenityParagraphs
{
    public partial class EditAmenityParagraphs
    {


        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private AmenityParagraphCreateDto NewAmenityParagraph { get; set; }
        private Validations NewAmenityParagraphValidations { get; set; } = new();
        private AmenityParagraphUpdateDto EditingAmenityParagraph { get; set; }
        private Validations EditingAmenityParagraphValidations { get; set; } = new();
        private Guid EditingAmenityParagraphId { get; set; }

        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private IReadOnlyList<LookupDto<Guid>> AmenitiesCollection { get; set; } = new List<LookupDto<Guid>>();

        private bool CreateCalled { get; set; } = false;
        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;
        public EditAmenityParagraphs()
        {
            NewAmenityParagraph = new AmenityParagraphCreateDto();
            EditingAmenityParagraph = new AmenityParagraphUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetAmenityCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingAmenityParagraphId = Guid.Parse(Id);
                    var amenityParagraphs = await AmenityParagraphsAppService.GetAsync(EditingAmenityParagraphId);
                    EditingAmenityParagraph = ObjectMapper.Map<AmenityParagraphDto, AmenityParagraphUpdateDto>(amenityParagraphs);

                    await EditingAmenityParagraphValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:AmenityParagraphs"],
                   url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        private async Task CreateAmenityParagraphAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewAmenityParagraphValidations.ValidateAll() == false)
                    isValid = false;

                if (HtmlParser.GetCleanedText(NewAmenityParagraph.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewAmenityParagraph.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                await AmenityParagraphsAppService.CreateAsync(NewAmenityParagraph);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateAmenityParagraphAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingAmenityParagraphValidations.ValidateAll() == false)
                    isValid = false;

                if (HtmlParser.GetCleanedText(EditingAmenityParagraph.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(EditingAmenityParagraph.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                await AmenityParagraphsAppService.UpdateAsync(EditingAmenityParagraphId, EditingAmenityParagraph);
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

        async Task OnSelectedValueChanged(Guid value)
        {
            NewAmenityParagraph.AmenityId = value;
        }
        async Task OnSelectedEditValueChanged(Guid value)
        {
            EditingAmenityParagraph.AmenityId = value;
        }
        private async Task GetAmenityCollectionLookupAsync(string? newValue = null)
        {
            AmenitiesCollection = (await AmenityParagraphsAppService.GetAmenityLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }
        private async Task SetNewAsync()
        {
            GetAmenityParagraphsInput getAmenityParagraphsInput = new GetAmenityParagraphsInput();
            getAmenityParagraphsInput.MaxResultCount = 1;
            PagedResultDto<AmenityParagraphWithNavigationPropertiesDto> amenityParagraphs = (await AmenityParagraphsAppService.GetListAsync(getAmenityParagraphsInput));
            if (amenityParagraphs != null)
                NewAmenityParagraph.Order = Convert.ToInt32(amenityParagraphs.TotalCount);
            else
                NewAmenityParagraph.Order = 0;

            if (!AmenitiesCollection.IsNullOrEmpty())
                NewAmenityParagraph.AmenityId = AmenitiesCollection.FirstOrDefault().Id;

        }

        public async Task CreatingDescriptionEnOnContentChanged(string value)
        {
            NewAmenityParagraph.DescriptionEn = value;
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
            NewAmenityParagraph.DescriptionAr = value;
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
            EditingAmenityParagraph.DescriptionEn = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionEnValidationError = false;
            else
                DescriptionEnValidationError = true;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingAmenityParagraph.DescriptionAr = value;
            if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
                DescriptionArValidationError = false;
            else
                DescriptionArValidationError = true;
        }

    }
}
