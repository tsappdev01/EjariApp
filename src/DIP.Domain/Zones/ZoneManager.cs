using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.Zones
{
    public partial class ZoneManager : DomainService
    {
        private readonly IZoneRepository _zoneRepository;

        public ZoneManager(IZoneRepository zoneRepository)
        {
            _zoneRepository = zoneRepository;
        }

        public async Task<Zone> CreateAsync(
        string titleEn, string titleAR, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string summaryEn, string summaryAr, string image, string headerImage, int order, bool isFeature, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAR, nameof(titleAR));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var zone = new Zone(
             GuidGenerator.Create(),
             titleEn, titleAR, metaTitleEn, metaDescriptionEn, metaTitleAr, metaDescriptionAr, slug, summaryEn, summaryAr, image, headerImage, order, isFeature, isActive
             );

            return await _zoneRepository.InsertAsync(zone);
        }

        public async Task<Zone> UpdateAsync(
            Guid id,
            string titleEn, string titleAR, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string summaryEn, string summaryAr, string image, string headerImage, int order, bool isFeature, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAR, nameof(titleAR));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var zone = await _zoneRepository.GetAsync(id);

            zone.TitleEn = titleEn;
            zone.TitleAR = titleAR;
            zone.MetaTitleEn = metaTitleEn;
            zone.MetaDescriptionEn = metaDescriptionEn;
            zone.MetaTitleAr = metaTitleAr;
            zone.MetaDescriptionAr = metaDescriptionAr;
            zone.Slug = slug;
            zone.SummaryEn = summaryEn;
            zone.SummaryAr = summaryAr;
            zone.Image = image;
            zone.HeaderImage = headerImage;
            zone.Order = order;
            zone.IsFeature = isFeature;
            zone.IsActive = isActive;

            zone.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _zoneRepository.UpdateAsync(zone);
        }

    }
}