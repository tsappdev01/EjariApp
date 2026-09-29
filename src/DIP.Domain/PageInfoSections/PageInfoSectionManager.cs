using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionManager : DomainService
    {
        private readonly IPageInfoSectionRepository _pageInfoSectionRepository;

        public PageInfoSectionManager(IPageInfoSectionRepository pageInfoSectionRepository)
        {
            _pageInfoSectionRepository = pageInfoSectionRepository;
        }

        public async Task<PageInfoSection> CreateAsync(
        Guid pageInfoId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string summaryEn, string summaryAr, string descriptionEn, string descriptionAr, string pageSectionMedia, string youtubeUrl, int order, bool isActive)
        {
            Check.NotNull(pageInfoId, nameof(pageInfoId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var pageInfoSection = new PageInfoSection(
             GuidGenerator.Create(),
             pageInfoId, titleEn, titleAr, subTitleEn, subTitleAr, summaryEn, summaryAr, descriptionEn, descriptionAr, pageSectionMedia, youtubeUrl, order, isActive
             );

            return await _pageInfoSectionRepository.InsertAsync(pageInfoSection);
        }

        public async Task<PageInfoSection> UpdateAsync(
            Guid id,
            Guid pageInfoId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string summaryEn, string summaryAr, string descriptionEn, string descriptionAr, string pageSectionMedia, string youtubeUrl, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(pageInfoId, nameof(pageInfoId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var pageInfoSection = await _pageInfoSectionRepository.GetAsync(id);

            pageInfoSection.PageInfoId = pageInfoId;
            pageInfoSection.TitleEn = titleEn;
            pageInfoSection.TitleAr = titleAr;
            pageInfoSection.SubTitleEn = subTitleEn;
            pageInfoSection.SubTitleAr = subTitleAr;
            pageInfoSection.SummaryEn = summaryEn;
            pageInfoSection.SummaryAr = summaryAr;
            pageInfoSection.DescriptionEn = descriptionEn;
            pageInfoSection.DescriptionAr = descriptionAr;
            pageInfoSection.PageSectionMedia = pageSectionMedia;
            pageInfoSection.YoutubeUrl = youtubeUrl;
            pageInfoSection.Order = order;
            pageInfoSection.IsActive = isActive;

            pageInfoSection.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _pageInfoSectionRepository.UpdateAsync(pageInfoSection);
        }

    }
}