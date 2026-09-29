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
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace DIP.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class DIPDbContext :
    AbpDbContext<DIPDbContext>,
    IIdentityProDbContext,
    ISaasDbContext
{
    public DbSet<MediaGallery> MediaGalleries { get; set; }
    public DbSet<SupportedBank> SupportedBanks { get; set; }
    public DbSet<FeedBack> FeedBacks { get; set; }
    public DbSet<ContactForm> ContactForms { get; set; }
    public DbSet<InquiryForm> InquiryForms { get; set; }
    public DbSet<PageInfoSection> PageInfoSections { get; set; }
    public DbSet<SiteSetting> SiteSettings { get; set; }
    public DbSet<MajorIndustry> MajorIndustries { get; set; }
    public DbSet<DipBranch> DipBranches { get; set; }
    public DbSet<Commercial> Commercials { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<TimeLine> TimeLines { get; set; }
    public DbSet<TimeLineCategory> TimeLineCategories { get; set; }
    public DbSet<AmenityParagraph> AmenityParagraphs { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<ZoneParagraph> ZoneParagraphs { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<EFormServiceSubCategory> EFormServiceSubCategories { get; set; }
    public DbSet<EFormService> EFormServices { get; set; }
    public DbSet<EService> EServices { get; set; }
    public DbSet<PageInfo> PageInfos { get; set; }
    public DbSet<LastEvents> LastEventss { get; set; }
    public DbSet<PressRelease> PressReleases { get; set; }
    public DbSet<DipFact> DipFacts { get; set; }
    public DbSet<SliderHomePage> SliderHomePages { get; set; }
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DIPDbContext(DbContextOptions<DIPDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureFeatureManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureGdpr();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(DIPConsts.DbTablePrefix + "YourEntities", DIPConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<EService>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "EServices", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(EService.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(EService.TitleAr));
    b.Property(x => x.Slug).HasColumnName(nameof(EService.Slug)).IsRequired();
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(EService.DescriptionEn));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(EService.DescriptionAr));
    b.Property(x => x.Image).HasColumnName(nameof(EService.Image));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(EService.HeaderImage));
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(EService.MetaTitleEn));
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(EService.MetaTitleAr));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(EService.MetaDescriptionEn));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(EService.MetaDescriptionAr));
    b.Property(x => x.Order).HasColumnName(nameof(EService.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(EService.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<EFormService>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "EFormServices", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(EFormService.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(EFormService.TitleAr));
    b.Property(x => x.Order).HasColumnName(nameof(EFormService.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(EFormService.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<EFormServiceSubCategory>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "EFormServiceSubCategories", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(EFormServiceSubCategory.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(EFormServiceSubCategory.TitleAr));
    b.Property(x => x.File).HasColumnName(nameof(EFormServiceSubCategory.File));
    b.Property(x => x.Order).HasColumnName(nameof(EFormServiceSubCategory.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(EFormServiceSubCategory.IsActive));
    b.HasOne<EFormService>().WithMany().IsRequired().HasForeignKey(x => x.EFormServiceId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<TimeLineCategory>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "TimeLineCategories", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(TimeLineCategory.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(TimeLineCategory.TitleAr)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(TimeLineCategory.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(TimeLineCategory.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {

        }

        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SiteSetting>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "SiteSettings", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.FaceBookLink).HasColumnName(nameof(SiteSetting.FaceBookLink));
    b.Property(x => x.TwitterLink).HasColumnName(nameof(SiteSetting.TwitterLink));
    b.Property(x => x.InstagramLink).HasColumnName(nameof(SiteSetting.InstagramLink));
    b.Property(x => x.YouTubeLink).HasColumnName(nameof(SiteSetting.YouTubeLink));
    b.Property(x => x.LinkedinLink).HasColumnName(nameof(SiteSetting.LinkedinLink));
    b.Property(x => x.FireDepartmentPhone).HasColumnName(nameof(SiteSetting.FireDepartmentPhone));
    b.Property(x => x.Emergency1Phone).HasColumnName(nameof(SiteSetting.Emergency1Phone));
    b.Property(x => x.Emergency2Phone).HasColumnName(nameof(SiteSetting.Emergency2Phone));
    b.Property(x => x.PolicePost).HasColumnName(nameof(SiteSetting.PolicePost));
    b.Property(x => x.DipEmail).HasColumnName(nameof(SiteSetting.DipEmail));
    b.Property(x => x.Phone).HasColumnName(nameof(SiteSetting.Phone));
    b.Property(x => x.POBox).HasColumnName(nameof(SiteSetting.POBox));
    b.Property(x => x.OfficeLocationEn).HasColumnName(nameof(SiteSetting.OfficeLocationEn));
    b.Property(x => x.OfficeLocationAr).HasColumnName(nameof(SiteSetting.OfficeLocationAr));
    b.Property(x => x.SiteLink).HasColumnName(nameof(SiteSetting.SiteLink));
    b.Property(x => x.PbLocation).HasColumnName(nameof(SiteSetting.PbLocation));
    b.Property(x => x.WorkDays).HasColumnName(nameof(SiteSetting.WorkDays));
    b.Property(x => x.WorkHours).HasColumnName(nameof(SiteSetting.WorkHours));
    b.Property(x => x.RamadanWorkDays).HasColumnName(nameof(SiteSetting.RamadanWorkDays));
    b.Property(x => x.RamadanWorkHours).HasColumnName(nameof(SiteSetting.RamadanWorkHours));
    b.Property(x => x.FridayWorkHours).HasColumnName(nameof(SiteSetting.FridayWorkHours));
    b.Property(x => x.ClosedDay1).HasColumnName(nameof(SiteSetting.ClosedDay1));
    b.Property(x => x.ClosedDay2).HasColumnName(nameof(SiteSetting.ClosedDay2));
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SupportedBank>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "SupportedBanks", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(SupportedBank.TitleAr));
    b.Property(x => x.TitleEn).HasColumnName(nameof(SupportedBank.TitleEn));
    b.Property(x => x.IsActive).HasColumnName(nameof(SupportedBank.IsActive));
    b.Property(x => x.Order).HasColumnName(nameof(SupportedBank.Order));
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Media>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "Medias", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(Media.TitleEn));
    b.Property(x => x.TitleAr).HasColumnName(nameof(Media.TitleAr));
    b.Property(x => x.File).HasColumnName(nameof(Media.File));
    b.Property(x => x.Order).HasColumnName(nameof(Media.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(Media.IsActive));
    b.HasOne<ZoneParagraph>().WithMany().HasForeignKey(x => x.ZoneParagraphId).OnDelete(DeleteBehavior.NoAction);
    b.HasOne<AmenityParagraph>().WithMany().HasForeignKey(x => x.AmenityParagraphId).OnDelete(DeleteBehavior.NoAction);
    b.HasOne<MediaGallery>().WithMany().HasForeignKey(x => x.MediaGalleryId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<MediaGallery>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "MediaGalleries", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(MediaGallery.TitleEn)).IsRequired();
    b.Property(x => x.Slug).HasColumnName(nameof(MediaGallery.Slug)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(MediaGallery.TitleAr));
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(MediaGallery.MetaTitleEn));
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(MediaGallery.MetaTitleAr));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(MediaGallery.MetaDescriptionEn));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(MediaGallery.MetaDescriptionAr));
    b.Property(x => x.SummaryEn).HasColumnName(nameof(MediaGallery.SummaryEn));
    b.Property(x => x.SummaryAr).HasColumnName(nameof(MediaGallery.SummaryAr));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(MediaGallery.HeaderImage));
    b.Property(x => x.Image).HasColumnName(nameof(MediaGallery.Image));
    b.Property(x => x.Order).HasColumnName(nameof(MediaGallery.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(MediaGallery.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<DipBranch>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "DipBranches", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(DipBranch.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(DipBranch.TitleAr)).IsRequired();
    b.Property(x => x.SubTitleEn).HasColumnName(nameof(DipBranch.SubTitleEn));
    b.Property(x => x.SubTitleAr).HasColumnName(nameof(DipBranch.SubTitleAr));
    b.Property(x => x.Phone).HasColumnName(nameof(DipBranch.Phone));
    b.Property(x => x.AlternativePhone).HasColumnName(nameof(DipBranch.AlternativePhone));
    b.Property(x => x.Email).HasColumnName(nameof(DipBranch.Email));
    b.Property(x => x.AlternativeEmail).HasColumnName(nameof(DipBranch.AlternativeEmail));
    b.Property(x => x.Order).HasColumnName(nameof(DipBranch.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(DipBranch.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<MajorIndustry>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "MajorIndustries", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(MajorIndustry.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(MajorIndustry.TitleAr)).IsRequired();
    b.Property(x => x.Image).HasColumnName(nameof(MajorIndustry.Image));
    b.Property(x => x.Order).HasColumnName(nameof(MajorIndustry.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(MajorIndustry.IsActive));
    b.HasOne<Zone>().WithMany().IsRequired().HasForeignKey(x => x.ZoneId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<DipFact>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "DipFacts", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.Image).HasColumnName(nameof(DipFact.Image));
    b.Property(x => x.TitleAr).HasColumnName(nameof(DipFact.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(DipFact.TitleEn)).IsRequired();
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(DipFact.DescriptionAr)).IsRequired();
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(DipFact.DescriptionEn)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(DipFact.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(DipFact.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PageInfoSection>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "PageInfoSections", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(PageInfoSection.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(PageInfoSection.TitleAr)).IsRequired();
    b.Property(x => x.SubTitleEn).HasColumnName(nameof(PageInfoSection.SubTitleEn));
    b.Property(x => x.SubTitleAr).HasColumnName(nameof(PageInfoSection.SubTitleAr));
    b.Property(x => x.SummaryEn).HasColumnName(nameof(PageInfoSection.SummaryEn));
    b.Property(x => x.SummaryAr).HasColumnName(nameof(PageInfoSection.SummaryAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(PageInfoSection.DescriptionEn));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(PageInfoSection.DescriptionAr));
    b.Property(x => x.PageSectionMedia).HasColumnName(nameof(PageInfoSection.PageSectionMedia));
    b.Property(x => x.YoutubeUrl).HasColumnName(nameof(PageInfoSection.YoutubeUrl));
    b.Property(x => x.Order).HasColumnName(nameof(PageInfoSection.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(PageInfoSection.IsActive));
    b.HasOne<PageInfo>().WithMany().IsRequired().HasForeignKey(x => x.PageInfoId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PageInfo>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "PageInfos", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(PageInfo.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(PageInfo.TitleEn)).IsRequired();
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(PageInfo.MetaTitleAr));
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(PageInfo.MetaTitleEn));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(PageInfo.MetaDescriptionEn));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(PageInfo.MetaDescriptionAr));
    b.Property(x => x.Slug).HasColumnName(nameof(PageInfo.Slug)).IsRequired();
    b.Property(x => x.Image).HasColumnName(nameof(PageInfo.Image));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(PageInfo.HeaderImage));
    b.Property(x => x.YouTubeUrl).HasColumnName(nameof(PageInfo.YouTubeUrl));
    b.Property(x => x.PageInfoArticleTilteEn).HasColumnName(nameof(PageInfo.PageInfoArticleTilteEn));
    b.Property(x => x.PageInfoArticleTilteAr).HasColumnName(nameof(PageInfo.PageInfoArticleTilteAr));
    b.Property(x => x.PageInfoArticleSubtitleEn).HasColumnName(nameof(PageInfo.PageInfoArticleSubtitleEn));
    b.Property(x => x.PageInfoArticleSubtitleAr).HasColumnName(nameof(PageInfo.PageInfoArticleSubtitleAr));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(PageInfo.DescriptionAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(PageInfo.DescriptionEn));
    b.Property(x => x.SummaryEn).HasColumnName(nameof(PageInfo.SummaryEn));
    b.Property(x => x.SummaryAr).HasColumnName(nameof(PageInfo.SummaryAr));
    b.Property(x => x.Order).HasColumnName(nameof(PageInfo.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(PageInfo.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PressRelease>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "PressReleases", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(PressRelease.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(PressRelease.TitleEn)).IsRequired();
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(PressRelease.MetaTitleAr));
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(PressRelease.MetaTitleEn));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(PressRelease.MetaDescriptionEn));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(PressRelease.MetaDescriptionAr));
    b.Property(x => x.IsFeatured).HasColumnName(nameof(PressRelease.IsFeatured));
    b.Property(x => x.Slug).HasColumnName(nameof(PressRelease.Slug)).IsRequired();
    b.Property(x => x.Image).HasColumnName(nameof(PressRelease.Image));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(PressRelease.HeaderImage));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(PressRelease.DescriptionAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(PressRelease.DescriptionEn));
    b.Property(x => x.SummaryEn).HasColumnName(nameof(PressRelease.SummaryEn)).IsRequired();
    b.Property(x => x.SummaryAr).HasColumnName(nameof(PressRelease.SummaryAr)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(PressRelease.Order));
    b.Property(x => x.Date).HasColumnName(nameof(PressRelease.Date));
    b.Property(x => x.IsActive).HasColumnName(nameof(PressRelease.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<LastEvents>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "LastEventss", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(LastEvents.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(LastEvents.TitleEn)).IsRequired();
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(LastEvents.MetaTitleAr));
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(LastEvents.MetaTitleEn));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(LastEvents.MetaDescriptionEn));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(LastEvents.MetaDescriptionAr));
    b.Property(x => x.IsFeatured).HasColumnName(nameof(LastEvents.IsFeatured));
    b.Property(x => x.Slug).HasColumnName(nameof(LastEvents.Slug)).IsRequired();
    b.Property(x => x.Image).HasColumnName(nameof(LastEvents.Image));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(LastEvents.HeaderImage));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(LastEvents.DescriptionAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(LastEvents.DescriptionEn));
    b.Property(x => x.SummaryEn).HasColumnName(nameof(LastEvents.SummaryEn)).IsRequired();
    b.Property(x => x.SummaryAr).HasColumnName(nameof(LastEvents.SummaryAr)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(LastEvents.Order));
    b.Property(x => x.StartDate).HasColumnName(nameof(LastEvents.StartDate));
    b.Property(x => x.EndDate).HasColumnName(nameof(LastEvents.EndDate));
    b.Property(x => x.IsActive).HasColumnName(nameof(LastEvents.IsActive));
    b.Property(x => x.LocationAr).HasColumnName(nameof(LastEvents.LocationAr));
    b.Property(x => x.LocationEn).HasColumnName(nameof(LastEvents.LocationEn));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<TimeLine>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "TimeLines", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(TimeLine.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(TimeLine.TitleAr)).IsRequired();
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(TimeLine.DescriptionEn));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(TimeLine.DescriptionAr));
    b.Property(x => x.Image).HasColumnName(nameof(TimeLine.Image));
    b.Property(x => x.TimeLineDate).HasColumnName(nameof(TimeLine.TimeLineDate));
    b.Property(x => x.Order).HasColumnName(nameof(TimeLine.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(TimeLine.IsActive));
    b.HasOne<TimeLineCategory>().WithMany().IsRequired().HasForeignKey(x => x.TimeLineCategoryId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<AmenityParagraph>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "AmenityParagraphs", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(AmenityParagraph.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(AmenityParagraph.TitleAr)).IsRequired();
    b.Property(x => x.SubTitleEn).HasColumnName(nameof(AmenityParagraph.SubTitleEn));
    b.Property(x => x.SubTitleAr).HasColumnName(nameof(AmenityParagraph.SubTitleAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(AmenityParagraph.DescriptionEn));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(AmenityParagraph.DescriptionAr));
    b.Property(x => x.ButtonUrl).HasColumnName(nameof(AmenityParagraph.ButtonUrl));
    b.Property(x => x.Order).HasColumnName(nameof(AmenityParagraph.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(AmenityParagraph.IsActive));
    b.HasOne<Amenity>().WithMany().IsRequired().HasForeignKey(x => x.AmenityId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ZoneParagraph>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "ZoneParagraphs", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(ZoneParagraph.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(ZoneParagraph.TitleAr)).IsRequired();
    b.Property(x => x.SubTilteEn).HasColumnName(nameof(ZoneParagraph.SubTilteEn));
    b.Property(x => x.SubTitleAr).HasColumnName(nameof(ZoneParagraph.SubTitleAr));
    b.Property(x => x.DescriptionEn).HasColumnName(nameof(ZoneParagraph.DescriptionEn));
    b.Property(x => x.DescriptionAr).HasColumnName(nameof(ZoneParagraph.DescriptionAr));
    b.Property(x => x.Order).HasColumnName(nameof(ZoneParagraph.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(ZoneParagraph.IsActive));
    b.HasOne<Zone>().WithMany().IsRequired().HasForeignKey(x => x.ZoneId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Amenity>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "Amenities", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(Amenity.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(Amenity.TitleAr)).IsRequired();
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(Amenity.MetaTitleEn));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(Amenity.MetaDescriptionEn));
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(Amenity.MetaTitleAr));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(Amenity.MetaDescriptionAr));
    b.Property(x => x.Slug).HasColumnName(nameof(Amenity.Slug)).IsRequired();
    b.Property(x => x.HeaderImage).HasColumnName(nameof(Amenity.HeaderImage));
    b.Property(x => x.Image).HasColumnName(nameof(Amenity.Image));
    b.Property(x => x.Order).HasColumnName(nameof(Amenity.Order));
    b.Property(x => x.IsActive).HasColumnName(nameof(Amenity.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<InquiryForm>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "InquiryForms", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.CompanyName).HasColumnName(nameof(InquiryForm.CompanyName));
    b.Property(x => x.Name).HasColumnName(nameof(InquiryForm.Name)).IsRequired();
    b.Property(x => x.Email).HasColumnName(nameof(InquiryForm.Email));
    b.Property(x => x.Mobile).HasColumnName(nameof(InquiryForm.Mobile)).IsRequired();
    b.Property(x => x.Fax).HasColumnName(nameof(InquiryForm.Fax));
    b.Property(x => x.Phone).HasColumnName(nameof(InquiryForm.Phone));
    b.Property(x => x.InquiryType).HasColumnName(nameof(InquiryForm.InquiryType));
    b.Property(x => x.TradeLicensePlateOfIssue).HasColumnName(nameof(InquiryForm.TradeLicensePlateOfIssue));
    b.Property(x => x.BuyRent).HasColumnName(nameof(InquiryForm.BuyRent));
    b.Property(x => x.SpaceInSquareFeet).HasColumnName(nameof(InquiryForm.SpaceInSquareFeet));
    b.Property(x => x.Comments).HasColumnName(nameof(InquiryForm.Comments));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ContactForm>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "ContactForms", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.FullName).HasColumnName(nameof(ContactForm.FullName)).IsRequired();
    b.Property(x => x.Email).HasColumnName(nameof(ContactForm.Email)).IsRequired();
    b.Property(x => x.Subject).HasColumnName(nameof(ContactForm.Subject));
    b.Property(x => x.Message).HasColumnName(nameof(ContactForm.Message));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<FeedBack>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "FeedBacks", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.Subject).HasColumnName(nameof(FeedBack.Subject)).IsRequired();
    b.Property(x => x.CompanyName).HasColumnName(nameof(FeedBack.CompanyName)).IsRequired();
    b.Property(x => x.PlotNo).HasColumnName(nameof(FeedBack.PlotNo)).IsRequired();
    b.Property(x => x.PlotCategory).HasColumnName(nameof(FeedBack.PlotCategory)).IsRequired();
    b.Property(x => x.ContactPersonName).HasColumnName(nameof(FeedBack.ContactPersonName)).IsRequired();
    b.Property(x => x.EmailId).HasColumnName(nameof(FeedBack.EmailId)).IsRequired();
    b.Property(x => x.MobileNumber).HasColumnName(nameof(FeedBack.MobileNumber)).IsRequired();
    b.Property(x => x.Department).HasColumnName(nameof(FeedBack.Department)).IsRequired();
    b.Property(x => x.CategoryName).HasColumnName(nameof(FeedBack.CategoryName)).IsRequired();
    b.Property(x => x.Description).HasColumnName(nameof(FeedBack.Description)).IsRequired();
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Zone>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "Zones", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(Zone.TitleEn)).IsRequired();
    b.Property(x => x.TitleAR).HasColumnName(nameof(Zone.TitleAR)).IsRequired();
    b.Property(x => x.MetaTitleEn).HasColumnName(nameof(Zone.MetaTitleEn));
    b.Property(x => x.MetaDescriptionEn).HasColumnName(nameof(Zone.MetaDescriptionEn));
    b.Property(x => x.MetaTitleAr).HasColumnName(nameof(Zone.MetaTitleAr));
    b.Property(x => x.MetaDescriptionAr).HasColumnName(nameof(Zone.MetaDescriptionAr));
    b.Property(x => x.Slug).HasColumnName(nameof(Zone.Slug)).IsRequired();
    b.Property(x => x.SummaryEn).HasColumnName(nameof(Zone.SummaryEn));
    b.Property(x => x.SummaryAr).HasColumnName(nameof(Zone.SummaryAr));
    b.Property(x => x.Image).HasColumnName(nameof(Zone.Image));
    b.Property(x => x.HeaderImage).HasColumnName(nameof(Zone.HeaderImage));
    b.Property(x => x.Order).HasColumnName(nameof(Zone.Order));
    b.Property(x => x.IsFeature).HasColumnName(nameof(Zone.IsFeature));
    b.Property(x => x.IsActive).HasColumnName(nameof(Zone.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Category>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "Categories", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(Category.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(Category.TitleEn)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(Category.Order));
    b.Property(x => x.IsFeature).HasColumnName(nameof(Category.IsFeature));
    b.Property(x => x.IsActive).HasColumnName(nameof(Category.IsActive));
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SubCategory>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "SubCategories", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleAr).HasColumnName(nameof(SubCategory.TitleAr)).IsRequired();
    b.Property(x => x.TitleEn).HasColumnName(nameof(SubCategory.TitleEn)).IsRequired();
    b.Property(x => x.Order).HasColumnName(nameof(SubCategory.Order));
    b.Property(x => x.IsFeature).HasColumnName(nameof(SubCategory.IsFeature));
    b.Property(x => x.IsActive).HasColumnName(nameof(SubCategory.IsActive));
    b.HasOne<Category>().WithMany().IsRequired().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Commercial>(b =>
{
    b.ToTable(DIPConsts.DbTablePrefix + "Commercials", DIPConsts.DbSchema);
    b.ConfigureByConvention();
    b.Property(x => x.TitleEn).HasColumnName(nameof(Commercial.TitleEn)).IsRequired();
    b.Property(x => x.TitleAr).HasColumnName(nameof(Commercial.TitleAr)).IsRequired();
    b.Property(x => x.PlotNo).HasColumnName(nameof(Commercial.PlotNo));
    b.Property(x => x.ActivityEn).HasColumnName(nameof(Commercial.ActivityEn));
    b.Property(x => x.ActivityAr).HasColumnName(nameof(Commercial.ActivityAr));
    b.Property(x => x.Phone).HasColumnName(nameof(Commercial.Phone));
    b.Property(x => x.Fax).HasColumnName(nameof(Commercial.Fax));
    b.Property(x => x.MakaniNo).HasColumnName(nameof(Commercial.MakaniNo));
    b.Property(x => x.IsActive).HasColumnName(nameof(Commercial.IsActive));
    b.Property(x => x.Order).HasColumnName(nameof(Commercial.Order));
    b.HasOne<SubCategory>().WithMany().IsRequired().HasForeignKey(x => x.SubCategoryId).OnDelete(DeleteBehavior.NoAction);
});

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SliderHomePage>(b =>
            {
                b.ToTable(DIPConsts.DbTablePrefix + "SliderHomePages", DIPConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.TitleAr).HasColumnName(nameof(SliderHomePage.TitleAr)).IsRequired();
                b.Property(x => x.TitleEn).HasColumnName(nameof(SliderHomePage.TitleEn)).IsRequired();
                b.Property(x => x.DescriptionAr).HasColumnName(nameof(SliderHomePage.DescriptionAr)).IsRequired();
                b.Property(x => x.DescriptionEn).HasColumnName(nameof(SliderHomePage.DescriptionEn)).IsRequired();
                b.Property(x => x.ButtonTitleAr).HasColumnName(nameof(SliderHomePage.ButtonTitleAr));
                b.Property(x => x.ButtonTitleEn).HasColumnName(nameof(SliderHomePage.ButtonTitleEn));
                b.Property(x => x.ButtonUrlEn).HasColumnName(nameof(SliderHomePage.ButtonUrlEn));
                b.Property(x => x.ButtonUrlAr).HasColumnName(nameof(SliderHomePage.ButtonUrlAr));
                b.Property(x => x.Image).HasColumnName(nameof(SliderHomePage.Image));
                b.Property(x => x.YoutubeUrl).HasColumnName(nameof(SliderHomePage.YoutubeUrl));
                b.Property(x => x.IsActive).HasColumnName(nameof(SliderHomePage.IsActive));
                b.Property(x => x.Order).HasColumnName(nameof(SliderHomePage.Order));
            });

        }
    }
}