using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoryManager : DomainService
    {
        private readonly ITimeLineCategoryRepository _timeLineCategoryRepository;

        public TimeLineCategoryManager(ITimeLineCategoryRepository timeLineCategoryRepository)
        {
            _timeLineCategoryRepository = timeLineCategoryRepository;
        }

        public async Task<TimeLineCategory> CreateAsync(
        string titleEn, string titleAr, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var timeLineCategory = new TimeLineCategory(
             GuidGenerator.Create(),
             titleEn, titleAr, order, isActive
             );

            return await _timeLineCategoryRepository.InsertAsync(timeLineCategory);
        }

        public async Task<TimeLineCategory> UpdateAsync(
            Guid id,
            string titleEn, string titleAr, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var timeLineCategory = await _timeLineCategoryRepository.GetAsync(id);

            timeLineCategory.TitleEn = titleEn;
            timeLineCategory.TitleAr = titleAr;
            timeLineCategory.Order = order;
            timeLineCategory.IsActive = isActive;

            timeLineCategory.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _timeLineCategoryRepository.UpdateAsync(timeLineCategory);
        }

    }
}