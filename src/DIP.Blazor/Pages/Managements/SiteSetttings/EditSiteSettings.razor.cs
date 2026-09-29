using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.AspNetCore.Components;
using DIP.SiteSettings;
using DIP.Permissions;

namespace DIP.Blazor.Pages.Managements.SiteSetttings
{
    public partial class EditSiteSettings
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();

        [Inject]
        public IUiMessageService uiMessageService { get; set; }
        private int TotalCount { get; set; }
        private bool CanCreateSiteSetting { get; set; }
        private bool CanEditSiteSetting { get; set; }
        private bool CanDeleteSiteSetting { get; set; }
        private SiteSettingUpdateDto EditingSiteSetting { get; set; }
        private Validations EditingSiteSettingValidations { get; set; } = new();
        private Guid EditingSiteSettingId { get; set; }
        private GetSiteSettingsInput Filter { get; set; }
        private DataGridEntityActionsColumn<SiteSettingDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedTab = "setting-tab-english";

        public EditSiteSettings()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
            GetSiteSettingsInput getSiteSettingsInput = new GetSiteSettingsInput();
            getSiteSettingsInput.SkipCount = 0;
            getSiteSettingsInput.MaxResultCount = 1;

            List<SiteSettingDto> siteSettingDtos = (await SiteSettingsAppService.GetListAsync(getSiteSettingsInput)).Items.ToList();
            if (siteSettingDtos.IsNullOrEmpty())
            {
                SiteSettingCreateDto siteSettingCreateDto = new SiteSettingCreateDto();
                await SiteSettingsAppService.CreateAsync(siteSettingCreateDto);
                siteSettingDtos = (await SiteSettingsAppService.GetListAsync(getSiteSettingsInput)).Items.ToList();
            }
           

            SiteSettingDto siteSettingDto = siteSettingDtos.FirstOrDefault();
            EditingSiteSettingId = siteSettingDto.Id;
            EditingSiteSetting = ObjectMapper.Map<SiteSettingDto, SiteSettingUpdateDto>(siteSettingDto);
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:SiteSettings"]));
            return ValueTask.CompletedTask;
        }



        private async Task SetPermissionsAsync()
        {
            CanCreateSiteSetting = await AuthorizationService
                .IsGrantedAsync(DIPPermissions.SiteSettings.Create);
            CanEditSiteSetting = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SiteSettings.Edit);
            CanDeleteSiteSetting = await AuthorizationService
                            .IsGrantedAsync(DIPPermissions.SiteSettings.Delete);
        }


        private async Task UpdateSiteSettingAsync()
        {
            try
            {
                if (await EditingSiteSettingValidations.ValidateAll() == false)
                {
                    return;
                }

                await SiteSettingsAppService.UpdateAsync(EditingSiteSettingId, EditingSiteSetting);
                await GetNewToUpdate();
                await uiMessageService.Success(L["SuccessfullyUpdated"]);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetNewToUpdate()
        {
            GetSiteSettingsInput getSiteSettingsInput = new GetSiteSettingsInput();
            getSiteSettingsInput.SkipCount = 0;
            getSiteSettingsInput.MaxResultCount = 1;
            List<SiteSettingDto> siteSettingDtos = (await SiteSettingsAppService.GetListAsync(getSiteSettingsInput)).Items.ToList();
            SiteSettingDto siteSettingDto = siteSettingDtos.FirstOrDefault();
            EditingSiteSettingId = siteSettingDto.Id;
            EditingSiteSetting = ObjectMapper.Map<SiteSettingDto, SiteSettingUpdateDto>(siteSettingDto);
        }
        private void OnSelectedTabChanged(string name)
        {
            SelectedTab = name;
        }
        private async Task Cancel()
        {
            NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");

        }


    }
}
