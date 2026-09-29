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
using DIP.InquiryForms;
using DIP.Permissions;
using DIP.Shared;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;

namespace DIP.Blazor.Pages.Managements.InquiryForms
{
    public partial class InquiryForms
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private IReadOnlyList<InquiryFormDto> InquiryFormList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateInquiryForm { get; set; }
        private bool CanEditInquiryForm { get; set; }
        private bool CanDeleteInquiryForm { get; set; }
        private GetInquiryFormsInput Filter { get; set; }
        private DataGridEntityActionsColumn<InquiryFormDto> EntityActionsColumn { get; set; } = new();
        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        public InquiryForms()
        {
            Filter = new GetInquiryFormsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            InquiryFormList = new List<InquiryFormDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:InquiryForms"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

            //Toolbar.AddButton(L["NewInquiryForm"], async () =>
            //{
            //    await OpenCreateInquiryFormModalAsync();
            //}, IconName.Add, requiredPolicyName: DIPPermissions.InquiryForms.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateInquiryForm = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.InquiryForms.Create);
            CanEditInquiryForm = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.InquiryForms.Edit);
            CanDeleteInquiryForm = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.InquiryForms.Delete);
        }

        private async Task GetInquiryFormsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await InquiryFormsAppService.GetListAsync(Filter);
            InquiryFormList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetInquiryFormsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await InquiryFormsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/inquiry-forms/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InquiryFormDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetInquiryFormsAsync();
            await InvokeAsync(StateHasChanged);
        }

  

        protected Task OpenDetailsInquiryFormPageAsync(InquiryFormDto input)
        {
            NavigationManager.NavigateTo($"/admin/inquiry-forms/details/{input.Id}");
            return Task.CompletedTask;
        }
        private async Task DeleteInquiryFormAsync(InquiryFormDto input)
        {
            await InquiryFormsAppService.DeleteAsync(input.Id);
            await uiMessageService.Success(L["Message:SuccessfullyDeleted"]);

            await GetInquiryFormsAsync();
        }

    }
}
