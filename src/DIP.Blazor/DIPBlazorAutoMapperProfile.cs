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
using Volo.Abp.AutoMapper;
using DIP.SliderHomePages;
using AutoMapper;

namespace DIP.Blazor;

public class DIPBlazorAutoMapperProfile : Profile
{
    public DIPBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<SliderHomePageDto, SliderHomePageUpdateDto>();

        CreateMap<DipFactDto, DipFactUpdateDto>();

        CreateMap<PressReleaseDto, PressReleaseUpdateDto>();

        CreateMap<LastEventsDto, LastEventsUpdateDto>();

        CreateMap<PageInfoDto, PageInfoUpdateDto>();

        CreateMap<EServiceDto, EServiceUpdateDto>();

        CreateMap<EFormServiceDto, EFormServiceUpdateDto>();

        CreateMap<EFormServiceSubCategoryDto, EFormServiceSubCategoryUpdateDto>();

        CreateMap<ZoneDto, ZoneUpdateDto>();

        CreateMap<ZoneParagraphDto, ZoneParagraphUpdateDto>();

        CreateMap<MediaDto, MediaUpdateDto>();

        CreateMap<AmenityDto, AmenityUpdateDto>();

        CreateMap<AmenityParagraphDto, AmenityParagraphUpdateDto>();

        CreateMap<TimeLineCategoryDto, TimeLineCategoryUpdateDto>();

        CreateMap<TimeLineDto, TimeLineUpdateDto>();

        CreateMap<CategoryDto, CategoryUpdateDto>();

        CreateMap<SubCategoryDto, SubCategoryUpdateDto>();

        CreateMap<CommercialDto, CommercialUpdateDto>();

        CreateMap<DipBranchDto, DipBranchUpdateDto>();

        CreateMap<MajorIndustryDto, MajorIndustryUpdateDto>();

        CreateMap<SiteSettingDto, SiteSettingUpdateDto>();

        CreateMap<PageInfoSectionDto, PageInfoSectionUpdateDto>();

        CreateMap<InquiryFormDto, InquiryFormUpdateDto>();

        CreateMap<ContactFormDto, ContactFormUpdateDto>();

        CreateMap<FeedBackDto, FeedBackUpdateDto>();

        CreateMap<SupportedBankDto, SupportedBankUpdateDto>();

        CreateMap<MediaGalleryDto, MediaGalleryUpdateDto>();
    }
}