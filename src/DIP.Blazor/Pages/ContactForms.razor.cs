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
using DIP.ContactForms;
using DIP.Permissions;
using DIP.Shared;

namespace DIP.Blazor.Pages
{
    public partial class ContactForms
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<ContactFormDto> ContactFormList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateContactForm { get; set; }
        private bool CanEditContactForm { get; set; }
        private bool CanDeleteContactForm { get; set; }
        private ContactFormCreateDto NewContactForm { get; set; }
        private Validations NewContactFormValidations { get; set; } = new();
        private ContactFormUpdateDto EditingContactForm { get; set; }
        private Validations EditingContactFormValidations { get; set; } = new();
        private Guid EditingContactFormId { get; set; }
        private Modal CreateContactFormModal { get; set; } = new();
        private Modal EditContactFormModal { get; set; } = new();
        private GetContactFormsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ContactFormDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "contactForm-create-tab";
        protected string SelectedEditTab = "contactForm-edit-tab";
        
        public ContactForms()
        {
            NewContactForm = new ContactFormCreateDto();
            EditingContactForm = new ContactFormUpdateDto();
            Filter = new GetContactFormsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ContactFormList = new List<ContactFormDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ContactForms"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewContactForm"], async () =>
            {
                await OpenCreateContactFormModalAsync();
            }, IconName.Add, requiredPolicyName: DIPPermissions.ContactForms.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateContactForm = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.ContactForms.Create);
            CanEditContactForm = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.ContactForms.Edit);
            CanDeleteContactForm = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.ContactForms.Delete);
        }

        private async Task GetContactFormsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ContactFormsAppService.GetListAsync(Filter);
            ContactFormList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetContactFormsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await ContactFormsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("DIP") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/contact-forms/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ContactFormDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetContactFormsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateContactFormModalAsync()
        {
            NewContactForm = new ContactFormCreateDto{
                
                
            };
            await NewContactFormValidations.ClearAll();
            await CreateContactFormModal.Show();
        }

        private async Task CloseCreateContactFormModalAsync()
        {
            NewContactForm = new ContactFormCreateDto{
                
                
            };
            await CreateContactFormModal.Hide();
        }

        private async Task OpenEditContactFormModalAsync(ContactFormDto input)
        {
            var contactForm = await ContactFormsAppService.GetAsync(input.Id);
            
            EditingContactFormId = contactForm.Id;
            EditingContactForm = ObjectMapper.Map<ContactFormDto, ContactFormUpdateDto>(contactForm);
            await EditingContactFormValidations.ClearAll();
            await EditContactFormModal.Show();
        }

        private async Task DeleteContactFormAsync(ContactFormDto input)
        {
            await ContactFormsAppService.DeleteAsync(input.Id);
            await GetContactFormsAsync();
        }

        private async Task CreateContactFormAsync()
        {
            try
            {
                if (await NewContactFormValidations.ValidateAll() == false)
                {
                    return;
                }

                await ContactFormsAppService.CreateAsync(NewContactForm);
                await GetContactFormsAsync();
                await CloseCreateContactFormModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditContactFormModalAsync()
        {
            await EditContactFormModal.Hide();
        }

        private async Task UpdateContactFormAsync()
        {
            try
            {
                if (await EditingContactFormValidations.ValidateAll() == false)
                {
                    return;
                }

                await ContactFormsAppService.UpdateAsync(EditingContactFormId, EditingContactForm);
                await GetContactFormsAsync();
                await EditContactFormModal.Hide();                
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
