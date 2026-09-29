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
using DIP.EFormServiceSubCategories;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;
using Microsoft.Extensions.Logging;
using Volo.Abp.BlobStoring;
using DIP.TimeLines;
using System.IO;
using Volo.Abp;
using Volo.Abp.Guids;
using Blazorise.Extensions;

namespace DIP.Blazor.Pages.Managements.EFormServiceSubCategories
{
    public partial class EditEFormServiceSubCategories
    {


        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }


        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private IReadOnlyList<EFormServiceSubCategoryWithNavigationPropertiesDto> EFormServiceSubCategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEFormServiceSubCategory { get; set; }
        private bool CanEditEFormServiceSubCategory { get; set; }
        private bool CanDeleteEFormServiceSubCategory { get; set; }
        private EFormServiceSubCategoryCreateDto NewEFormServiceSubCategory { get; set; }
        private Validations NewEFormServiceSubCategoryValidations { get; set; } = new();
        private EFormServiceSubCategoryUpdateDto EditingEFormServiceSubCategory { get; set; }
        private Validations EditingEFormServiceSubCategoryValidations { get; set; } = new();
        private Guid EditingEFormServiceSubCategoryId { get; set; }
        private Modal CreateEFormServiceSubCategoryModal { get; set; } = new();
        private Modal EditEFormServiceSubCategoryModal { get; set; } = new();
        private GetEFormServiceSubCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<EFormServiceSubCategoryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private IReadOnlyList<LookupDto<Guid>> EFormServicesCollection { get; set; } = new List<LookupDto<Guid>>();

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }

        [Inject]
        public IBlobContainer<EFormServiceSubCategoryContainer> EFormServiceSubCategoryContainer { get; set; }
        public string FormFile { get; set; } = "";
        public byte[] FormFileContent { get; set; }
        public bool FormFileNewUpload { get; set; } = false;
     
        public EditEFormServiceSubCategories()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto();
            EditingEFormServiceSubCategory = new EFormServiceSubCategoryUpdateDto();
            Filter = new GetEFormServiceSubCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EFormServiceSubCategoryList = new List<EFormServiceSubCategoryWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetEFormServiceCollectionLookupAsync();
            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingEFormServiceSubCategoryId = Guid.Parse(Id);
                    var eFormServiceSubCategory = await EFormServiceSubCategoriesAppService.GetAsync(EditingEFormServiceSubCategoryId);
                    EditingEFormServiceSubCategory = ObjectMapper.Map<EFormServiceSubCategoryDto, EFormServiceSubCategoryUpdateDto>(eFormServiceSubCategory);
                    if (EditingEFormServiceSubCategory != null)
                    {
                        FormFile = EditingEFormServiceSubCategory?.File;
                    }
                    await EditingEFormServiceSubCategoryValidations.ClearAll();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:EFormServiceSubCategories"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

            Toolbar.AddButton(L["NewEFormServiceSubCategory"], async () =>
            {
                await OpenCreateEFormServiceSubCategoryModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.EFormServiceSubCategories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEFormServiceSubCategory = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Create);
            CanEditEFormServiceSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Edit);
            CanDeleteEFormServiceSubCategory = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.EFormServiceSubCategories.Delete);
        }

        private async Task GetEFormServiceSubCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EFormServiceSubCategoriesAppService.GetListAsync(Filter);
            EFormServiceSubCategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEFormServiceSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await EFormServiceSubCategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/e-form-service-sub-categories/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EFormServiceSubCategoryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEFormServiceSubCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEFormServiceSubCategoryModalAsync()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto {


            };
            await NewEFormServiceSubCategoryValidations.ClearAll();
            await CreateEFormServiceSubCategoryModal.Show();
        }

        private async Task CloseCreateEFormServiceSubCategoryModalAsync()
        {
            NewEFormServiceSubCategory = new EFormServiceSubCategoryCreateDto {


            };
            await CreateEFormServiceSubCategoryModal.Hide();
        }

        private async Task OpenEditEFormServiceSubCategoryModalAsync(EFormServiceSubCategoryWithNavigationPropertiesDto input)
        {
            var eFormServiceSubCategory = await EFormServiceSubCategoriesAppService.GetWithNavigationPropertiesAsync(input.EFormServiceSubCategory.Id);

            EditingEFormServiceSubCategoryId = eFormServiceSubCategory.EFormServiceSubCategory.Id;
            EditingEFormServiceSubCategory = ObjectMapper.Map<EFormServiceSubCategoryDto, EFormServiceSubCategoryUpdateDto>(eFormServiceSubCategory.EFormServiceSubCategory);
            await EditingEFormServiceSubCategoryValidations.ClearAll();
            await EditEFormServiceSubCategoryModal.Show();
        }

      

        private async Task CreateEFormServiceSubCategoryAsync()
        {
            try
            {
                if (await NewEFormServiceSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!FormFile.IsNullOrEmpty() && !FormFileContent.IsNullOrEmpty())
                {
                    await EFormServiceSubCategoryContainer.SaveAsync(FormFile, FormFileContent);
                    NewEFormServiceSubCategory.File = FormFile;
                }
                await EFormServiceSubCategoriesAppService.CreateAsync(NewEFormServiceSubCategory);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEFormServiceSubCategoryModalAsync()
        {
            await EditEFormServiceSubCategoryModal.Hide();
        }

        private async Task UpdateEFormServiceSubCategoryAsync()
        {
            try
            {
                if (await EditingEFormServiceSubCategoryValidations.ValidateAll() == false)
                {
                    return;
                }
                if (!FormFile.IsNullOrEmpty() && !FormFileContent.IsNullOrEmpty() && FormFile != EditingEFormServiceSubCategory.File)
                {
                    if (!EditingEFormServiceSubCategory.File.IsNullOrEmpty())
                        await EFormServiceSubCategoryContainer.DeleteAsync(EditingEFormServiceSubCategory.File);
                    await EFormServiceSubCategoryContainer.SaveAsync(FormFile, FormFileContent);
                    EditingEFormServiceSubCategory.File = FormFile;
                }
                else
                {
                    if (FormFile.IsNullOrEmpty() && !EditingEFormServiceSubCategory.File.IsNullOrEmpty())
                    {
                        await EFormServiceSubCategoryContainer.DeleteAsync(EditingEFormServiceSubCategory.File);
                        EditingEFormServiceSubCategory.File = null;
                    }
                }
                await EFormServiceSubCategoriesAppService.UpdateAsync(EditingEFormServiceSubCategoryId, EditingEFormServiceSubCategory);
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


        private async Task GetEFormServiceCollectionLookupAsync(string? newValue = null)
        {
            EFormServicesCollection = (await EFormServiceSubCategoriesAppService.GetEFormServiceLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }

        public async Task OnFileUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    FormFileContent = await result.GetAllBytesAsync();
                    FormFile = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    FormFileNewUpload = true;
                }
            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        public async Task FileChanged(FileChangedEventArgs e)
        {
            try
            {
                if (e.Files.Count() == 0)
                {
                    FormFile = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private async Task RemoveMedia()
        {
            FormFile = null;
        }

        private async Task SetNewAsync()
        {
            GetEFormServiceSubCategoriesInput getEFormServiceSubCategoriesInput = new GetEFormServiceSubCategoriesInput();
            getEFormServiceSubCategoriesInput.MaxResultCount = 1;
            PagedResultDto<EFormServiceSubCategoryWithNavigationPropertiesDto> pagedResultDto = (await EFormServiceSubCategoriesAppService.GetListAsync(getEFormServiceSubCategoriesInput));
            if (pagedResultDto != null)
                NewEFormServiceSubCategory.Order = Convert.ToInt32(pagedResultDto.TotalCount) + 1;
            if (!EFormServicesCollection.IsNullOrEmpty())
                NewEFormServiceSubCategory.EFormServiceId = EFormServicesCollection.FirstOrDefault().Id;


        }
    } 
 
}
