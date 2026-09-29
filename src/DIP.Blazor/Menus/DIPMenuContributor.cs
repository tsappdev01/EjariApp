using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using DIP.Localization;
using DIP.Permissions;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Saas.Host.Blazor.Navigation;

namespace DIP.Blazor.Menus;

public class DIPMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<DIPResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                DIPMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 1
            )
        );

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.HostDashboard,
                l["Menu:Dashboard"],
                "~/HostDashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(DIPPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(DIPPermissions.Dashboard.Tenant)
        );

        /* Example nested menu definition:

        context.Menu.AddItem(
            new ApplicationMenuItem("Menu0", "Menu Level 0")
            .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
            .AddItem(
                new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                    .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                        .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
            )
            .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
        );

        */

        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 4;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.SliderHomePages,
                l["Menu:SliderHomePages"],
                url: "/admin/slider-home-pages",
icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.SliderHomePages.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.DipFacts,
                l["Menu:DipFacts"],
                url: "/admin/dip-facts",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.DipFacts.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.EServices,
                l["Menu:EServices"],
                url: "/admin/e-services",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.EServices.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.EFormServices,
                l["Menu:EFormServices"],
                url: "/admin/e-form-services",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.EFormServices.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.EFormServiceSubCategories,
                l["Menu:EFormServiceSubCategories"],
                url: "/admin/e-form-service-sub-categories",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.EFormServiceSubCategories.Default)
        );
        context.Menu.AddItem(new ApplicationMenuItem("PageManagment", l["Menu:PageManagment"], icon: "fa fa-file-alt")
               .AddItem(new ApplicationMenuItem(
                                DIPMenus.PageInfos,
                                l["Menu:PageInfos"],
                                url: "/admin/page-infos",
                                icon: "fa fa-file-alt",
                                requiredPermissionName: DIPPermissions.PageInfos.Default))
               .AddItem(new ApplicationMenuItem(
                                DIPMenus.PageInfoSections,
                                l["Menu:PageInfoSections"],
                                url: "/admin/page-info-sections",
                                icon: "fa fa-file-alt",
                                requiredPermissionName: DIPPermissions.PageInfoSections.Default))
           );

        context.Menu.AddItem(new ApplicationMenuItem("ZoneManagment", l["Menu:ZoneManagment"], icon: "fa fa-file-alt")
                .AddItem(new ApplicationMenuItem(
                                DIPMenus.Zones,
                                l["Menu:Zones"],
                                url: "/admin/zones",
                                icon: "fa fa-file-alt",
                                requiredPermissionName: DIPPermissions.Zones.Default))
                .AddItem(new ApplicationMenuItem(
                                DIPMenus.ZoneParagraphs,
                                l["Menu:ZoneParagraphs"],
                                url: "/admin/zone-paragraphs",
                                icon: "fa fa-file-alt",
                                requiredPermissionName: DIPPermissions.ZoneParagraphs.Default))
            );

        context.Menu.AddItem(new ApplicationMenuItem("AmenityManagment", l["Menu:AmenityManagment"], icon: "fa fa-file-alt")
                .AddItem(new ApplicationMenuItem(
                        DIPMenus.Amenities,
                        l["Menu:Amenities"],
                        url: "/admin/amenities",
                        icon: "fa fa-file-alt",
                        requiredPermissionName: DIPPermissions.Amenities.Default))
                .AddItem(new ApplicationMenuItem(
                        DIPMenus.AmenityParagraphs,
                        l["Menu:AmenityParagraphs"],
                        url: "/admin/amenity-paragraphs",
                        icon: "fa fa-file-alt",
                        requiredPermissionName: DIPPermissions.AmenityParagraphs.Default))
            );

        context.Menu.AddItem(new ApplicationMenuItem("TimeLineManagment", l["Menu:TimeLineManagment"], icon: "fa fa-file-alt")
        .AddItem(new ApplicationMenuItem(
                DIPMenus.TimeLineCategories,
                l["Menu:TimeLineCategories"],
                url: "/admin/time-line-categories",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.TimeLineCategories.Default))
        .AddItem(new ApplicationMenuItem(
                DIPMenus.TimeLines,
                l["Menu:TimeLines"],
                url: "/admin/time-lines",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.TimeLines.Default))
            );

        context.Menu.AddItem(new ApplicationMenuItem("MediaManagment", l["Menu:MediaManagment"], icon: "fa fa-file-alt")
        .AddItem(new ApplicationMenuItem(
                DIPMenus.PressReleases,
                l["Menu:PressReleases"],
                url: "/admin/press-releases",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.PressReleases.Default))
        .AddItem(new ApplicationMenuItem(
                DIPMenus.LastEventss,
                l["Menu:LastEventss"],
                url: "/admin/last-events",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.LastEventss.Default))
        .AddItem(new ApplicationMenuItem(
                DIPMenus.MediaGalleries,
                l["Menu:MediaGalleries"],
                url: "/admin/media-galleries",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.MediaGalleries.Default))
            );

        context.Menu.AddItem(new ApplicationMenuItem("CategoriesManagment", l["Menu:CategoriesManagment"], icon: "fa fa-file-alt")
      .AddItem(new ApplicationMenuItem(
                DIPMenus.Categories,
                l["Menu:Categories"],
                url: "/admin/categories",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.Categories.Default))
      .AddItem(new ApplicationMenuItem(
                DIPMenus.SubCategories,
                l["Menu:SubCategories"],
                url: "/admin/sub-categories",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.SubCategories.Default))
      .AddItem(new ApplicationMenuItem(
                DIPMenus.Commercials,
                l["Menu:Commercials"],
                url: "/admin/commercials",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.Commercials.Default))
          );

        //context.Menu.AddItem(
        //    new ApplicationMenuItem(
        //        DIPMenus.Categories,
        //        l["Menu:Categories"],
        //        url: "/admin/categories",
        //        icon: "fa fa-file-alt",
        //        requiredPermissionName: DIPPermissions.Categories.Default)
        //);

        //context.Menu.AddItem(
        //    new ApplicationMenuItem(
        //        DIPMenus.SubCategories,
        //        l["Menu:SubCategories"],
        //        url: "/admin/sub-categories",
        //        icon: "fa fa-file-alt",
        //        requiredPermissionName: DIPPermissions.SubCategories.Default)
        //);

        //context.Menu.AddItem(
        //    new ApplicationMenuItem(
        //        DIPMenus.Commercials,
        //        l["Menu:Commercials"],
        //        url: "/admin/commercials",
        //        icon: "fa fa-file-alt",
        //        requiredPermissionName: DIPPermissions.Commercials.Default)
        //);
        context.Menu.AddItem(new ApplicationMenuItem("UserFormsManagment", l["Menu:UserFormsManagment"], icon: "fa fa-file-alt")
            .AddItem(new ApplicationMenuItem(
                            DIPMenus.InquiryForms,
                            l["Menu:InquiryForms"],
                            url: "/admin/inquiry-forms",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: DIPPermissions.InquiryForms.Default))
            .AddItem(new ApplicationMenuItem(
                            DIPMenus.ContactForms,
                            l["Menu:ContactForms"],
                            url: "/admin/contact-forms",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: DIPPermissions.ContactForms.Default))
            .AddItem(new ApplicationMenuItem(
                            DIPMenus.FeedBacks,
                            l["Menu:FeedBacks"],
                            url: "/admin/feed-backs",
                            icon: "fa fa-file-alt",
                            requiredPermissionName: DIPPermissions.FeedBacks.Default))
            );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.DipBranches,
                l["Menu:DipBranches"],
                url: "/admin/dip-branches",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.DipBranches.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.MajorIndustries,
                l["Menu:MajorIndustries"],
                url: "/admin/major-industries",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.MajorIndustries.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.SiteSettings,
                l["Menu:SiteSettings"],
                url: "/admin/site-settings",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.SiteSettings.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                DIPMenus.SupportedBanks,
                l["Menu:SupportedBanks"],
                url: "/admin/supported-banks",
                icon: "fa fa-file-alt",
                requiredPermissionName: DIPPermissions.SupportedBanks.Default)
        );

        //context.Menu.AddItem(
        //    new ApplicationMenuItem(
        //        DIPMenus.MediaGalleries,
        //        l["Menu:MediaGalleries"],
        //        url: "/media-galleries",
        //        icon: "fa fa-file-alt",
        //        requiredPermissionName: DIPPermissions.MediaGalleries.Default)
        //);

        //context.Menu.AddItem(
        //    new ApplicationMenuItem(
        //        DIPMenus.Medias,
        //        l["Menu:Medias"],
        //        url: "/medias",
        //        icon: "fa fa-file-alt",
        //        requiredPermissionName: DIPPermissions.Medias.Default)
        //);
        return Task.CompletedTask;
    }
}