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
using DIP.ZoneParagraphs;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.LanguageManagement;
using Blazorise.Extensions;

namespace DIP.Blazor.Pages.Managements.ZoneParagraphs
{
    public partial class EditZoneParagraphs
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private ZoneParagraphCreateDto NewZoneParagraph { get; set; }
        private ZoneParagraphUpdateDto EditingZoneParagraph { get; set; }
        private Validations NewZoneParagraphValidations { get; set; } = new();

        private Validations EditingZoneParagraphValidations { get; set; } = new();
        private Guid EditingZoneParagraphId { get; set; }
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private Guid EditingZonehParagraphsId { get; set; }
        private ZoneParagraphUpdateDto EditingZoneParagraphs { get; set; }

        private IReadOnlyList<LookupDto<Guid>> ZonesCollection { get; set; } = new List<LookupDto<Guid>>();

        private bool DescriptionEnValidationError { get; set; } = false;
        private bool DescriptionArValidationError { get; set; } = false;

        public EditZoneParagraphs()
        {
            NewZoneParagraph = new ZoneParagraphCreateDto();
            EditingZoneParagraph = new ZoneParagraphUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetZoneCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingZoneParagraphId = Guid.Parse(Id);
                    var ZoneParagraph = await ZoneParagraphsAppService.GetAsync(EditingZoneParagraphId);
                    EditingZoneParagraph = ObjectMapper.Map<ZoneParagraphDto, ZoneParagraphUpdateDto>(ZoneParagraph);
                 
                    await EditingZoneParagraphValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ZoneParagraphs"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }


        private async Task CreateZoneParagraphAsync()
        {
            try
            {
                bool isValid = true;
                if (await NewZoneParagraphValidations.ValidateAll() == false)
                    isValid = false;
                if (HtmlParser.GetCleanedText(NewZoneParagraph.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(NewZoneParagraph.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                var ent=   await ZoneParagraphsAppService.CreateAsync(NewZoneParagraph);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateZoneParagraphAsync()
        {
            try
            {
                bool isValid = true;
                if (await EditingZoneParagraphValidations.ValidateAll() == false)
                    isValid = false;
                if (HtmlParser.GetCleanedText(EditingZoneParagraph.DescriptionEn).IsNullOrEmpty())
                {
                    DescriptionEnValidationError = true;
                    isValid = false;
                }
                if (HtmlParser.GetCleanedText(EditingZoneParagraph.DescriptionAr).IsNullOrEmpty())
                {
                    DescriptionArValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                await ZoneParagraphsAppService.UpdateAsync(EditingZoneParagraphId, EditingZoneParagraph);
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
            NewZoneParagraph.DescriptionEn = value;
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewZoneParagraph.DescriptionAr= value;
        }
        async Task OnSelectedValueChanged(Guid value)
        {
            NewZoneParagraph.ZoneId = value;
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingZoneParagraph.DescriptionEn = value;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingZoneParagraph.DescriptionAr = value;
        }
        async Task OnSelectedEditValueChanged(Guid value)
        {
            EditingZoneParagraph.ZoneId = value;
        }
        private async Task GetZoneCollectionLookupAsync(string? newValue = null)
        {

            ZonesCollection = (await ZoneParagraphsAppService.GetZoneLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task SetNewAsync()
        {
            GetZoneParagraphsInput getZoneParagraphsInput = new GetZoneParagraphsInput();
            getZoneParagraphsInput.MaxResultCount = 1;
            PagedResultDto<ZoneParagraphWithNavigationPropertiesDto> zoneParagraphs = (await ZoneParagraphsAppService.GetListAsync(getZoneParagraphsInput));
            if (zoneParagraphs != null)
                NewZoneParagraph.Order = Convert.ToInt32(zoneParagraphs.TotalCount);
            else
                NewZoneParagraph.Order = 0;

            if (!ZonesCollection.IsNullOrEmpty())
                NewZoneParagraph.ZoneId = ZonesCollection.FirstOrDefault().Id;

        }

    }
}
