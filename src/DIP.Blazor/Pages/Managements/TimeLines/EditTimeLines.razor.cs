using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using DIP.TimeLines;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.Extensions.Logging;
using Blazorise.Extensions;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using DIP.Zones;
using System.IO;
using Volo.Abp;

namespace DIP.Blazor.Pages.Managements.TimeLines
{
    public partial class EditTimeLines
    {
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();

        private TimeLineCreateDto NewTimeLine { get; set; }
        private TimeLineUpdateDto EditingTimeLine { get; set; }
        private Validations NewTimeLineValidations { get; set; } = new();

        private Validations EditingTimeLineValidations { get; set; } = new();
        private Guid EditingTimeLineId { get; set; }
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private Guid EditingTimeLinehParagraphsId { get; set; }

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<TimeLineContainer> TimeLineContainer { get; set; }
        public string TimeLineImage { get; set; } = "";
        public byte[] TimeLineImageContent { get; set; }
        public bool TimeLineImageNewUpload { get; set; } = false;

        private IReadOnlyList<LookupDto<Guid>> TimeLineCategoriesCollection { get; set; } = new List<LookupDto<Guid>>();
        private bool CreateCalled { get; set; } = false;
        //private bool DescriptionEnValidationError { get; set; } = false;
        //private bool DescriptionArValidationError { get; set; } = false;
        private bool ImageValidationError { get; set; } = false;
        DatePicker<DateTime?> datePicker;

        public EditTimeLines()
        {
            NewTimeLine = new TimeLineCreateDto();
            EditingTimeLine = new TimeLineUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await GetTimeLineCategoryCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingTimeLineId = Guid.Parse(Id);
                    var TimeLine = await TimeLinesAppService.GetAsync(EditingTimeLineId);
                    EditingTimeLine = ObjectMapper.Map<TimeLineDto, TimeLineUpdateDto>(TimeLine);
                    if (EditingTimeLine != null)
                    {
                        TimeLineImage = EditingTimeLine?.Image;
                    }
                    await EditingTimeLineValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:TimeLines"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        private async Task CreateTimeLineAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await NewTimeLineValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                //if (HtmlParser.GetCleanedText(NewTimeLine.DescriptionEn).IsNullOrEmpty())
                //{
                //    DescriptionEnValidationError = true;
                //    isValid = false;
                //}
                //if (HtmlParser.GetCleanedText(NewTimeLine.DescriptionAr).IsNullOrEmpty())
                //{
                //    DescriptionArValidationError = true;
                //    isValid = false;
                //}
                if (TimeLineImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;

                if (!TimeLineImage.IsNullOrEmpty() && !TimeLineImageContent.IsNullOrEmpty())
                {
                    await TimeLineContainer.SaveAsync(TimeLineImage, TimeLineImageContent);
                    NewTimeLine.Image = TimeLineImage;
                }
                var ent=   await TimeLinesAppService.CreateAsync(NewTimeLine);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);
             
               NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateTimeLineAsync()
        {
            try
            {
                CreateCalled = true;
                bool isValid = true;
                if (await EditingTimeLineValidations.ValidateAll() == false)
                {
                    isValid = false;
                }
                //if (HtmlParser.GetCleanedText(EditingTimeLine.DescriptionEn).IsNullOrEmpty())
                //{
                //    DescriptionEnValidationError = true;
                //    isValid = false;
                //}
                //if (HtmlParser.GetCleanedText(EditingTimeLine.DescriptionAr).IsNullOrEmpty())
                //{
                //    DescriptionArValidationError = true;
                //    isValid = false;
                //}
                if (TimeLineImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    isValid = false;
                }

                if (!isValid)
                    return;
    
                if (!TimeLineImage.IsNullOrEmpty() && !TimeLineImageContent.IsNullOrEmpty() && TimeLineImage != EditingTimeLine.Image)
                {
                    if (!EditingTimeLine.Image.IsNullOrEmpty())
                        await TimeLineContainer.DeleteAsync(EditingTimeLine.Image);
                    await TimeLineContainer.SaveAsync(TimeLineImage, TimeLineImageContent);
                    EditingTimeLine.Image = TimeLineImage;
                }
                else
                {
                    if (TimeLineImage.IsNullOrEmpty() && !EditingTimeLine.Image.IsNullOrEmpty())
                    {
                        await TimeLineContainer.DeleteAsync(EditingTimeLine.Image);
                        EditingTimeLine.Image = null;
                    }
                }
                await TimeLinesAppService.UpdateAsync(EditingTimeLineId, EditingTimeLine);
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
            NewTimeLine.DescriptionEn = value;
            //if (CreateCalled)
            //{
            //    if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
            //        DescriptionEnValidationError = false;
            //    else
            //        DescriptionEnValidationError = true;
            //}
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewTimeLine.DescriptionAr= value;
            //if (CreateCalled)
            //{
            //    if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
            //        DescriptionArValidationError = false;
            //    else
            //        DescriptionArValidationError = true;
            //}
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingTimeLine.DescriptionEn = value;
            //if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
            //    DescriptionEnValidationError = false;
            //else
            //    DescriptionEnValidationError = true;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingTimeLine.DescriptionAr = value;
            //if (!HtmlParser.GetCleanedText(value).IsNullOrEmpty())
            //    DescriptionArValidationError = false;
            //else
            //    DescriptionArValidationError = true;
        }
        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    TimeLineImageContent = await result.GetAllBytesAsync();
                    TimeLineImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    TimeLineImageNewUpload = true;

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
                    TimeLineImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private async Task RemoveMedia()
        {
          TimeLineImage = null;
        }
        async Task OnSelectedValueChanged(Guid value)
        {
            NewTimeLine.TimeLineCategoryId = value;
        }
        async Task OnSelectedEditValueChanged(Guid value)
        {
            EditingTimeLine.TimeLineCategoryId = value;
        }
        private async Task GetTimeLineCategoryCollectionLookupAsync(string? newValue = null)
        {

            TimeLineCategoriesCollection = (await TimeLinesAppService.GetTimeLineCategoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task SetNewAsync()
        {
            GetTimeLinesInput getTimeLinesInput = new GetTimeLinesInput();
            getTimeLinesInput.MaxResultCount = 1;
            PagedResultDto<TimeLineWithNavigationPropertiesDto> timeLines = (await TimeLinesAppService.GetListAsync(getTimeLinesInput));
            if (timeLines != null)
                NewTimeLine.Order = Convert.ToInt32(timeLines.TotalCount);
            if (!TimeLineCategoriesCollection.IsNullOrEmpty())
                NewTimeLine.TimeLineCategoryId = TimeLineCategoriesCollection.FirstOrDefault().Id;
            NewTimeLine.TimeLineDate = DateTime.Now;


        }

    }
}
