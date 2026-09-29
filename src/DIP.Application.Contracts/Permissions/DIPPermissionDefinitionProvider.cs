using DIP.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace DIP.Permissions;

public class DIPPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DIPPermissions.GroupName);

        myGroup.AddPermission(DIPPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(DIPPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(DIPPermissions.MyPermission1, L("Permission:MyPermission1"));

        var sliderHomePagePermission = myGroup.AddPermission(DIPPermissions.SliderHomePages.Default, L("Permission:SliderHomePages"));
        sliderHomePagePermission.AddChild(DIPPermissions.SliderHomePages.Create, L("Permission:Create"));
        sliderHomePagePermission.AddChild(DIPPermissions.SliderHomePages.Edit, L("Permission:Edit"));
        sliderHomePagePermission.AddChild(DIPPermissions.SliderHomePages.Delete, L("Permission:Delete"));

        var dipFactPermission = myGroup.AddPermission(DIPPermissions.DipFacts.Default, L("Permission:DipFacts"));
        dipFactPermission.AddChild(DIPPermissions.DipFacts.Create, L("Permission:Create"));
        dipFactPermission.AddChild(DIPPermissions.DipFacts.Edit, L("Permission:Edit"));
        dipFactPermission.AddChild(DIPPermissions.DipFacts.Delete, L("Permission:Delete"));

        var pressReleasePermission = myGroup.AddPermission(DIPPermissions.PressReleases.Default, L("Permission:PressReleases"));
        pressReleasePermission.AddChild(DIPPermissions.PressReleases.Create, L("Permission:Create"));
        pressReleasePermission.AddChild(DIPPermissions.PressReleases.Edit, L("Permission:Edit"));
        pressReleasePermission.AddChild(DIPPermissions.PressReleases.Delete, L("Permission:Delete"));

        var lastEventsPermission = myGroup.AddPermission(DIPPermissions.LastEventss.Default, L("Permission:LastEventss"));
        lastEventsPermission.AddChild(DIPPermissions.LastEventss.Create, L("Permission:Create"));
        lastEventsPermission.AddChild(DIPPermissions.LastEventss.Edit, L("Permission:Edit"));
        lastEventsPermission.AddChild(DIPPermissions.LastEventss.Delete, L("Permission:Delete"));

        var pageInfoPermission = myGroup.AddPermission(DIPPermissions.PageInfos.Default, L("Permission:PageInfos"));
        pageInfoPermission.AddChild(DIPPermissions.PageInfos.Create, L("Permission:Create"));
        pageInfoPermission.AddChild(DIPPermissions.PageInfos.Edit, L("Permission:Edit"));
        pageInfoPermission.AddChild(DIPPermissions.PageInfos.Delete, L("Permission:Delete"));

        var eServicePermission = myGroup.AddPermission(DIPPermissions.EServices.Default, L("Permission:EServices"));
        eServicePermission.AddChild(DIPPermissions.EServices.Create, L("Permission:Create"));
        eServicePermission.AddChild(DIPPermissions.EServices.Edit, L("Permission:Edit"));
        eServicePermission.AddChild(DIPPermissions.EServices.Delete, L("Permission:Delete"));

        var eFormServicePermission = myGroup.AddPermission(DIPPermissions.EFormServices.Default, L("Permission:EFormServices"));
        eFormServicePermission.AddChild(DIPPermissions.EFormServices.Create, L("Permission:Create"));
        eFormServicePermission.AddChild(DIPPermissions.EFormServices.Edit, L("Permission:Edit"));
        eFormServicePermission.AddChild(DIPPermissions.EFormServices.Delete, L("Permission:Delete"));

        var eFormServiceSubCategoryPermission = myGroup.AddPermission(DIPPermissions.EFormServiceSubCategories.Default, L("Permission:EFormServiceSubCategories"));
        eFormServiceSubCategoryPermission.AddChild(DIPPermissions.EFormServiceSubCategories.Create, L("Permission:Create"));
        eFormServiceSubCategoryPermission.AddChild(DIPPermissions.EFormServiceSubCategories.Edit, L("Permission:Edit"));
        eFormServiceSubCategoryPermission.AddChild(DIPPermissions.EFormServiceSubCategories.Delete, L("Permission:Delete"));

        var zonePermission = myGroup.AddPermission(DIPPermissions.Zones.Default, L("Permission:Zones"));
        zonePermission.AddChild(DIPPermissions.Zones.Create, L("Permission:Create"));
        zonePermission.AddChild(DIPPermissions.Zones.Edit, L("Permission:Edit"));
        zonePermission.AddChild(DIPPermissions.Zones.Delete, L("Permission:Delete"));

        var zoneParagraphPermission = myGroup.AddPermission(DIPPermissions.ZoneParagraphs.Default, L("Permission:ZoneParagraphs"));
        zoneParagraphPermission.AddChild(DIPPermissions.ZoneParagraphs.Create, L("Permission:Create"));
        zoneParagraphPermission.AddChild(DIPPermissions.ZoneParagraphs.Edit, L("Permission:Edit"));
        zoneParagraphPermission.AddChild(DIPPermissions.ZoneParagraphs.Delete, L("Permission:Delete"));

        var mediaPermission = myGroup.AddPermission(DIPPermissions.Medias.Default, L("Permission:Medias"));
        mediaPermission.AddChild(DIPPermissions.Medias.Create, L("Permission:Create"));
        mediaPermission.AddChild(DIPPermissions.Medias.Edit, L("Permission:Edit"));
        mediaPermission.AddChild(DIPPermissions.Medias.Delete, L("Permission:Delete"));

        var amenityPermission = myGroup.AddPermission(DIPPermissions.Amenities.Default, L("Permission:Amenities"));
        amenityPermission.AddChild(DIPPermissions.Amenities.Create, L("Permission:Create"));
        amenityPermission.AddChild(DIPPermissions.Amenities.Edit, L("Permission:Edit"));
        amenityPermission.AddChild(DIPPermissions.Amenities.Delete, L("Permission:Delete"));

        var amenityParagraphPermission = myGroup.AddPermission(DIPPermissions.AmenityParagraphs.Default, L("Permission:AmenityParagraphs"));
        amenityParagraphPermission.AddChild(DIPPermissions.AmenityParagraphs.Create, L("Permission:Create"));
        amenityParagraphPermission.AddChild(DIPPermissions.AmenityParagraphs.Edit, L("Permission:Edit"));
        amenityParagraphPermission.AddChild(DIPPermissions.AmenityParagraphs.Delete, L("Permission:Delete"));

        var timeLineCategoryPermission = myGroup.AddPermission(DIPPermissions.TimeLineCategories.Default, L("Permission:TimeLineCategories"));
        timeLineCategoryPermission.AddChild(DIPPermissions.TimeLineCategories.Create, L("Permission:Create"));
        timeLineCategoryPermission.AddChild(DIPPermissions.TimeLineCategories.Edit, L("Permission:Edit"));
        timeLineCategoryPermission.AddChild(DIPPermissions.TimeLineCategories.Delete, L("Permission:Delete"));

        var timeLinePermission = myGroup.AddPermission(DIPPermissions.TimeLines.Default, L("Permission:TimeLines"));
        timeLinePermission.AddChild(DIPPermissions.TimeLines.Create, L("Permission:Create"));
        timeLinePermission.AddChild(DIPPermissions.TimeLines.Edit, L("Permission:Edit"));
        timeLinePermission.AddChild(DIPPermissions.TimeLines.Delete, L("Permission:Delete"));

        var categoryPermission = myGroup.AddPermission(DIPPermissions.Categories.Default, L("Permission:Categories"));
        categoryPermission.AddChild(DIPPermissions.Categories.Create, L("Permission:Create"));
        categoryPermission.AddChild(DIPPermissions.Categories.Edit, L("Permission:Edit"));
        categoryPermission.AddChild(DIPPermissions.Categories.Delete, L("Permission:Delete"));

        var subCategoryPermission = myGroup.AddPermission(DIPPermissions.SubCategories.Default, L("Permission:SubCategories"));
        subCategoryPermission.AddChild(DIPPermissions.SubCategories.Create, L("Permission:Create"));
        subCategoryPermission.AddChild(DIPPermissions.SubCategories.Edit, L("Permission:Edit"));
        subCategoryPermission.AddChild(DIPPermissions.SubCategories.Delete, L("Permission:Delete"));

        var commercialPermission = myGroup.AddPermission(DIPPermissions.Commercials.Default, L("Permission:Commercials"));
        commercialPermission.AddChild(DIPPermissions.Commercials.Create, L("Permission:Create"));
        commercialPermission.AddChild(DIPPermissions.Commercials.Edit, L("Permission:Edit"));
        commercialPermission.AddChild(DIPPermissions.Commercials.Delete, L("Permission:Delete"));

        var dipBranchPermission = myGroup.AddPermission(DIPPermissions.DipBranches.Default, L("Permission:DipBranches"));
        dipBranchPermission.AddChild(DIPPermissions.DipBranches.Create, L("Permission:Create"));
        dipBranchPermission.AddChild(DIPPermissions.DipBranches.Edit, L("Permission:Edit"));
        dipBranchPermission.AddChild(DIPPermissions.DipBranches.Delete, L("Permission:Delete"));

        var majorIndustryPermission = myGroup.AddPermission(DIPPermissions.MajorIndustries.Default, L("Permission:MajorIndustries"));
        majorIndustryPermission.AddChild(DIPPermissions.MajorIndustries.Create, L("Permission:Create"));
        majorIndustryPermission.AddChild(DIPPermissions.MajorIndustries.Edit, L("Permission:Edit"));
        majorIndustryPermission.AddChild(DIPPermissions.MajorIndustries.Delete, L("Permission:Delete"));

        var siteSettingPermission = myGroup.AddPermission(DIPPermissions.SiteSettings.Default, L("Permission:SiteSettings"));
        siteSettingPermission.AddChild(DIPPermissions.SiteSettings.Create, L("Permission:Create"));
        siteSettingPermission.AddChild(DIPPermissions.SiteSettings.Edit, L("Permission:Edit"));
        siteSettingPermission.AddChild(DIPPermissions.SiteSettings.Delete, L("Permission:Delete"));

        var pageInfoSectionPermission = myGroup.AddPermission(DIPPermissions.PageInfoSections.Default, L("Permission:PageInfoSections"));
        pageInfoSectionPermission.AddChild(DIPPermissions.PageInfoSections.Create, L("Permission:Create"));
        pageInfoSectionPermission.AddChild(DIPPermissions.PageInfoSections.Edit, L("Permission:Edit"));
        pageInfoSectionPermission.AddChild(DIPPermissions.PageInfoSections.Delete, L("Permission:Delete"));

        var inquiryFormPermission = myGroup.AddPermission(DIPPermissions.InquiryForms.Default, L("Permission:InquiryForms"));
        inquiryFormPermission.AddChild(DIPPermissions.InquiryForms.Create, L("Permission:Create"));
        inquiryFormPermission.AddChild(DIPPermissions.InquiryForms.Edit, L("Permission:Edit"));
        inquiryFormPermission.AddChild(DIPPermissions.InquiryForms.Delete, L("Permission:Delete"));

        var contactFormPermission = myGroup.AddPermission(DIPPermissions.ContactForms.Default, L("Permission:ContactForms"));
        contactFormPermission.AddChild(DIPPermissions.ContactForms.Create, L("Permission:Create"));
        contactFormPermission.AddChild(DIPPermissions.ContactForms.Edit, L("Permission:Edit"));
        contactFormPermission.AddChild(DIPPermissions.ContactForms.Delete, L("Permission:Delete"));

        var feedBackPermission = myGroup.AddPermission(DIPPermissions.FeedBacks.Default, L("Permission:FeedBacks"));
        feedBackPermission.AddChild(DIPPermissions.FeedBacks.Create, L("Permission:Create"));
        feedBackPermission.AddChild(DIPPermissions.FeedBacks.Edit, L("Permission:Edit"));
        feedBackPermission.AddChild(DIPPermissions.FeedBacks.Delete, L("Permission:Delete"));

        var supportedBankPermission = myGroup.AddPermission(DIPPermissions.SupportedBanks.Default, L("Permission:SupportedBanks"));
        supportedBankPermission.AddChild(DIPPermissions.SupportedBanks.Create, L("Permission:Create"));
        supportedBankPermission.AddChild(DIPPermissions.SupportedBanks.Edit, L("Permission:Edit"));
        supportedBankPermission.AddChild(DIPPermissions.SupportedBanks.Delete, L("Permission:Delete"));

        var mediaGalleryPermission = myGroup.AddPermission(DIPPermissions.MediaGalleries.Default, L("Permission:MediaGalleries"));
        mediaGalleryPermission.AddChild(DIPPermissions.MediaGalleries.Create, L("Permission:Create"));
        mediaGalleryPermission.AddChild(DIPPermissions.MediaGalleries.Edit, L("Permission:Edit"));
        mediaGalleryPermission.AddChild(DIPPermissions.MediaGalleries.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DIPResource>(name);
    }
}