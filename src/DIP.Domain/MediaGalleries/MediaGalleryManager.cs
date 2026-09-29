using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.MediaGalleries
{
    public class MediaGalleryManager : DomainService
    {
        private readonly IMediaGalleryRepository _mediaGalleryRepository;

        public MediaGalleryManager(IMediaGalleryRepository mediaGalleryRepository)
        {
            _mediaGalleryRepository = mediaGalleryRepository;
        }

        public async Task<MediaGallery> CreateAsync(
        string titleEn, string slug, string titleAr, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, string summaryEn, string summaryAr, string headerImage, string image, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var mediaGallery = new MediaGallery(
             GuidGenerator.Create(),
             titleEn, slug, titleAr, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, summaryEn, summaryAr, headerImage, image, order, isActive
             );

            return await _mediaGalleryRepository.InsertAsync(mediaGallery);
        }

        public async Task<MediaGallery> UpdateAsync(
            Guid id,
            string titleEn, string slug, string titleAr, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, string summaryEn, string summaryAr, string headerImage, string image, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var mediaGallery = await _mediaGalleryRepository.GetAsync(id);

            mediaGallery.TitleEn = titleEn;
            mediaGallery.Slug = slug;
            mediaGallery.TitleAr = titleAr;
            mediaGallery.MetaTitleEn = metaTitleEn;
            mediaGallery.MetaTitleAr = metaTitleAr;
            mediaGallery.MetaDescriptionEn = metaDescriptionEn;
            mediaGallery.MetaDescriptionAr = metaDescriptionAr;
            mediaGallery.SummaryEn = summaryEn;
            mediaGallery.SummaryAr = summaryAr;
            mediaGallery.HeaderImage = headerImage;
            mediaGallery.Image = image;
            mediaGallery.Order = order;
            mediaGallery.IsActive = isActive;

            mediaGallery.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _mediaGalleryRepository.UpdateAsync(mediaGallery);
        }

    }
}