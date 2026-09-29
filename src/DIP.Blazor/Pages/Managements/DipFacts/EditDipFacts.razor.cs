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
using DIP.DipFacts;
using DIP.Permissions;
using DIP.Shared;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using DIP.DipFacts;
using Volo.Abp.BlobStoring;
using Volo.Abp.Guids;
using System.IO;
using Volo.Abp;

namespace DIP.Blazor.Pages.Managements.DipFacts
{
    public partial class EditDipFacts
    {

        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }



        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<DipFactDto> DipFactList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateDipFact { get; set; }
        private bool CanEditDipFact { get; set; }
        private bool CanDeleteDipFact { get; set; }
        private DipFactCreateDto NewDipFact { get; set; }
        private Validations NewDipFactValidations { get; set; } = new();
        private DipFactUpdateDto EditingDipFact { get; set; }
        private Validations EditingDipFactValidations { get; set; } = new();
        private Guid EditingDipFactId { get; set; }
        private Modal CreateDipFactModal { get; set; } = new();
        private Modal EditDipFactModal { get; set; } = new();
        private GetDipFactsInput Filter { get; set; }
        private DataGridEntityActionsColumn<DipFactDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
      
        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        [Inject]
        public IBlobContainer<DipFactContainer> DipFactContainer { get; set; }

        public string DipFactImage { get; set; } = "";
        public byte[] DipFactImageContent { get; set; }
        public bool DipFactImageNewUpload { get; set; } = false;

        private bool ImageValidationError { get; set; } = false;

        public EditDipFacts()
        {
            NewDipFact = new DipFactCreateDto();
            EditingDipFact = new DipFactUpdateDto();
            Filter = new GetDipFactsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            DipFactList = new List<DipFactDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingDipFactId = Guid.Parse(Id);
                    var dipFact = await DipFactsAppService.GetAsync(EditingDipFactId);
                    EditingDipFact = ObjectMapper.Map<DipFactDto, DipFactUpdateDto>(dipFact);
                    if (EditingDipFact != null)
                    {
                        DipFactImage = EditingDipFact?.Image;
                    }
                    await EditingDipFactValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:DipFacts"]));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        //protected virtual ValueTask SetToolbarItemsAsync()
        //{
        //    Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
        //    Toolbar.AddButton(L["NewDipFact"], async () =>
        //    {
        //        await OpenCreateDipFactModalAsync();
        //    }, IconName.Add, requiredPolicyName: DIPPermissions.DipFacts.Create);

        //    return ValueTask.CompletedTask;
        //}

        private async Task SetPermissionsAsync()
        {
            CanCreateDipFact = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.DipFacts.Create);
            CanEditDipFact = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipFacts.Edit);
            CanDeleteDipFact = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.DipFacts.Delete);
        }

        private async Task GetDipFactsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await DipFactsAppService.GetListAsync(Filter);
            DipFactList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetDipFactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await DipFactsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/dip-facts/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DipFactDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetDipFactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateDipFactModalAsync()
        {
            NewDipFact = new DipFactCreateDto{
                
                
            };
            await NewDipFactValidations.ClearAll();
            await CreateDipFactModal.Show();
        }

        private async Task CloseCreateDipFactModalAsync()
        {
            NewDipFact = new DipFactCreateDto{
                
                
            };
            await CreateDipFactModal.Hide();
        }

        private async Task OpenEditDipFactModalAsync(DipFactDto input)
        {
            var dipFact = await DipFactsAppService.GetAsync(input.Id);
            
            EditingDipFactId = dipFact.Id;
            EditingDipFact = ObjectMapper.Map<DipFactDto, DipFactUpdateDto>(dipFact);
            await EditingDipFactValidations.ClearAll();
            await EditDipFactModal.Show();
        }

     

        private async Task CreateDipFactAsync()
        {
            try
            {
                if (await NewDipFactValidations.ValidateAll() == false)
                {
                    return;
                }
                if (DipFactImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    return;
                }
                if (!DipFactImage.IsNullOrEmpty() && !DipFactImageContent.IsNullOrEmpty())
                {
                    await DipFactContainer.SaveAsync(DipFactImage, DipFactImageContent);
                    NewDipFact.Image = DipFactImage;
                }
                var ent = await DipFactsAppService.CreateAsync(NewDipFact);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditDipFactModalAsync()
        {
            await EditDipFactModal.Hide();
        }

        private async Task UpdateDipFactAsync()
        {
            try
            {
                if (await EditingDipFactValidations.ValidateAll() == false)
                {
                    return;
                }
                if (DipFactImage.IsNullOrEmpty())
                {
                    ImageValidationError = true;
                    return;
                }
                if (!DipFactImage.IsNullOrEmpty() && !DipFactImageContent.IsNullOrEmpty() && DipFactImage != EditingDipFact.Image)
                {
                    if (!EditingDipFact.Image.IsNullOrEmpty())
                        await DipFactContainer.DeleteAsync(EditingDipFact.Image);
                    await DipFactContainer.SaveAsync(DipFactImage, DipFactImageContent);
                    EditingDipFact.Image = DipFactImage;
                }
                else
                {
                    if (DipFactImage.IsNullOrEmpty() && !EditingDipFact.Image.IsNullOrEmpty())
                    {
                        await DipFactContainer.DeleteAsync(EditingDipFact.Image);
                        EditingDipFact.Image = null;
                    }
                }
                await DipFactsAppService.UpdateAsync(EditingDipFactId, EditingDipFact);
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
            NewDipFact.DescriptionEn = value;
        }
        public async Task CreatingDescriptionArOnContentChanged(string value)
        {
            NewDipFact.DescriptionAr = value;
        }
        public async Task EditingDescriptionEnOnContentChanged(string value)
        {
            EditingDipFact.DescriptionEn = value;
        }
        public async Task EditingDescriptionArOnContentChanged(string value)
        {
            EditingDipFact.DescriptionAr = value;
        }

        public async Task OnImageUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    DipFactImageContent = await result.GetAllBytesAsync();
                  //  DipFactImage = $"{Path.GetFileNameWithoutExtension(e.File.Name)}_{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    DipFactImage = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    DipFactImageNewUpload = true;
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
                    DipFactImage = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task RemoveMedia()
        {
                DipFactImage = null;
        }

        private async Task SetNewAsync()
        {
            GetDipFactsInput getDipFactsInput = new GetDipFactsInput();
            getDipFactsInput.MaxResultCount = 1;
            PagedResultDto<DipFactDto> dipFacts = (await DipFactsAppService.GetListAsync(getDipFactsInput));
            if (dipFacts != null)
                NewDipFact.Order = Convert.ToInt32(dipFacts.TotalCount);
            else
                NewDipFact.Order = 0;
            NewDipFact.IsActive = true;

        }
    }
}
