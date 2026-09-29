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
using System;
using DIP.Shared;
using Volo.Abp.AutoMapper;
using DIP.SliderHomePages;
using AutoMapper;
using System.Globalization;

namespace DIP;

public class DIPApplicationAutoMapperProfile : Profile
{
    public DIPApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<SliderHomePage, SliderHomePageDto>();
        CreateMap<SliderHomePage, SliderHomePageExcelDto>();

        CreateMap<DipFact, DipFactDto>();
        CreateMap<DipFact, DipFactExcelDto>();

        CreateMap<PressRelease, PressReleaseDto>();
        CreateMap<PressRelease, PressReleaseExcelDto>();

        CreateMap<LastEvents, LastEventsDto>();
        CreateMap<LastEvents, LastEventsExcelDto>();

        CreateMap<PageInfo, PageInfoDto>();
        CreateMap<PageInfo, PageInfoExcelDto>();

        CreateMap<EService, EServiceDto>();
        CreateMap<EService, EServiceExcelDto>();

        CreateMap<EFormService, EFormServiceDto>();
        CreateMap<EFormService, EFormServiceExcelDto>();

        CreateMap<EFormServiceSubCategory, EFormServiceSubCategoryDto>();
        CreateMap<EFormServiceSubCategory, EFormServiceSubCategoryExcelDto>();
        CreateMap<EFormServiceSubCategoryWithNavigationProperties, EFormServiceSubCategoryWithNavigationPropertiesDto>();
        CreateMap<EFormService, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<Zone, ZoneDto>();
        CreateMap<Zone, ZoneExcelDto>();

        CreateMap<ZoneParagraph, ZoneParagraphDto>();
        CreateMap<ZoneParagraph, ZoneParagraphExcelDto>();
        CreateMap<ZoneParagraphWithNavigationProperties, ZoneParagraphWithNavigationPropertiesDto>();
        CreateMap<Zone, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));
        CreateMap<Media, MediaDto>();

        CreateMap<Media, MediaDto>()
             .ForMember(dest => dest.OldFileName, opt => opt.MapFrom(src => src.File));
        CreateMap<Media, MediaExcelDto>();
        CreateMap<MediaWithNavigationProperties, MediaWithNavigationPropertiesDto>();
        CreateMap<ZoneParagraph, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<Amenity, AmenityDto>();
        CreateMap<Amenity, AmenityExcelDto>();

        CreateMap<AmenityParagraph, AmenityParagraphDto>();
        CreateMap<AmenityParagraph, AmenityParagraphExcelDto>();

        CreateMap<AmenityParagraphWithNavigationProperties, AmenityParagraphWithNavigationPropertiesDto>();
        CreateMap<Amenity, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<AmenityParagraph, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<TimeLineCategory, TimeLineCategoryDto>();
        CreateMap<TimeLineCategory, TimeLineCategoryExcelDto>();

        CreateMap<TimeLine, TimeLineDto>();
        CreateMap<TimeLine, TimeLineExcelDto>();
        CreateMap<TimeLineWithNavigationProperties, TimeLineWithNavigationPropertiesDto>();
        CreateMap<TimeLineCategory, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryExcelDto>();

        CreateMap<SubCategory, SubCategoryDto>();
        CreateMap<SubCategory, SubCategoryExcelDto>();

        CreateMap<SubCategoryWithNavigationProperties, SubCategoryWithNavigationPropertiesDto>();
        CreateMap<Category, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<Commercial, CommercialDto>();
        CreateMap<Commercial, CommercialExcelDto>();
        CreateMap<CommercialWithNavigationProperties, CommercialWithNavigationPropertiesDto>();
        CreateMap<SubCategory, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));

        CreateMap<DipBranch, DipBranchDto>();
        CreateMap<DipBranch, DipBranchExcelDto>();

        CreateMap<MajorIndustry, MajorIndustryDto>();
        CreateMap<MajorIndustry, MajorIndustryExcelDto>();
        CreateMap<MajorIndustryWithNavigationProperties, MajorIndustryWithNavigationPropertiesDto>();

        CreateMap<SiteSetting, SiteSettingDto>();
        CreateMap<SiteSetting, SiteSettingExcelDto>();

        CreateMap<PageInfoSection, PageInfoSectionDto>();
        CreateMap<PageInfoSection, PageInfoSectionExcelDto>();
        CreateMap<PageInfoSectionWithNavigationProperties, PageInfoSectionWithNavigationPropertiesDto>();
        CreateMap<PageInfo, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));
        CreateMap<PageInfo, PageInfoHeaderFrontEnd>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? src.TitleEn : src.TitleAr))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug));
        CreateMap<InquiryForm, InquiryFormDto>();
        CreateMap<InquiryForm, InquiryFormExcelDto>();

        CreateMap<ContactForm, ContactFormDto>();
        CreateMap<ContactForm, ContactFormExcelDto>();

        CreateMap<FeedBack, FeedBackDto>();
        CreateMap<FeedBack, FeedBackExcelDto>();

        CreateMap<SupportedBank, SupportedBankDto>();
        CreateMap<SupportedBank, SupportedBankExcelDto>();

        CreateMap<MediaGallery, MediaGalleryDto>();
        CreateMap<MediaGallery, MediaGalleryExcelDto>();

        CreateMap<MediaGallery, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.TitleEn));
    }
}