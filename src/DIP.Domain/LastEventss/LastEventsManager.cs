using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.LastEventss
{
    public class LastEventsManager : DomainService
    {
        private readonly ILastEventsRepository _lastEventsRepository;

        public LastEventsManager(ILastEventsRepository lastEventsRepository)
        {
            _lastEventsRepository = lastEventsRepository;
        }

        public async Task<LastEvents> CreateAsync(
        string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, bool isFeatured, string slug, string image, string headerImage, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, DateTime startDate, DateTime endDate, bool isActive, string locationAr, string locationEn)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));
            Check.NotNullOrWhiteSpace(summaryEn, nameof(summaryEn));
            Check.NotNullOrWhiteSpace(summaryAr, nameof(summaryAr));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));

            var lastEvents = new LastEvents(
             GuidGenerator.Create(),
             titleAr, titleEn, metaTitleAr, metaTitleEn, metaDescriptionEn, metaDescriptionAr, isFeatured, slug, image, headerImage, descriptionAr, descriptionEn, summaryEn, summaryAr, order, startDate, endDate, isActive, locationAr, locationEn
             );

            return await _lastEventsRepository.InsertAsync(lastEvents);
        }

        public async Task<LastEvents> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, string metaTitleAr, string metaTitleEn, string metaDescriptionEn, string metaDescriptionAr, bool isFeatured, string slug, string image, string headerImage, string descriptionAr, string descriptionEn, string summaryEn, string summaryAr, int order, DateTime startDate, DateTime endDate, bool isActive, string locationAr, string locationEn, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));
            Check.NotNullOrWhiteSpace(summaryEn, nameof(summaryEn));
            Check.NotNullOrWhiteSpace(summaryAr, nameof(summaryAr));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));

            var lastEvents = await _lastEventsRepository.GetAsync(id);

            lastEvents.TitleAr = titleAr;
            lastEvents.TitleEn = titleEn;
            lastEvents.MetaTitleAr = metaTitleAr;
            lastEvents.MetaTitleEn = metaTitleEn;
            lastEvents.MetaDescriptionEn = metaDescriptionEn;
            lastEvents.MetaDescriptionAr = metaDescriptionAr;
            lastEvents.IsFeatured = isFeatured;
            lastEvents.Slug = slug;
            lastEvents.Image = image;
            lastEvents.HeaderImage = headerImage;
            lastEvents.DescriptionAr = descriptionAr;
            lastEvents.DescriptionEn = descriptionEn;
            lastEvents.SummaryEn = summaryEn;
            lastEvents.SummaryAr = summaryAr;
            lastEvents.Order = order;
            lastEvents.StartDate = startDate;
            lastEvents.EndDate = endDate;
            lastEvents.IsActive = isActive;
            lastEvents.LocationAr = locationAr;
            lastEvents.LocationEn = locationEn;

            lastEvents.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _lastEventsRepository.UpdateAsync(lastEvents);
        }

    }
}