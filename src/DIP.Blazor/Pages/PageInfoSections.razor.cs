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
using DIP.PageInfoSections;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages
{
    public partial class PageInfoSections
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<PageInfoSectionWithNavigationPropertiesDto> PageInfoSectionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePageInfoSection { get; set; }
        private bool CanEditPageInfoSection { get; set; }
        private bool CanDeletePageInfoSection { get; set; }
        private PageInfoSectionCreateDto NewPageInfoSection { get; set; }
        private Validations NewPageInfoSectionValidations { get; set; } = new();
        private PageInfoSectionUpdateDto EditingPageInfoSection { get; set; }
        private Validations EditingPageInfoSectionValidations { get; set; } = new();
        private Guid EditingPageInfoSectionId { get; set; }
        private Modal CreatePageInfoSectionModal { get; set; } = new();
        private Modal EditPageInfoSectionModal { get; set; } = new();
        private GetPageInfoSectionsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PageInfoSectionWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "pageInfoSection-create-tab";
        protected string SelectedEditTab = "pageInfoSection-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> PageInfosCollection { get; set; } = new List<LookupDto<Guid>>();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public PageInfoSections()
        {
            NewPageInfoSection = new PageInfoSectionCreateDto();
            EditingPageInfoSection = new PageInfoSectionUpdateDto();
            Filter = new GetPageInfoSectionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PageInfoSectionList = new List<PageInfoSectionWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            await GetPageInfoCollectionLookupAsync();


        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:PageInfoSections"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPageInfoSection"], async () =>
            {
                await OpenCreatePageInfoSectionModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.PageInfoSections.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePageInfoSection = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.PageInfoSections.Create);
            CanEditPageInfoSection = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PageInfoSections.Edit);
            CanDeletePageInfoSection = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.PageInfoSections.Delete);
        }

        private async Task GetPageInfoSectionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PageInfoSectionsAppService.GetListAsync(Filter);
            PageInfoSectionList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPageInfoSectionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await PageInfoSectionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/page-info-sections/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PageInfoSectionWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPageInfoSectionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePageInfoSectionModalAsync()
        {
            NewPageInfoSection = new PageInfoSectionCreateDto{
                
                PageInfoId = PageInfosCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewPageInfoSectionValidations.ClearAll();
            await CreatePageInfoSectionModal.Show();
        }

        private async Task CloseCreatePageInfoSectionModalAsync()
        {
            NewPageInfoSection = new PageInfoSectionCreateDto{
                
                PageInfoId = PageInfosCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreatePageInfoSectionModal.Hide();
        }

        private async Task OpenEditPageInfoSectionModalAsync(PageInfoSectionWithNavigationPropertiesDto input)
        {
            var pageInfoSection = await PageInfoSectionsAppService.GetWithNavigationPropertiesAsync(input.PageInfoSection.Id);
            
            EditingPageInfoSectionId = pageInfoSection.PageInfoSection.Id;
            EditingPageInfoSection = ObjectMapper.Map<PageInfoSectionDto, PageInfoSectionUpdateDto>(pageInfoSection.PageInfoSection);
            await EditingPageInfoSectionValidations.ClearAll();
            await EditPageInfoSectionModal.Show();
        }

        private async Task DeletePageInfoSectionAsync(PageInfoSectionWithNavigationPropertiesDto input)
        {
            await PageInfoSectionsAppService.DeleteAsync(input.PageInfoSection.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetPageInfoSectionsAsync();
        }

        private async Task CreatePageInfoSectionAsync()
        {
            try
            {
                if (await NewPageInfoSectionValidations.ValidateAll() == false)
                {
                    return;
                }

                await PageInfoSectionsAppService.CreateAsync(NewPageInfoSection);
                await GetPageInfoSectionsAsync();
                await CloseCreatePageInfoSectionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPageInfoSectionModalAsync()
        {
            await EditPageInfoSectionModal.Hide();
        }

        private async Task UpdatePageInfoSectionAsync()
        {
            try
            {
                if (await EditingPageInfoSectionValidations.ValidateAll() == false)
                {
                    return;
                }

                await PageInfoSectionsAppService.UpdateAsync(EditingPageInfoSectionId, EditingPageInfoSection);
                await GetPageInfoSectionsAsync();
                await EditPageInfoSectionModal.Hide();                
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
        

        private async Task GetPageInfoCollectionLookupAsync(string? newValue = null)
        {
            PageInfosCollection = (await PageInfoSectionsAppService.GetPageInfoLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
