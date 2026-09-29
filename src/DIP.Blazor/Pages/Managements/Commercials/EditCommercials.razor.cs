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
using DIP.Commercials;
using DIP.Permissions;
using DIP.Shared;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using DIP.SliderHomePages;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using Blazorise.Extensions;
using DIP.Helper;
using DIP.MakaniNumber;

namespace DIP.Blazor.Pages.Managements.Commercials
{
    public partial class EditCommercials
    {


        [Parameter]
        public string Lang { get; set; }
        [Parameter]
        public string Id { get; set; }


        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<CommercialWithNavigationPropertiesDto> CommercialList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCommercial { get; set; }
        private bool CanEditCommercial { get; set; }
        private bool CanDeleteCommercial { get; set; }
        private CommercialCreateDto NewCommercial { get; set; }
        private Validations NewCommercialValidations { get; set; } = new();
        private CommercialUpdateDto EditingCommercial { get; set; }
        private Validations EditingCommercialValidations { get; set; } = new();
        private Guid EditingCommercialId { get; set; }
        private Modal CreateCommercialModal { get; set; } = new();
        private Modal EditCommercialModal { get; set; } = new();
        private GetCommercialsInput Filter { get; set; }
        private DataGridEntityActionsColumn<CommercialWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "create-tab-english";
        protected string SelectedEditTab = "edit-tab-english";
        private IReadOnlyList<LookupDto<Guid>> SubCategoriesCollection { get; set; } = new List<LookupDto<Guid>>();

        [Inject]
        public IMakaniNumberAppService MakaniNumberAppService { get; set; }
        private bool MakaniApiError { get; set; } = false;
        private bool PlotEmpty { get; set; } = false;
        private bool DisableGetMakaninButton { get; set; } = false;

        public EditCommercials()
        {
            NewCommercial = new CommercialCreateDto();
            EditingCommercial = new CommercialUpdateDto();
            Filter = new GetCommercialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CommercialList = new List<CommercialWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetSubCategoryCollectionLookupAsync();


            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EditingCommercialId = Guid.Parse(Id);
                    var commercial = await CommercialsAppService.GetAsync(EditingCommercialId);
                    EditingCommercial = ObjectMapper.Map<CommercialDto, CommercialUpdateDto>(commercial);

                    await EditingCommercialValidations.ClearAll();
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
            {
                if (!SubCategoriesCollection.IsNullOrEmpty())
                    NewCommercial.SubCategoryId = SubCategoriesCollection.FirstOrDefault().Id;
            }


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Commercials"],
                 url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
            if (Id.IsNullOrEmpty())
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Create"]));
            else
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Edit"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCommercial"], async () =>
            {
                await OpenCreateCommercialModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.Commercials.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCommercial = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.Commercials.Create);
            CanEditCommercial = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Commercials.Edit);
            CanDeleteCommercial = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.Commercials.Delete);
        }

        private async Task GetCommercialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CommercialsAppService.GetListAsync(Filter);
            CommercialList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCommercialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await CommercialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/commercials/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CommercialWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCommercialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCommercialModalAsync()
        {
            NewCommercial = new CommercialCreateDto{
                
                SubCategoryId = SubCategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewCommercialValidations.ClearAll();
            await CreateCommercialModal.Show();
        }

        private async Task CloseCreateCommercialModalAsync()
        {
            NewCommercial = new CommercialCreateDto{
                
                SubCategoryId = SubCategoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateCommercialModal.Hide();
        }

        private async Task OpenEditCommercialModalAsync(CommercialWithNavigationPropertiesDto input)
        {
            var commercial = await CommercialsAppService.GetWithNavigationPropertiesAsync(input.Commercial.Id);
            
            EditingCommercialId = commercial.Commercial.Id;
            EditingCommercial = ObjectMapper.Map<CommercialDto, CommercialUpdateDto>(commercial.Commercial);
            await EditingCommercialValidations.ClearAll();
            await EditCommercialModal.Show();
        }

      

        private async Task CreateCommercialAsync()
        {
            try
            {
                if (await NewCommercialValidations.ValidateAll() == false)
                {
                    return;
                }

                await CommercialsAppService.CreateAsync(NewCommercial);
                await uiMessageService.Success(L["Message:SuccessfullyCreated"]);

                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCommercialModalAsync()
        {
            await EditCommercialModal.Hide();
        }

        private async Task UpdateCommercialAsync()
        {
            try
            {
                if (await EditingCommercialValidations.ValidateAll() == false)
                {
                    return;
                }

                await CommercialsAppService.UpdateAsync(EditingCommercialId, EditingCommercial);
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
        

        private async Task GetSubCategoryCollectionLookupAsync(string? newValue = null)
        {
            SubCategoriesCollection = (await CommercialsAppService.GetSubCategoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
        }

        private void OnPlotNoChanged(string value)
        {
            if (Id.IsNullOrEmpty())
            {
                NewCommercial.PlotNo = value;
                NewCommercial.MakaniNo = "";
            }
            else
            {
                EditingCommercial.PlotNo = value;
                EditingCommercial.MakaniNo = "";


            }
        }

        private async Task GetMakani()
        {
            DisableGetMakaninButton = true;
            string? plotNo = "";
            MakaniApiError = false;
            PlotEmpty = false;
            if (Id.IsNullOrEmpty())
                plotNo = NewCommercial?.PlotNo;
            else
                plotNo = EditingCommercial?.PlotNo;

            if (!plotNo.IsNullOrWhiteSpace())
            {
                MakaniNumberPropertyDto makaniNumberPropertyDto = await MakaniNumberAppService.GetMakaniNumberAsync(plotNo);
                if(makaniNumberPropertyDto != null && makaniNumberPropertyDto.MAKANI != null && makaniNumberPropertyDto.MAKANI.Count > 0 && !makaniNumberPropertyDto.MAKANI[0].Makani.IsNullOrWhiteSpace())
                {
                    if (Id.IsNullOrEmpty())
                        NewCommercial.MakaniNo = makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", "");
                    else
                        EditingCommercial.MakaniNo = makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", "");
                }
                else
                {
                    MakaniApiError = true;
                    if (Id.IsNullOrEmpty())
                         NewCommercial.MakaniNo ="";
                    else
                        EditingCommercial.MakaniNo = "";

                }
            }
            else
            {
                PlotEmpty = true;
                if (Id.IsNullOrEmpty())
                    NewCommercial.MakaniNo = "";
                else
                    EditingCommercial.MakaniNo = "";
            }

            DisableGetMakaninButton = false;
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
    }
}
