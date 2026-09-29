using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.MajorIndustries
{
    public class MajorIndustryManager : DomainService
    {
        private readonly IMajorIndustryRepository _majorIndustryRepository;

        public MajorIndustryManager(IMajorIndustryRepository majorIndustryRepository)
        {
            _majorIndustryRepository = majorIndustryRepository;
        }

        public async Task<MajorIndustry> CreateAsync(
        Guid zoneId, string titleEn, string titleAr, string image, int order, bool isActive)
        {
            Check.NotNull(zoneId, nameof(zoneId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var majorIndustry = new MajorIndustry(
             GuidGenerator.Create(),
             zoneId, titleEn, titleAr, image, order, isActive
             );

            return await _majorIndustryRepository.InsertAsync(majorIndustry);
        }

        public async Task<MajorIndustry> UpdateAsync(
            Guid id,
            Guid zoneId, string titleEn, string titleAr, string image, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(zoneId, nameof(zoneId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var majorIndustry = await _majorIndustryRepository.GetAsync(id);

            majorIndustry.ZoneId = zoneId;
            majorIndustry.TitleEn = titleEn;
            majorIndustry.TitleAr = titleAr;
            majorIndustry.Image = image;
            majorIndustry.Order = order;
            majorIndustry.IsActive = isActive;

            majorIndustry.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _majorIndustryRepository.UpdateAsync(majorIndustry);
        }

    }
}