using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.PressReleases
{
    public class PressReleaseManager : DomainService
    {
        private readonly IPressReleaseRepository _pressReleaseRepository;

        public PressReleaseManager(IPressReleaseRepository pressReleaseRepository)
        {
            _pressReleaseRepository = pressReleaseRepository;
        }

        public async Task<PressRelease> CreateAsync(
        string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, bool isFeatured, string slug, string image, string headerImage, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, DateTime date, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));
            Check.NotNullOrWhiteSpace(summaryEn, nameof(summaryEn));
            Check.NotNullOrWhiteSpace(summaryAr, nameof(summaryAr));
            Check.NotNull(date, nameof(date));

            var pressRelease = new PressRelease(
             GuidGenerator.Create(),
             titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, isFeatured, slug, image, headerImage, descriptionAr, descriptionEn, summaryEn, summaryAr, order, date, isActive
             );

            return await _pressReleaseRepository.InsertAsync(pressRelease);
        }

        public async Task<PressRelease> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, bool isFeatured, string slug, string image, string headerImage, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, DateTime date, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));
            Check.NotNullOrWhiteSpace(summaryEn, nameof(summaryEn));
            Check.NotNullOrWhiteSpace(summaryAr, nameof(summaryAr));
            Check.NotNull(date, nameof(date));

            var pressRelease = await _pressReleaseRepository.GetAsync(id);

            pressRelease.TitleAr = titleAr;
            pressRelease.TitleEn = titleEn;
            pressRelease.MetaTitleAr = metaTitleAr;
            pressRelease.MetaTitleEn = metaTitleEn;
            pressRelease.MetaDescriptionEn = metaDescriptionEn;
            pressRelease.MetaDescriptionAr = metaDescriptionAr;
            pressRelease.IsFeatured = isFeatured;
            pressRelease.Slug = slug;
            pressRelease.Image = image;
            pressRelease.HeaderImage = headerImage;
            pressRelease.DescriptionAr = descriptionAr;
            pressRelease.DescriptionEn = descriptionEn;
            pressRelease.SummaryEn = summaryEn;
            pressRelease.SummaryAr = summaryAr;
            pressRelease.Order = order;
            pressRelease.Date = date;
            pressRelease.IsActive = isActive;

            pressRelease.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _pressReleaseRepository.UpdateAsync(pressRelease);
        }

    }
}