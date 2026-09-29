using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.TimeLines
{
    public class TimeLineManager : DomainService
    {
        private readonly ITimeLineRepository _timeLineRepository;

        public TimeLineManager(ITimeLineRepository timeLineRepository)
        {
            _timeLineRepository = timeLineRepository;
        }

        public async Task<TimeLine> CreateAsync(
        Guid timeLineCategoryId, string titleEn, string titleAr, string descriptionEn, string descriptionAr, string image, int order, bool isActive, DateTime? timeLineDate = null)
        {
            Check.NotNull(timeLineCategoryId, nameof(timeLineCategoryId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var timeLine = new TimeLine(
             GuidGenerator.Create(),
             timeLineCategoryId, titleEn, titleAr, descriptionEn, descriptionAr, image, order, isActive, timeLineDate
             );

            return await _timeLineRepository.InsertAsync(timeLine);
        }

        public async Task<TimeLine> UpdateAsync(
            Guid id,
            Guid timeLineCategoryId, string titleEn, string titleAr, string descriptionEn, string descriptionAr, string image, int order, bool isActive, DateTime? timeLineDate = null, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(timeLineCategoryId, nameof(timeLineCategoryId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var timeLine = await _timeLineRepository.GetAsync(id);

            timeLine.TimeLineCategoryId = timeLineCategoryId;
            timeLine.TitleEn = titleEn;
            timeLine.TitleAr = titleAr;
            timeLine.DescriptionEn = descriptionEn;
            timeLine.DescriptionAr = descriptionAr;
            timeLine.Image = image;
            timeLine.Order = order;
            timeLine.IsActive = isActive;
            timeLine.TimeLineDate = timeLineDate;

            timeLine.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _timeLineRepository.UpdateAsync(timeLine);
        }

    }
}