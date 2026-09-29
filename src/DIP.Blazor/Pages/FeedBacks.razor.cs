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
using DIP.FeedBacks;
using DIP.Permissions;
using DIP.Shared;

namespace DIP.Blazor.Pages
{
    public partial class FeedBacks
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<FeedBackDto> FeedBackList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateFeedBack { get; set; }
        private bool CanEditFeedBack { get; set; }
        private bool CanDeleteFeedBack { get; set; }
        private FeedBackCreateDto NewFeedBack { get; set; }
        private Validations NewFeedBackValidations { get; set; } = new();
        private FeedBackUpdateDto EditingFeedBack { get; set; }
        private Validations EditingFeedBackValidations { get; set; } = new();
        private Guid EditingFeedBackId { get; set; }
        private Modal CreateFeedBackModal { get; set; } = new();
        private Modal EditFeedBackModal { get; set; } = new();
        private GetFeedBacksInput Filter { get; set; }
        private DataGridEntityActionsColumn<FeedBackDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "feedBack-create-tab";
        protected string SelectedEditTab = "feedBack-edit-tab";
        
        public FeedBacks()
        {
            NewFeedBack = new FeedBackCreateDto();
            EditingFeedBack = new FeedBackUpdateDto();
            Filter = new GetFeedBacksInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            FeedBackList = new List<FeedBackDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:FeedBacks"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewFeedBack"], async () =>
            {
                await OpenCreateFeedBackModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.FeedBacks.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateFeedBack = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.FeedBacks.Create);
            CanEditFeedBack = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.FeedBacks.Edit);
            CanDeleteFeedBack = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.FeedBacks.Delete);
        }

        private async Task GetFeedBacksAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await FeedBacksAppService.GetListAsync(Filter);
            FeedBackList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetFeedBacksAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await FeedBacksAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/feed-backs/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<FeedBackDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetFeedBacksAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateFeedBackModalAsync()
        {
            NewFeedBack = new FeedBackCreateDto{
                
                
            };
            await NewFeedBackValidations.ClearAll();
            await CreateFeedBackModal.Show();
        }

        private async Task CloseCreateFeedBackModalAsync()
        {
            NewFeedBack = new FeedBackCreateDto{
                
                
            };
            await CreateFeedBackModal.Hide();
        }

        private async Task OpenEditFeedBackModalAsync(FeedBackDto input)
        {
            var feedBack = await FeedBacksAppService.GetAsync(input.Id);
            
            EditingFeedBackId = feedBack.Id;
            EditingFeedBack = ObjectMapper.Map<FeedBackDto, FeedBackUpdateDto>(feedBack);
            await EditingFeedBackValidations.ClearAll();
            await EditFeedBackModal.Show();
        }

        private async Task DeleteFeedBackAsync(FeedBackDto input)
        {
            await FeedBacksAppService.DeleteAsync(input.Id);
            await GetFeedBacksAsync();
        }

        private async Task CreateFeedBackAsync()
        {
            try
            {
                if (await NewFeedBackValidations.ValidateAll() == false)
                {
                    return;
                }

                await FeedBacksAppService.CreateAsync(NewFeedBack);
                await GetFeedBacksAsync();
                await CloseCreateFeedBackModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditFeedBackModalAsync()
        {
            await EditFeedBackModal.Hide();
        }

        private async Task UpdateFeedBackAsync()
        {
            try
            {
                if (await EditingFeedBackValidations.ValidateAll() == false)
                {
                    return;
                }

                await FeedBacksAppService.UpdateAsync(EditingFeedBackId, EditingFeedBack);
                await GetFeedBacksAsync();
                await EditFeedBackModal.Hide();                
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
        

    }
}
