using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphManager : DomainService
    {
        private readonly IAmenityParagraphRepository _amenityParagraphRepository;

        public AmenityParagraphManager(IAmenityParagraphRepository amenityParagraphRepository)
        {
            _amenityParagraphRepository = amenityParagraphRepository;
        }

        public async Task<AmenityParagraph> CreateAsync(
        Guid amenityId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string descriptionEn, string descriptionAr, string buttonUrl, int order, bool isActive)
        {
            Check.NotNull(amenityId, nameof(amenityId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var amenityParagraph = new AmenityParagraph(
             GuidGenerator.Create(),
             amenityId, titleEn, titleAr, subTitleEn, subTitleAr, descriptionEn, descriptionAr, buttonUrl, order, isActive
             );

            return await _amenityParagraphRepository.InsertAsync(amenityParagraph);
        }

        public async Task<AmenityParagraph> UpdateAsync(
            Guid id,
            Guid amenityId, string titleEn, string titleAr, string subTitleEn, string subTitleAr, string descriptionEn, string descriptionAr, string buttonUrl, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(amenityId, nameof(amenityId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var amenityParagraph = await _amenityParagraphRepository.GetAsync(id);

            amenityParagraph.AmenityId = amenityId;
            amenityParagraph.TitleEn = titleEn;
            amenityParagraph.TitleAr = titleAr;
            amenityParagraph.SubTitleEn = subTitleEn;
            amenityParagraph.SubTitleAr = subTitleAr;
            amenityParagraph.DescriptionEn = descriptionEn;
            amenityParagraph.DescriptionAr = descriptionAr;
            amenityParagraph.ButtonUrl = buttonUrl;
            amenityParagraph.Order = order;
            amenityParagraph.IsActive = isActive;

            amenityParagraph.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _amenityParagraphRepository.UpdateAsync(amenityParagraph);
        }

    }
}