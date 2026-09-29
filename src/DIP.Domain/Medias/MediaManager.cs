using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.Medias
{
    public partial class MediaManager : DomainService
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaManager(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<Media> CreateAsync(
        Guid? zoneParagraphId, Guid? amenityParagraphId, Guid? mediaGalleryId, string titleEn, string titleAr, string file, int order, bool isActive)
        {

            var media = new Media(
             GuidGenerator.Create(),
             zoneParagraphId, amenityParagraphId, mediaGalleryId, titleEn, titleAr, file, order, isActive
             );

            return await _mediaRepository.InsertAsync(media);
        }

        public async Task<Media> UpdateAsync(
            Guid id,
            Guid? zoneParagraphId, Guid? amenityParagraphId, Guid? mediaGalleryId, string titleEn, string titleAr, string file, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {

            var media = await _mediaRepository.GetAsync(id);

            media.ZoneParagraphId = zoneParagraphId;
            media.AmenityParagraphId = amenityParagraphId;
            media.MediaGalleryId = mediaGalleryId;
            media.TitleEn = titleEn;
            media.TitleAr = titleAr;
            media.File = file;
            media.Order = order;
            media.IsActive = isActive;

            media.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mediaRepository.UpdateAsync(media);
        }

    }
}