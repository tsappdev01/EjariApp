using DIP.MediaGalleries;
using DIP.SupportedBanks;
using DIP.FeedBacks;
using DIP.ContactForms;
using DIP.InquiryForms;
using DIP.PageInfoSections;
using DIP.SiteSettings;
using DIP.MajorIndustries;
using DIP.DipBranches;
using DIP.Commercials;
using DIP.SubCategories;
using DIP.Categories;
using DIP.TimeLines;
using DIP.TimeLineCategories;
using DIP.AmenityParagraphs;
using DIP.Amenities;
using DIP.Medias;
using DIP.ZoneParagraphs;
using DIP.Zones;
using DIP.EFormServiceSubCategories;
using DIP.EFormServices;
using DIP.EServices;
using DIP.PageInfos;
using DIP.LastEventss;
using DIP.PressReleases;
using DIP.DipFacts;
using DIP.SliderHomePages;
using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace DIP.EntityFrameworkCore;

[DependsOn(
    typeof(DIPDomainModule),
    typeof(AbpIdentityProEntityFrameworkCoreModule),
    typeof(AbpOpenIddictProEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(LanguageManagementEntityFrameworkCoreModule),
    typeof(SaasEntityFrameworkCoreModule),
    typeof(TextTemplateManagementEntityFrameworkCoreModule),
    typeof(AbpGdprEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
    )]
public class DIPEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        DIPEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<DIPDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<SliderHomePage, SliderHomePages.EfCoreSliderHomePageRepository>();

            options.AddRepository<DipFact, DipFacts.EfCoreDipFactRepository>();

            options.AddRepository<PressRelease, PressReleases.EfCorePressReleaseRepository>();

            options.AddRepository<LastEvents, LastEventss.EfCoreLastEventsRepository>();

            options.AddRepository<PageInfo, PageInfos.EfCorePageInfoRepository>();

            options.AddRepository<EService, EServices.EfCoreEServiceRepository>();

            options.AddRepository<EFormService, EFormServices.EfCoreEFormServiceRepository>();

            options.AddRepository<EFormServiceSubCategory, EFormServiceSubCategories.EfCoreEFormServiceSubCategoryRepository>();

            options.AddRepository<Zone, Zones.EfCoreZoneRepository>();

            options.AddRepository<ZoneParagraph, ZoneParagraphs.EfCoreZoneParagraphRepository>();

            options.AddRepository<Media, Medias.EfCoreMediaRepository>();

            options.AddRepository<Amenity, Amenities.EfCoreAmenityRepository>();

            options.AddRepository<AmenityParagraph, AmenityParagraphs.EfCoreAmenityParagraphRepository>();

            options.AddRepository<TimeLineCategory, TimeLineCategories.EfCoreTimeLineCategoryRepository>();

            options.AddRepository<TimeLine, TimeLines.EfCoreTimeLineRepository>();

            options.AddRepository<Category, Categories.EfCoreCategoryRepository>();

            options.AddRepository<SubCategory, SubCategories.EfCoreSubCategoryRepository>();

            options.AddRepository<Commercial, Commercials.EfCoreCommercialRepository>();

            options.AddRepository<DipBranch, DipBranches.EfCoreDipBranchRepository>();

            options.AddRepository<MajorIndustry, MajorIndustries.EfCoreMajorIndustryRepository>();

            options.AddRepository<SiteSetting, SiteSettings.EfCoreSiteSettingRepository>();

            options.AddRepository<PageInfoSection, PageInfoSections.EfCorePageInfoSectionRepository>();

            options.AddRepository<InquiryForm, InquiryForms.EfCoreInquiryFormRepository>();

            options.AddRepository<ContactForm, ContactForms.EfCoreContactFormRepository>();

            options.AddRepository<FeedBack, FeedBacks.EfCoreFeedBackRepository>();

            options.AddRepository<SupportedBank, SupportedBanks.EfCoreSupportedBankRepository>();

            options.AddRepository<MediaGallery, MediaGalleries.EfCoreMediaGalleryRepository>();

        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also DIPDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });

    }
}