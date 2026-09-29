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

namespace DIP.Blazor.Pages
{
    public partial class AmenityParagraphs
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<AmenityParagraphWithNavigationPropertiesDto> AmenityParagraphList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateAmenityParagraph { get; set; }
        private bool CanEditAmenityParagraph { get; set; }
        private bool CanDeleteAmenityParagraph { get; set; }
        private AmenityParagraphCreateDto NewAmenityParagraph { get; set; }
        private Validations NewAmenityParagraphValidations { get; set; } = new();
        private AmenityParagraphUpdateDto EditingAmenityParagraph { get; set; }
        private Validations EditingAmenityParagraphValidations { get; set; } = new();
        private Guid EditingAmenityParagraphId { get; set; }
        private Modal CreateAmenityParagraphModal { get; set; } = new();
        private Modal EditAmenityParagraphModal { get; set; } = new();
        private GetAmenityParagraphsInput Filter { get; set; }
        private DataGridEntityActionsColumn<AmenityParagraphWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "amenityParagraph-create-tab";
        protected string SelectedEditTab = "amenityParagraph-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> AmenitiesCollection { get; set; } = new List<LookupDto<Guid>>();

        public AmenityParagraphs()
        {
            NewAmenityParagraph = new AmenityParagraphCreateDto();
            EditingAmenityParagraph = new AmenityParagraphUpdateDto();
            Filter = new GetAmenityParagraphsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            AmenityParagraphList = new List<AmenityParagraphWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:AmenityParagraphs"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewAmenityParagraph"], async () =>
            {
                await OpenCreateAmenityParagraphModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.AmenityParagraphs.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAmenityParagraph = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.AmenityParagraphs.Create);
            CanEditAmenityParagraph = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.AmenityParagraphs.Edit);
            CanDeleteAmenityParagraph = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.AmenityParagraphs.Delete);
        }

        private async Task GetAmenityParagraphsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AmenityParagraphsAppService.GetListAsync(Filter);
            AmenityParagraphList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAmenityParagraphsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await AmenityParagraphsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/amenity-paragraphs/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AmenityParagraphWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetAmenityParagraphsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateAmenityParagraphModalAsync()
        {
            NewAmenityParagraph = new AmenityParagraphCreateDto{
                
                
            };
            await NewAmenityParagraphValidations.ClearAll();
            await CreateAmenityParagraphModal.Show();
        }

        private async Task CloseCreateAmenityParagraphModalAsync()
        {
            NewAmenityParagraph = new AmenityParagraphCreateDto{
                
                
            };
            await CreateAmenityParagraphModal.Hide();
        }

        private async Task OpenEditAmenityParagraphModalAsync(AmenityParagraphWithNavigationPropertiesDto input)
        {
            var amenityParagraph = await AmenityParagraphsAppService.GetWithNavigationPropertiesAsync(input.AmenityParagraph.Id);
            
            EditingAmenityParagraphId = amenityParagraph.AmenityParagraph.Id;
            EditingAmenityParagraph = ObjectMapper.Map<AmenityParagraphDto, AmenityParagraphUpdateDto>(amenityParagraph.AmenityParagraph);
            await EditingAmenityParagraphValidations.ClearAll();
            await EditAmenityParagraphModal.Show();
        }

        private async Task DeleteAmenityParagraphAsync(AmenityParagraphWithNavigationPropertiesDto input)
        {
            await AmenityParagraphsAppService.DeleteAsync(input.AmenityParagraph.Id);
            await GetAmenityParagraphsAsync();
        }

        private async Task CreateAmenityParagraphAsync()
        {
            try
            {
                if (await NewAmenityParagraphValidations.ValidateAll() == false)
                {
                    return;
                }

                await AmenityParagraphsAppService.CreateAsync(NewAmenityParagraph);
                await GetAmenityParagraphsAsync();
                await CloseCreateAmenityParagraphModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditAmenityParagraphModalAsync()
        {
            await EditAmenityParagraphModal.Hide();
        }

        private async Task UpdateAmenityParagraphAsync()
        {
            try
            {
                if (await EditingAmenityParagraphValidations.ValidateAll() == false)
                {
                    return;
                }

                await AmenityParagraphsAppService.UpdateAsync(EditingAmenityParagraphId, EditingAmenityParagraph);
                await GetAmenityParagraphsAsync();
                await EditAmenityParagraphModal.Hide();                
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
        

        private async Task GetAmenityCollectionLookupAsync(string? newValue = null)
        {
            AmenitiesCollection = (await AmenityParagraphsAppService.GetAmenityLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
