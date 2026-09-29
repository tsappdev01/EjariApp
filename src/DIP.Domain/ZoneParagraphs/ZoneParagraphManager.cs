using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphManager : DomainService
    {
        private readonly IZoneParagraphRepository _zoneParagraphRepository;

        public ZoneParagraphManager(IZoneParagraphRepository zoneParagraphRepository)
        {
            _zoneParagraphRepository = zoneParagraphRepository;
        }

        public async Task<ZoneParagraph> CreateAsync(
        Guid zoneId, string titleEn, string titleAr, string subTilteEn, string subTitleAr, string descriptionEn, string descriptionAr, int order, bool isActive)
        {
            Check.NotNull(zoneId, nameof(zoneId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var zoneParagraph = new ZoneParagraph(
             GuidGenerator.Create(),
             zoneId, titleEn, titleAr, subTilteEn, subTitleAr, descriptionEn, descriptionAr, order, isActive
             );

            return await _zoneParagraphRepository.InsertAsync(zoneParagraph);
        }

        public async Task<ZoneParagraph> UpdateAsync(
            Guid id,
            Guid zoneId, string titleEn, string titleAr, string subTilteEn, string subTitleAr, string descriptionEn, string descriptionAr, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNull(zoneId, nameof(zoneId));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var zoneParagraph = await _zoneParagraphRepository.GetAsync(id);

            zoneParagraph.ZoneId = zoneId;
            zoneParagraph.TitleEn = titleEn;
            zoneParagraph.TitleAr = titleAr;
            zoneParagraph.SubTilteEn = subTilteEn;
            zoneParagraph.SubTitleAr = subTitleAr;
            zoneParagraph.DescriptionEn = descriptionEn;
            zoneParagraph.DescriptionAr = descriptionAr;
            zoneParagraph.Order = order;
            zoneParagraph.IsActive = isActive;

            zoneParagraph.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _zoneParagraphRepository.UpdateAsync(zoneParagraph);
        }

    }
}